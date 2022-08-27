using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AsyncUtils;
using Match3.Core;
using Match3.Core.GameActions.Actions;
using Match3.Core.GameActions.Interactions;
using Match3.Core.GameDataExtraction;
using Match3.Core.GameEvents;
using Match3.Core.Gravity;
using Match3.Core.Levels;
using Match3.Core.TurnSteps;
using Match3.View.GameEndConditions;
using Match3.View.Interactions;
using NaughtyAttributes;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;

namespace Match3.View
{
    public class GameControllerView : MonoBehaviour
    {
        [SerializeField] private BoardView boardView;
        [SerializeField] private GameEndConditionsView gameEndView;
        [SerializeField] private SkillsInitializer _skillsInitializer;
        [SerializeField] private SwapInteractionView _swapInteractionView;
        [SerializeField] private DoubleClickInteractionView _doubleClickInteractionView;
        
        [SerializeField] private SerializableEventsProvider _eventsProvider;
        [SerializeField] private GameContextAsset _gameContext;
        
        [SerializeField] private PopupSkipTurn _skipPopupPrefab;

        /*
         * the bool parameter tells whether the game was won or not.
         */
        [SerializeField] private UnityEvent<bool> _onGameEnd;
        [SerializeField] private BoolEvent _gameEndEvent;

        private GameController _gameController;
        private List<SkillView> _skillViews;
        private Level _lastLevel;
        
        private CancellationTokenSource _cts;
        private bool _gameLoopIsRunning;

#if UNITY_EDITOR
        private bool _editorRequestStop;
        private bool _requestedResult;
#endif

        private bool ExecutingTurn { get; set; }

        public GameController GameController => _gameController;

        private void Start()
        {
            InitializeSkillsAndInteractionViews();
        }

        private void OnDisable()
        {
            StopGameLoop();
        }

        public async void StartGameInLevel(Level level)
        {
            if (Application.isPlaying)
            {
                StopGameLoop();
                _cts = new CancellationTokenSource();
                var ct = _cts.Token;
                await WaitUntilLoopStopped(ct);
                
                PopulateLevel(level);
                
                await StartGameLoop(ct);
            }
            else
            {
                PopulateLevel(level);
            }
        }

        public async void ResumeCurrentGame()
        {
            if (Application.isPlaying)
            {
                StopGameLoop();
                _cts = new CancellationTokenSource();
                var ct = _cts.Token;
                await WaitUntilLoopStopped(ct);
                
                await ResumeGameLoop(ct);
            }
        }

        public void RestartCurrentLevel()
        {
            StartGameInLevel(_lastLevel);
        }

#if UNITY_EDITOR
        [Button("Win current game")]
        private void WinCurrentLevel()
        {
            _editorRequestStop = true;
            _requestedResult = true;
            StopGameLoop();
        }
        
        [Button("Lose current game")]
        private void LoseCurrentLevel()
        {
            _editorRequestStop = true;
            _requestedResult = false;
            StopGameLoop();
        }
#endif
        
        private void PopulateLevel(Level level)
        {
            var victoryEvaluator = level.VictoryEvaluator;
            var defeatEvaluator = level.DefeatEvaluator;
            var context = _gameContext == null ? GameContext.GetDefault() : _gameContext.GameContextCopy;
            context.EventsProvider = _eventsProvider;
            _gameController = new GameController(level, context);
            boardView.UpdateView(_gameController.Board);
            gameEndView.SetupUi(victoryEvaluator, defeatEvaluator);
            _lastLevel = level;
        }

        private void InitializeSkillsAndInteractionViews()
        {
            _skillViews = _skillsInitializer.InitializeSkills();
            _swapInteractionView.Initialize();
            _doubleClickInteractionView.Initialize();
        }
        
#region Game loop

        private async Task StartGameLoop(CancellationToken ct)
        {
            var turn = _gameController.StartGame();
            var (victory, defeat) = await ExecuteTurn(turn, ct);
            bool gameEnd = victory || defeat;

            if (!gameEnd)
            {
                victory = await ExecuteGameLoop(ct);
            }
            
            NotifyGameEnd(victory);
        }
        
        private async Task ResumeGameLoop(CancellationToken ct)
        {
            var victory = await ExecuteGameLoop(ct);
            NotifyGameEnd(victory);
        }

        private void NotifyGameEnd(bool victory)
        {
            _gameController.EndGame();
            _onGameEnd?.Invoke(victory);
            _gameEndEvent.Raise(victory);
        }

        private async Task<bool> ExecuteGameLoop(CancellationToken ct)
        {
            try
            {
                _gameLoopIsRunning = true;

#if UNITY_EDITOR
                _editorRequestStop = false;
                _requestedResult = false;
#endif

                bool gameEnd = false;
                bool victory = false;

                // game loop
                while (!gameEnd && !ct.IsCancellationRequested)
                {
                    var (interaction, action) = await WaitForInteractionAsync(ct);
                    if (interaction == null || action == null) continue;

                    var turn = _gameController.ExecuteGameAction(interaction, action);
                    if (turn == null) continue;

                    var defeat = false;
                    (victory, defeat) = await ExecuteTurn(turn, ct);
                    gameEnd = victory || defeat;
                }

#if UNITY_EDITOR
                if (_editorRequestStop)
                {
                    victory = _requestedResult;
                }
#endif

                return victory;
            }
            finally
            {
                _gameLoopIsRunning = false;
            }
        }

        private void StopGameLoop()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        private async Task WaitUntilLoopStopped(CancellationToken ct)
        {
            while (_gameLoopIsRunning && !ct.IsCancellationRequested)
            {
                await Task.Yield();
            }
        }
        
        private async Task<(IInteraction, IGameAction)> WaitForInteractionAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                var linkedCt = linkedCts.Token;
                Task allTasks = null;
                try
                {
                    var swapTask = _swapInteractionView.WaitInteractionAsync(linkedCt);
                    var doubleClickTask = _doubleClickInteractionView.WaitInteractionAsync(linkedCt);
                    var skillsPressedTasks =
                        _skillViews.ConvertAll(skillView => skillView.WaitForSkillPressed(linkedCt));
                    var firstSkillPressedTask = skillsPressedTasks.Count > 0
                        ? Task.WhenAny(skillsPressedTasks)
                        : AsyncUtils.Utils.NeverEndTaskAsync<Task<SkillView>>(linkedCt);

                    allTasks = Task.WhenAll(swapTask, doubleClickTask, firstSkillPressedTask);
                    var firstFinished = await Task.WhenAny(swapTask, doubleClickTask, firstSkillPressedTask);
                    await firstFinished;

                    if (firstFinished == firstSkillPressedTask) // skill pressed
                    {
                        var skillPressedTask = await firstSkillPressedTask;
                        var skillView = await skillPressedTask;
                        linkedCts.Cancel(); // cancel other async tasks

                        if (skillView.IsSkillUsable)
                        {
                            // wait for skill interaction
                            var linkedCtsInteraction = CancellationTokenSource.CreateLinkedTokenSource(ct);
                            var linkedCtInteraction = linkedCtsInteraction.Token;
                            try
                            {
                                var interactionTask = skillView.WaitForInteractionAsync(linkedCtInteraction);
                                var (interaction, gameAction, success) = await interactionTask;
                                if (success)
                                {
                                    return (interaction, gameAction);
                                }
                            }
                            finally
                            {
                                linkedCtsInteraction.Cancel();
                                linkedCtsInteraction.Dispose();
                            }
                        }
                        else
                        {
                            skillView.CostView.OnCantApplyCost();
                            await Task.Yield(); // this avoids breaking the loop, don't know why
                        }
                    }
                    else if (firstFinished == swapTask) // swapped
                    {
                        var (interaction, success) = await swapTask;
                        if (success)
                        {
                            var action = new SwapGameAction();
                            return (interaction, action);
                        }
                    }
                    else // double clicked
                    {
                        var (interaction, success) = await doubleClickTask;
                        if (success)
                        {
                            var action = new DoubleClickGameAction();
                            return (interaction, action);
                        }
                    }
                }
                finally
                {
                    if (!linkedCts.IsCancellationRequested)
                    {
                        linkedCts.Cancel();
                        try
                        {
                            if (allTasks != null)
                            {
                                await allTasks;  // wait for cancellation
                            }
                        }
                        finally{}
                    }
                    linkedCts.Dispose();
                }
            }
            return (null, null);
        }

        private async Task<(bool, bool)> ExecuteTurn(Turn turn, CancellationToken ct)
        {
            ExecutingTurn = true;
            var victoryEvaluator = _gameController.CurrentLevel.VictoryEvaluator;
            var defeatEvaluator = _gameController.CurrentLevel.DefeatEvaluator;
            var gameData = _gameController.GameData;

            bool victory = false;
            bool defeat = false;
            Task skipTask = null;
            bool skipTurn = false;
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var linkedCt = linkedCts.Token;
            try
            {
                foreach (var turnStep in turn.TurnSteps)
                {
                    if (turnStep is TurnStepGameEndVictory)
                    {
                        victory = true;
                        // enable skip button
                        skipTask = Popups.ShowPopup(_skipPopupPrefab, linkedCt);
                    }
                    else if (turnStep is TurnStepGameEndDefeat)
                    {
                        defeat = true;
                    }

                    if (defeat && !victory)
                        break;

                    if (!skipTurn)
                    {
                        var renderTask = boardView.RenderTurnStepAsync(turnStep, ct);
                        if (skipTask != null)
                        {
                            var finishedTask = await Task.WhenAny(renderTask, skipTask);
                            if (finishedTask == skipTask)
                            {
                                skipTurn = true;
                            }
                        }
                        else
                        {
                            await renderTask;
                        }
                        gameEndView.UpdateUi(victoryEvaluator, defeatEvaluator, gameData);
                    }
                }
            }
            finally
            {
                linkedCts.Cancel();
                linkedCts.Dispose();
            }

            ExecutingTurn = false;
            
            return (victory, defeat);
        }

#endregion
    }
}
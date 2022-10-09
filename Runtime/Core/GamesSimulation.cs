using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Match3.Core.GameActions.Actions;
using Match3.Core.GameActions.Interactions;
using Match3.Core.GameDataExtraction;
using Match3.Core.GameEvents;
using Match3.Core.Gravity;
using Match3.Core.Levels;
using Match3.Core.TokensEvents.Events;
using Match3.Core.TurnSteps;
using Match3.Settings;
using UnityEngine;
using Utils.Extensions;
using Random = UnityEngine.Random;

namespace Match3.Core
{
    public class SimulationReport
    {
        public int GamesCount;
        public int GamesWon;
        public List<int> GamesWonTurnCount;
        public List<GameData> GamesData;

        public SimulationReport()
        {
            GamesCount = 0;
            GamesWon = 0;
            GamesWonTurnCount = new List<int>();
            GamesData = new List<GameData>();
        }

        public float WinRate => (float) GamesWon / GamesCount;
        
        public float MeanTurnCount()
        {
            float sum = 0;
            foreach (var count in GamesWonTurnCount)
            {
                sum += count;
            }

            return sum / GamesWonTurnCount.Count;
        }
    }
    
    public static class GamesSimulation
    {
        public static async Task<SimulationReport> SimulateGamesInLevelAsync(
            Level level,
            int gamesCount,
            List<IDataExtractor> additionalDataExtractors = null)
        {
            var report = new SimulationReport();
            
            var start = DateTime.Now;
            for (int i = 0; i < gamesCount; i++)
            {
                var gameController = SetupGame(level);
                var disposableDataExtractor = CreateDataExtractor(gameController.Context.EventsProvider, additionalDataExtractors);
                using (disposableDataExtractor)
                {
                    var won = await PlayGame(gameController);
                    FillReportWithFinishedGame(report, gameController, disposableDataExtractor.Extractor, won);
                }
                
                int gamesPlayed = i + 1;
                if (gamesPlayed % 10 == 0)
                {
                    var elapsed = (DateTime.Now - start).TotalMilliseconds / 1000;
                    Debug.Log($"Simulated {gamesPlayed} games; elapsed time: {elapsed}");
                }

                await Task.Yield();
            }

            return report;
        }

        private static DisposableDataExtractor CreateDataExtractor(
            IGameEventsProvider eventsProvider,
            List<IDataExtractor> additionalDataExtractors)
        {
            var dataExtractor = new DisposableDataExtractor(eventsProvider);
            var extractors = additionalDataExtractors ?? new List<IDataExtractor>();
            extractors.ForEach(extractor => dataExtractor.Extractor.AddDataExtractorIfNotExists(extractor));
            return dataExtractor;
        }

        private static void FillReportWithFinishedGame(
            SimulationReport report,
            GameController gameController,
            GameObserverDataExtractor dataExtractor,
            bool won)
        {
            report.GamesCount++;
            report.GamesData.Add(dataExtractor.CurrentGameData);
            if (won)
            {
                report.GamesWon++;
                report.GamesWonTurnCount.Add(gameController.TurnCount);
            }
        }

        private static GameController SetupGame(
            Level level,
            GameContext context = null)
        {
            if (context == null)
            {
                if (SimulationSettings.Instance != null && SimulationSettings.Instance.DefaultSimulationContext != null)
                    context = SimulationSettings.Instance.DefaultSimulationContext.GameContextCopy;
            }

            if (context == null)
            {
                context = GameContext.GetDefault();
            }

            var gameController = new GameController(
                level,
                context
            );
            return gameController;
        }
        
        private static async Task<bool> PlayGame(GameController gameController, int turns=-1)
        {
            bool gameEnd = false;
            bool won = false;
            bool gameStarted = false;
            bool TurnsLimitReached() {return turns != -1 && gameController.TurnCount < turns;}
            while (!gameEnd || TurnsLimitReached())
            {
                Turn turn;
                if (!gameStarted)
                {
                    gameStarted = true;
                    turn = gameController.StartGame();
                }
                else
                {
                    turn = PlayRandomMove(gameController);
                }
                foreach (var turnStep in turn.TurnSteps)
                {
                    if (turnStep is TurnStepGameEndVictory)
                    {
                        gameEnd = true;
                        won = true;
                    }
                    else if (turnStep is TurnStepGameEndDefeat)
                    {
                        gameEnd = true;
                        won = false;
                    }
                }

                await Task.Yield();
                if (gameController.TurnCount % 100 == 0)
                    Debug.Log($"\t\tturn {gameController.TurnCount}");
            }
            
            gameController.EndGame();

            return won;
        }

        private static Turn PlayRandomMove(GameController gameController)
        {
            var moves = GetAllPossibleMoves(gameController);
            var move = moves.GetRandom();
            if (Random.value > 0.5f)
                move = (move.Item2, move.Item1);  // invert
            
            var interaction = new SwapInteraction(move.Item1, move.Item2);
            var swapAction = new SwapGameAction();
            var turn = gameController.ExecuteGameAction(interaction, swapAction);
            
            return turn;
        }

        public static List<(Vector2Int, Vector2Int)> GetAllPossibleMoves(GameController controller)
        {
            var board = controller.Board;
            var moves = new List<(Vector2Int, Vector2Int)>();
            var positions = board.MainLayer.GetPositions();
            var dirs = new List<Vector2Int>
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };
            var solutions = new List<TokenData>();
            
            foreach (var position in positions)
            {
                bool canMove = GravityUtils.CanMoveFrom(board, position);
                if (!canMove)
                    continue;
                
                var token = board.MainLayer.GetTokenAt(position);
                var tokenData = token.TokenData;
                foreach (var dir in dirs)
                {
                    var adjacent = position + dir;
                    bool canMoveAdjacent = GravityUtils.CanMoveFrom(board, adjacent);
                    if (!canMoveAdjacent)
                        continue;
                    
                    bool hasSwapResolver = tokenData.Resolvers.Any(resolver => resolver.EventType.Type == typeof(EventSwapped));
                    bool isSolution = false;
                    if (!hasSwapResolver)
                    {
                        solutions.Clear();
                        board.GetAllSolutionsInDirection(controller.Context, position, dir, solutions);
                        isSolution = solutions.Contains(tokenData);
                    }
                    
                    if (hasSwapResolver || isSolution)
                    {
                        moves.Add((position, adjacent));
                    }
                }
            }

            return moves;
        }
    }
    
    internal class DisposableDataExtractor : IDisposable
    {
        private GameObserverDataExtractor _extractor;
        private IGameEventsProvider _eventsProvider;

        public GameObserverDataExtractor Extractor => _extractor;

        public DisposableDataExtractor(IGameEventsProvider eventsProvider)
        {
            _extractor = GameObserverDataExtractor.CreateEmpty();
            _eventsProvider = eventsProvider;
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            _eventsProvider.GameStartedEvent.Register(_extractor.OnGameStarted);
            _eventsProvider.GameEndedEvent.Register(_extractor.OnGameEnded);
            _eventsProvider.TurnStepEvent.Register(_extractor.OnTurnStep);
        }
        
        private void UnregisterEvents()
        {
            _eventsProvider.GameStartedEvent.Register(_extractor.OnGameStarted);
            _eventsProvider.GameEndedEvent.Register(_extractor.OnGameEnded);
            _eventsProvider.TurnStepEvent.Register(_extractor.OnTurnStep);
        }

        public void Dispose()
        {
            UnregisterEvents();
        }
    }
}
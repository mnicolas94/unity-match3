using System;
using System.Collections.Generic;
using Match3.Core.GameActions;
using Match3.Core.GameActions.Actions;
using Match3.Core.GameActions.Interactions;
using Match3.Core.GameDataExtraction;
using Match3.Core.GameEndConditions.Defeat;
using Match3.Core.GameEndConditions.Victory;
using Match3.Core.GameEvents;
using Match3.Core.Gravity;
using Match3.Core.Levels;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.Core
{
    [Serializable]
    public class GameController
    {
        private Board _board;
        private Level _currentLevel;
        private TokensCreator _tokensCreator;
        private IVictoryEvaluator _victoryEvaluator;
        private IDefeatEvaluator _defeatEvaluator;
        private GameContext _context;

        private int _turnCount;
        
        public int TurnCount => _turnCount;
        public Board Board => _board;
        public Level CurrentLevel => _currentLevel;

        public GameContext Context => _context;

        public GameController(
            Level level,
            GameContext context)
        {
            _turnCount = 0;
            _currentLevel = level;
            _tokensCreator = new TokensCreator(level.TokensCreationData, context.TokenCreationRequests);
            _victoryEvaluator = level.VictoryEvaluator;
            _defeatEvaluator = level.DefeatEvaluator;
            _context = context;
            _board = Board.PopulateLevel(level, context);
            _victoryEvaluator.Initialize(this);
            _defeatEvaluator.Initialize(this);
        }

        public Turn StartGame()
        {
            _context.EventsProvider.GameStartedEvent.Raise(this);
            var action = new DoNothingAction();
            return ExecuteGameAction(null, action);
        }

        public void EndGame()
        {
            _context.EventsProvider.GameEndedEvent.Raise(this);
        }

        public Turn ExecuteGameAction(IInteraction interaction, IGameAction action)
        {
            var turn = BoardGameActions.ExecuteAction(_context, _board, interaction, action, SpawnToken);
            HandleTurn(turn);
            return turn;
        }
        
        private void HandleTurn(Turn turn)
        {
            turn.TransformTurnSteps(steps => HandleTurn(steps, turn.CountAsTurn));
            turn.TransformTurnSteps(RaiseTurnStepsEvent);
        }

        private IEnumerable<TurnStep> HandleTurn(IEnumerable<TurnStep> turn, bool turnCounts)
        {
            bool defeat = false;
            bool victory = false;
            
            // begin turn
            var beginStep = new TurnStepTurnBegin();
            yield return beginStep;
            
            foreach (var turnStep in turn)
            {
                yield return turnStep;
                
                if (!victory)
                {
                    victory = CheckVictoryCondition(turnStep);
                    if (victory)
                    {
                        var victoryStep = new TurnStepGameEndVictory();
                        yield return victoryStep;
                    }
                }
                if (!defeat)
                {
                    defeat = CheckDefeatCondition(turnStep);
                }
            }
            
            // end turn
            var endStep = new TurnStepTurnEnd(turnCounts);
            // check defeat condition again
            defeat = CheckDefeatCondition(endStep);
            yield return endStep;

            if (!victory && defeat)
            {
                var defeatStep = new TurnStepGameEndDefeat();
                yield return defeatStep;
            }
            
            // increment turn count
            _turnCount += turnCounts ? 1 : 0;
        }

        private IEnumerable<TurnStep> RaiseTurnStepsEvent(IEnumerable<TurnStep> turn)
        {
            foreach (var turnStep in turn)
            {
                RaiseTurnStepEvent(turnStep);
                yield return turnStep;
            }
        }

        private void RaiseTurnStepEvent(TurnStep turnStep)
        {
            _context.EventsProvider.TurnStepEvent.Raise(turnStep);
        }

        private bool CheckVictoryCondition(TurnStep turnStep)
        {
            return _victoryEvaluator.CheckVictoryInTurnStep(turnStep);
        }
        
        private bool CheckDefeatCondition(TurnStep turnStep)
        {
            return _defeatEvaluator.CheckDefeatInTurnStep(turnStep);
        }
        
        private TokenData SpawnToken(Vector2Int position, TokenSource tokenSource)
        {
            var token = tokenSource.ProvidesToken()
                ? tokenSource.GetToken()
                : _tokensCreator.SpawnToken(this, tokenSource, position);
            return token;
        }
    }
}
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

        private GameData _gameData;

        public GameData GameData => _gameData;
        public int TurnCount => _gameData.TurnCount;
        public Board Board => _board;
        public Level CurrentLevel => _currentLevel;

        public GameContext Context => _context;

        public GameController(
            Level level,
            GameContext context)
        {
            _gameData = new GameData
            {
                TurnCount = 0,
                AllTurnsData = new GameExtractedData(),
                LastTurnData = new GameExtractedData()
            };
            _currentLevel = level;
            _tokensCreator = new TokensCreator(level.TokensCreationData);
            _victoryEvaluator = level.VictoryEvaluator;
            _defeatEvaluator = level.DefeatEvaluator;
            _context = context;
            _context.DataExtractors.AddRange(_victoryEvaluator.GetDataExtractors());
            _context.DataExtractors.AddRange(_defeatEvaluator.GetDataExtractors());
            _board = Board.PopulateLevel(level, context);
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
            turn.TransformTurnSteps(HandleTurn);
            if (turn.CountAsTurn)
                _gameData.TurnCount++;
        }

        private IEnumerable<TurnStep> HandleTurn(IEnumerable<TurnStep> turn)
        {
            bool defeat = false;
            bool victory = false;
            _gameData.LastTurnData.ClearData();
            foreach (var turnStep in turn)
            {
                ExtractData(turnStep);
                RaiseTurnStepEvent(turnStep);

                yield return turnStep;
                
                if (!victory)
                {
                    victory = CheckVictoryCondition(turnStep, _gameData);
                    if (victory)
                    {
                        var victoryStep = new TurnStepGameEndVictory();
                        ExtractData(victoryStep);
                        RaiseTurnStepEvent(victoryStep);
                        yield return victoryStep;
                    }
                }
                if (!defeat)
                {
                    defeat = CheckDefeatCondition(turnStep, _gameData);
                }
            }

            if (!victory && defeat)
            {
                var defeatStep = new TurnStepGameEndDefeat();
                ExtractData(defeatStep);
                RaiseTurnStepEvent(defeatStep);
                yield return defeatStep;
            }
        }

        private bool CheckVictoryCondition(TurnStep turnStep, GameData gameData)
        {
            return _victoryEvaluator.CheckVictoryInTurnStep(turnStep, gameData);
        }
        
        private bool CheckDefeatCondition(TurnStep turnStep, GameData gameData)
        {
            return _defeatEvaluator.CheckDefeatInTurnStep(turnStep, gameData);
        }

        private void ExtractData(TurnStep turnStep)
        {
            foreach (var extractor in _context.DataExtractors)
            {
                if (extractor.CanExtractDataFrom(turnStep))
                {
                    var data = extractor.ExtractData(turnStep);
                    _gameData.LastTurnData.AggregateData(data);
                    _gameData.AllTurnsData.AggregateData(data);
                }
            }
        }
        
        private void RaiseTurnStepEvent(TurnStep turnStep)
        {
            _context.EventsProvider.TurnStepEvent.Raise(turnStep);
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
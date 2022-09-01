using System.Collections.Generic;
using JetBrains.Annotations;
using Match3.Core.GameEvents.Observers;
using Match3.Core.TurnSteps;
using UnityEngine;
using Utils.Attributes;
using Utils.Extensions;

namespace Match3.Core.GameDataExtraction
{
    [CreateAssetMenu(
        fileName = "GameObserverDataExtractor",
        menuName = "Facticus/Match3/DataExtraction/GameObserverDataExtractor",
        order = 0)]
    public class GameObserverDataExtractor : ScriptableObject, IGameStartObserver, IGameEndedObserver, ITurnStepObserver
    {
        [SerializeField, AutoProperty(AutoPropertyMode.Asset)]
        private GlobalDataExtractedStorage _globalDataStorage;
        
        [SerializeReference, SubclassSelector] private List<IDataExtractor> _dataExtractors;
        
        private GameData _currentGameData;
        [CanBeNull] public GameData CurrentGameData => _currentGameData;

        public static GameObserverDataExtractor CreateEmpty()
        {
            var extractor = CreateInstance<GameObserverDataExtractor>();
            var storage = GlobalDataExtractedStorage.CreateEmpty();
            extractor._globalDataStorage = storage;
            extractor._dataExtractors = new List<IDataExtractor>();
            return extractor;
        }
        
        public void AddDataExtractorIfNotExists(IDataExtractor dataExtractor)
        {
            _dataExtractors.AddIfNotExists(dataExtractor);
        }
        
        public void OnGameStarted(GameController controller)
        {
            _currentGameData = new GameData();
            _globalDataStorage.AddGamesStarted();
        }

        public void OnGameEnded(GameController controller)
        {
            _globalDataStorage.AddGamesCompleted();
            _globalDataStorage.AggregateData(_currentGameData.AllTurnsData);
        }

        public void OnTurnStep(TurnStep step)
        {
            if (step is TurnStepTurnBegin)
            {
                _currentGameData.LastTurnData.ClearData();
            }

            // raise turn count if is end of turn
            if (step is TurnStepTurnEnd endStep)
            {
                if (endStep.CountAsTurn)
                {
                    _currentGameData.TurnCount++;
                }
            }
            
            if (step is TurnStepGameEndVictory)
            {
                _globalDataStorage.AddGamesWon();
            }
            
            ExtractData(step);
        }

        private void ExtractData(TurnStep turnStep)
        {
            foreach (var extractor in _dataExtractors)
            {
                if (extractor.CanExtractDataFrom(turnStep))
                {
                    var data = extractor.ExtractData(turnStep);
                    _currentGameData.LastTurnData.AggregateData(data);
                    _currentGameData.AllTurnsData.AggregateData(data);
                }
            }
        }
    }
}
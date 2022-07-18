﻿using System.Collections.Generic;
using Match3.Core.GameDataExtraction;
using Match3.Core.GameEndConditions.Defeat;
using Match3.Core.GameEndConditions.Victory;
using Match3.View.GameEndConditions.Defeat;
using Match3.View.GameEndConditions.Victory;
using UnityEngine;
using Utils.Extensions;

namespace Match3.View.GameEndConditions
{
    public class GameEndConditionsView : MonoBehaviour
    {
        [SerializeField] private Transform _victoryConditionContainer;
        [SerializeField] private Transform _defeatConditionContainer;

        [SerializeField] private List<VictoryConditionView> _victoryViewsPrefabs;
        [SerializeField] private List<DefeatConditionView> _defeatViewsPrefabs;

        private VictoryConditionView _currentVictoryView;
        private DefeatConditionView _currentDefeatView;

        public void SetupUi(IVictoryEvaluator victoryEvaluator, IDefeatEvaluator defeatEvaluator)
        {
            Clear();
            var victoryPrefab = _victoryViewsPrefabs.Find(vvp => vvp.CanHandle(victoryEvaluator));
            var defeatPrefab = _defeatViewsPrefabs.Find(dvp => dvp.CanHandle(defeatEvaluator));

            _currentVictoryView = Instantiate(victoryPrefab, _victoryConditionContainer);
            _currentDefeatView = Instantiate(defeatPrefab, _defeatConditionContainer);

            _currentVictoryView.SetupUi(victoryEvaluator);
            _currentDefeatView.SetupUi(defeatEvaluator);
        }
        
        public void UpdateUi(IVictoryEvaluator victoryEvaluator, IDefeatEvaluator defeatEvaluator, GameData gameData)
        {
            _currentVictoryView.UpdateUi(victoryEvaluator, gameData);
            _currentDefeatView.UpdateUi(defeatEvaluator, gameData);
        }

        private void Clear()
        {
            _currentVictoryView = null;
            _currentDefeatView = null;
            _victoryConditionContainer.ClearChildren();
            _defeatConditionContainer.ClearChildren();
        }
    }
}
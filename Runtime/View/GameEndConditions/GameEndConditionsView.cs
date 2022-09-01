using System.Collections.Generic;
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
        [SerializeField] private Transform _container;

        [SerializeField] private List<VictoryConditionView> _victoryViewsPrefabs;
        [SerializeField] private List<DefeatConditionView> _defeatViewsPrefabs;

        private VictoryConditionView _currentVictoryView;
        private DefeatConditionView _currentDefeatView;

        public void SetupUi(IVictoryEvaluator victoryEvaluator, IDefeatEvaluator defeatEvaluator)
        {
            Clear();
            var victoryPrefab = _victoryViewsPrefabs.Find(vvp => vvp.CanHandle(victoryEvaluator));
            var defeatPrefab = _defeatViewsPrefabs.Find(dvp => dvp.CanHandle(defeatEvaluator));
            
            _currentVictoryView = Instantiate(victoryPrefab, _container);
            if (victoryPrefab.gameObject == defeatPrefab.gameObject)  // is the same prefab?
            {
                // get from the same object
                var defeatEvaluatorType = defeatPrefab.GetType();
                _currentDefeatView = (DefeatConditionView) _currentVictoryView.GetComponent(defeatEvaluatorType);
            }
            else
            {
                _currentDefeatView = Instantiate(defeatPrefab, _container);
            }

            _currentVictoryView.SetupUi(victoryEvaluator);
            _currentDefeatView.SetupUi(defeatEvaluator);
        }
        
        public void UpdateUi(IVictoryEvaluator victoryEvaluator, IDefeatEvaluator defeatEvaluator)
        {
            _currentVictoryView.UpdateUi(victoryEvaluator);
            _currentDefeatView.UpdateUi(defeatEvaluator);
        }

        private void Clear()
        {
            _currentVictoryView = null;
            _currentDefeatView = null;
            _container.ClearChildren();
        }
    }
}
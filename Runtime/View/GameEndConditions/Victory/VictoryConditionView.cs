using Match3.Core.GameDataExtraction;
using Match3.Core.GameEndConditions.Victory;
using UnityEngine;

namespace Match3.View.GameEndConditions.Victory
{
    public abstract class VictoryConditionView : MonoBehaviour
    {
        public abstract bool CanHandle(IVictoryEvaluator victoryEvaluator);
        public abstract void SetupUi(IVictoryEvaluator victoryEvaluator);
        public abstract void UpdateUi(IVictoryEvaluator victoryEvaluator);
    }
    
    public abstract class VictoryConditionView<T> : VictoryConditionView where T : IVictoryEvaluator
    {
        public override bool CanHandle(IVictoryEvaluator victoryEvaluator)
        {
            return victoryEvaluator is T;
        }
        
        public override void SetupUi(IVictoryEvaluator victoryEvaluator)
        {
            SetupUi((T) victoryEvaluator);
        }
        
        public override void UpdateUi(IVictoryEvaluator victoryEvaluator)
        {
            UpdateUi((T) victoryEvaluator);
        }
        
        protected abstract void SetupUi(T victoryEvaluator);
        protected abstract void UpdateUi(T victoryEvaluator);
    }
}
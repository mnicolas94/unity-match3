using Match3.Core.GameDataExtraction;
using Match3.Core.GameEndConditions.Defeat;
using UnityEngine;

namespace Match3.View.GameEndConditions.Defeat
{
    public abstract class DefeatConditionView : MonoBehaviour
    {
        public abstract bool CanHandle(IDefeatEvaluator defeatEvaluator);
        public abstract void SetupUi(IDefeatEvaluator defeatEvaluator);
        public abstract void UpdateUi(IDefeatEvaluator defeatEvaluator);
    }
    
    public abstract class DefeatConditionView<T> : DefeatConditionView where T : IDefeatEvaluator
    {
        public override bool CanHandle(IDefeatEvaluator defeatEvaluator)
        {
            return defeatEvaluator is T;
        }
        
        public override void SetupUi(IDefeatEvaluator defeatEvaluator)
        {
            SetupUi((T) defeatEvaluator);
        }
        
        public override void UpdateUi(IDefeatEvaluator defeatEvaluator)
        {
            UpdateUi((T) defeatEvaluator);
        }
        
        protected abstract void SetupUi(T defeatEvaluator);
        protected abstract void UpdateUi(T defeatEvaluator);
    }
}
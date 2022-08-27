using Match3.Core.GameActions;
using ModelView;
using UnityEngine;
using UnityEngine.Events;

namespace Match3.View.SkillCostViews
{
    public abstract class SkillCostViewBase : ViewBaseBehaviour<Skill>
    {
        [SerializeField] private UnityEvent _onCantApplyCost;

        public void OnCantApplyCost()
        {
            _onCantApplyCost?.Invoke();
            OnCantApplyCostInternal();
        }
        
        protected virtual void OnCantApplyCostInternal(){}
    }
}
using Match3.Core.GameActions;
using ModelView;
using UnityEngine;
using UnityEngine.Events;

namespace Match3.View.SkillCostViews
{
    public abstract class SkillCostViewBase : ViewBaseBehaviour<Skill>
    {
        [SerializeField] private UnityEvent<Skill> _onCantApplyCost;

        public void OnCantApplyCost(Skill skill)
        {
            _onCantApplyCost?.Invoke(skill);
            OnCantApplyCostInternal();
        }
        
        protected virtual void OnCantApplyCostInternal(){}
    }
}
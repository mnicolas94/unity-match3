using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting.APIUpdating;

namespace Match3.Core.GameActions
{
    [MovedFrom(false, "BoardCores.Data.GameActions.Actions")]
    [Serializable]
    public class SkillCostCount : SkillCostBase
    {
        [SerializeField] private UnityEvent<Skill> _onApplyCost;
        
        public override bool CanExecuteSkill(Skill skill)
        {
            return SkillsCountStorage.GetSkillCount(skill) > 0;
        }

        public override void ApplySkillCost(Skill skill)
        {
            bool success = SkillsCountStorage.ConsumeSkill(skill);
            if (success)
            {
                _onApplyCost.Invoke(skill);
            }
        }

        public int GetRemainingCount(Skill skill)
        {
            return SkillsCountStorage.GetSkillCount(skill);
        }
    }
}
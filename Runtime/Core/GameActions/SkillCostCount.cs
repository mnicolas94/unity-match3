using System;
using Codice.Client.BaseCommands;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting.APIUpdating;
using Utils.Attributes;

namespace Match3.Core.GameActions
{
    [MovedFrom(false, "BoardCores.Data.GameActions.Actions")]
    [Serializable]
    public class SkillCostCount : SkillCostBase
    {
        [SerializeField] private UnityEvent<Skill> _onApplyCost;

        [SerializeField, AutoProperty(AutoPropertyMode.Asset)]
        private SkillsCountStorage _storage;
        
        public override bool CanExecuteSkill(Skill skill)
        {
            return _storage.GetSkillCount(skill) > 0;
        }

        public override void ApplySkillCost(Skill skill)
        {
            bool success = _storage.ConsumeSkill(skill);
            if (success)
            {
                _onApplyCost.Invoke(skill);
            }
        }

        public int GetRemainingCount(Skill skill)
        {
            return _storage.GetSkillCount(skill);
        }
    }
}
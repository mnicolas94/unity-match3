using System;

namespace Match3.Core.GameActions
{
    [Serializable]
    public class SkillCostFree : SkillCostBase
    {
        public override bool CanExecuteSkill(Skill skill)
        {
            return true;
        }

        public override void ApplySkillCost(Skill skill)
        {
            // nothing
        }
    }
}
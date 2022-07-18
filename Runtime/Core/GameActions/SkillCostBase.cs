namespace Match3.Core.GameActions
{
    public abstract class SkillCostBase
    {
        public abstract bool CanExecuteSkill(Skill skill);
        public abstract void ApplySkillCost(Skill skill);
    }
}
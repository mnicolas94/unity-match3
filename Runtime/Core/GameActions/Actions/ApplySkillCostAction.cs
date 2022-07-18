using System;
using Match3.Core.GameActions.Interactions;
using Match3.Core.Gravity;

namespace Match3.Core.GameActions.Actions
{
    public class ApplySkillCostAction : IGameAction
    {
        public Action<ApplySkillCostAction> OnCostApplied;
        
        private readonly IGameAction _delegateAction;
        private readonly Skill _skill;
        
        public ApplySkillCostAction(IGameAction delegateAction, Skill skill)
        {
            _delegateAction = delegateAction;
            _skill = skill;
        }

        public bool CanExecuteFromInteractionType(Type interactionType)
        {
            return _delegateAction.CanExecuteFromInteractionType(interactionType);
        }

        public bool IsInteractionValid(Board board, IInteraction interaction)
        {
            return _delegateAction.IsInteractionValid(board, interaction);
        }

        public GameActionExecution Execute(GameContext context, Board board, IInteraction interaction)
        {
            _skill.SkillCost.ApplySkillCost(_skill);
            OnCostApplied?.Invoke(this);
            return _delegateAction.Execute(context, board, interaction);
        }
    }
}
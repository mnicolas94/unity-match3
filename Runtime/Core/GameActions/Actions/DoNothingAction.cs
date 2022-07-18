using System;
using System.Collections.Generic;
using Match3.Core.GameActions.Interactions;
using Match3.Core.Gravity;
using Match3.Core.TurnSteps;

namespace Match3.Core.GameActions.Actions
{
    public class DoNothingAction : IGameAction
    {
        public bool CanExecuteFromInteractionType(Type interactionType)
        {
            return true;
        }

        public bool IsInteractionValid(Board board, IInteraction interaction)
        {
            return true;
        }

        public GameActionExecution Execute(GameContext context, Board board, IInteraction interaction)
        {
            return new GameActionExecution(new List<TurnStep>(), false);
        }
    }
}
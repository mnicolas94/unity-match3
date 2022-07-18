using System;
using System.Collections.Generic;
using Match3.Core.GameActions.Interactions;
using Match3.Core.Gravity;
using Match3.Core.TurnSteps;

namespace Match3.Core.GameActions.Actions
{
    public interface IGameAction
    {
        bool CanExecuteFromInteractionType(Type interactionType);
        bool IsInteractionValid(Board board, IInteraction interaction);
        GameActionExecution Execute(GameContext context, Board board, IInteraction interaction);
    }
    
    public abstract class GameActionBase<T> : IGameAction where T : IInteraction
    {
        public abstract bool IsInteractionValid(Board board, T interaction);
        public abstract GameActionExecution Execute(GameContext context, Board board, T interaction);

        private Type GetInteractionType()
        {
            return typeof(T);
        }

        public bool CanExecuteFromInteractionType(Type interactionType)
        {
            return typeof(T).IsAssignableFrom(interactionType);
        }

        public bool IsInteractionValid(Board board, IInteraction interaction)
        {
            var type = GetInteractionType();
            if (interaction.GetType().IsAssignableFrom(type))
            {
                return IsInteractionValid(board, (T) interaction);
            }

            return false;
        }

        public GameActionExecution Execute(GameContext context, Board board, IInteraction interaction)
        {
            return Execute(context, board, (T) interaction);
        }
    }

    public struct GameActionExecution
    {
        private readonly IEnumerable<TurnStep> _turnSteps;
        private readonly bool _countAsTurn;

        public IEnumerable<TurnStep> TurnSteps => _turnSteps;

        public bool CountAsTurn => _countAsTurn;

        public GameActionExecution(IEnumerable<TurnStep> turnSteps, bool countAsTurn)
        {
            _turnSteps = turnSteps;
            _countAsTurn = countAsTurn;
        }
    }
}
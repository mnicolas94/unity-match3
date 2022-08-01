using System.Collections.Generic;
using Match3.Core.GameActions.Interactions;
using Match3.Core.Gravity;
using Match3.Core.TokensEvents.Events;
using Match3.Core.TokensEvents.Outputs;

namespace Match3.Core.GameActions.Actions
{
    public class DoubleClickGameAction : GameActionBase<DoubleClickInteraction>
    {
        public override bool IsInteractionValid(Board board, DoubleClickInteraction interaction)
        {
            var position = interaction.Position;
            bool exists = board.ExistsTokenAnyLayerAt(position);
            if (exists)
            {
                var (token, layer) = board.GetTopTokenAt(position);
                bool hasResolver = token.TokenData.Resolvers.HasResolvers<EventDoubleClicked>();
                return hasResolver;
            }
            
            return false;
        }

        public override GameActionExecution Execute(GameContext context, Board board, DoubleClickInteraction interaction)
        {
            // send double clicked event
            var position = interaction.Position;
            var (token, layer) = board.GetTopTokenAt(position);
            var outputs = new List<TokenEventOutput>();
            var @event = new EventDoubleClicked(board, token, position);
            SendEventUtils.SendEvent(context, token, @event, outputs);
            
            var turnSteps = BoardGameActions.Gts_HandleEventOutputs(context, board, outputs);
            var execution = new GameActionExecution(turnSteps, true);
            return execution;
        }
    }
}
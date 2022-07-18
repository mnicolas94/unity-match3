using System;
using System.Collections.Generic;
using Match3.Core.GameActions.Interactions;
using Match3.Core.Gravity;
using Match3.Core.TokensEvents.Events;
using Match3.Core.TokensEvents.Resolvers;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Match3.Core.GameActions.Actions
{
    [MovedFrom(false, "BoardCores.Data.GameActions.Actions")]
    [Serializable]
    public class OutputEventsAction : GameActionBase<SelectPositionInteraction>
    {
        [SerializeReference, SubclassSelector] public List<IEventResolver> _resolvers;

        public override bool IsInteractionValid(Board board, SelectPositionInteraction interaction)
        {
            bool exists = board.ExistsTokenAnyLayerAt(interaction.Position);
            return exists;
        }

        public override GameActionExecution Execute(GameContext context, Board board, SelectPositionInteraction interaction)
        {
            var position = interaction.Position;
            var (token, layer) = board.GetTopTokenAt(position);
            var @event = new TokenEventInput(board, token, position);
            var outputs = _resolvers.ConvertAll(resolver => resolver.OnEvent(@event));
            var turnSteps = BoardGameActions.Gts_HandleEventOutputs(context, board, outputs);
            var execution = new GameActionExecution(turnSteps, countAsTurn: false);
            return execution;
        }
    }
}
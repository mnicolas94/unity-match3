using System;
using System.Collections.Generic;
using Match3.Core.GameActions.TokensDamage;
using Match3.Core.SerializableTuples;
using Match3.Core.TokensEvents.Events;
using Match3.Core.TokensEvents.Outputs;
using Match3.Core.TurnSteps;
using UnityEngine.Scripting.APIUpdating;

namespace Match3.Core.TokensEvents.Resolvers
{
    [Serializable]
    public class DestroySelfResolver : IEventResolver
    {
        public TokenEventOutput OnEvent(TokenEventInput @event)
        {
            var position = @event.Position;
            var token = @event.Token;
            var positionToken = new PositionTokenDamageOrder(position, token, 0, new DamageInfo(0));
            var positionsToDestroy = new List<PositionTokenDamageOrder> { positionToken };
            var destruction = new TokensDamaged(null, position, positionsToDestroy);
            return new DestroyTokensEventOutput(destruction);
        }
    }
}
using System;
using System.Collections.Generic;
using Match3.Core.SerializableTuples;
using Match3.Core.TokensEvents.Events;
using Match3.Core.TokensEvents.Outputs;
using Match3.Core.TurnSteps.TokensDestructionSource;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Match3.Core.TokensEvents.Resolvers
{
    [Serializable]
    public class DamageSelfResolver : IEventResolver
    {
        [SerializeReference, SubclassSelector] private ITokenDestructionSource _damageSource;
        
        public TokenEventOutput OnEvent(TokenEventInput @event)
        {
            var position = @event.Position;
            var positionDamageOrder = new PositionDamageOrder(position, 0);
            return new DamagePositionsEventOutput(
                _damageSource,
                position,
                new List<PositionDamageOrder>{ positionDamageOrder });
        }
    }
}
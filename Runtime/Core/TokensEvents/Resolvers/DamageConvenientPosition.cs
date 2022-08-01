using System;
using System.Collections.Generic;
using Match3.Core.SerializableTuples;
using Match3.Core.TokensEvents.Events;
using Match3.Core.TokensEvents.Outputs;
using Match3.Core.TurnSteps.TokensDestructionSource;
using UnityEngine;
using Utils.Extensions;

namespace Match3.Core.TokensEvents.Resolvers
{
    [Serializable]
    public class DamageConvenientPosition : IEventResolver
    {
        [SerializeReference, SubclassSelector] private ITokenDestructionSource _damageSource;
        
        public TokenEventOutput OnEvent(TokenEventInput @event)
        {
            var position = GetConvenientPosition(@event.Board);
            return new DamagePositionsEventOutput(
                _damageSource,
                position,
                new List<PositionDamageOrder>
                {
                    new PositionDamageOrder(@event.Position, 0),
                    new PositionDamageOrder(position, 1)
                });
        }

        private Vector2Int GetConvenientPosition(Board board)
        {
            // TODO implementar cálculo de posiciones "convenientes"
            var positions = board.MainLayer.GetPositions();
            if (positions.Count > 0)
                return positions.GetRandom();
            return Vector2Int.zero;
        }
    }
}
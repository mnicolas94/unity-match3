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
        [SerializeField] private int _times;
        [SerializeReference, SubclassSelector] private ITokenDamageSource _damageSource;
        
        public TokenEventOutput OnEvent(TokenEventInput @event)
        {
            var positionsToDamage = new List<PositionToAttackOrder>
            {
                new PositionToAttackOrder(@event.Position, 0),
            };
            for (int i = 0; i < _times; i++)
            {
                var position = GetConvenientPosition(@event.Board);
                positionsToDamage.Add(new PositionToAttackOrder(position, 1));
            }
            
            return new DamagePositionsEventOutput(
                _damageSource,
                @event.Position,
                positionsToDamage);
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
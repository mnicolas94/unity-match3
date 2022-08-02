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
    public class DamageAllInDirectionsResolver : IEventResolver
    {
        [SerializeReference, SubclassSelector] private ITokenDamageSource _damageSource;
        [SerializeField] private List<Vector2Int> _directions;

        private DamagePositionsEventOutput GetOutput(Board board, Vector2Int position)
        {
            var bounds = board.BoardShape.GetBounds();
            var positionsToDamage = new List<PositionToAttackOrder>();
            foreach (var direction in _directions)
            {
                int damageOrder = 0;
                var positionToDamage = position + direction;
                while (bounds.Contains(positionToDamage))
                {
                    positionsToDamage.Add(new PositionToAttackOrder(positionToDamage, damageOrder));
                    positionToDamage += direction;
                    damageOrder++;
                }
            }
            
            return new DamagePositionsEventOutput(_damageSource, position, positionsToDamage);
        }
        
        public TokenEventOutput OnEvent(TokenEventInput @event)
        {
            return GetOutput(@event.Board, @event.Position);
        }
    }
}
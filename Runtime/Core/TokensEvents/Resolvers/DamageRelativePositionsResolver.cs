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
    public class DamageRelativePositionsResolver : IEventResolver
    {
        [SerializeReference, SubclassSelector] private ITokenDamageSource _damageSource;
        [SerializeField] private List<Vector2Int> _relativePositions;

        private DamagePositionsEventOutput DamageRelativesPositions(Vector2Int position)
        {
            var positionsToDamage = _relativePositions.ConvertAll(relativePosition =>
            {
                var relPosition = position + relativePosition;
                return new PositionToAttackOrder(relPosition, 0);
            });
            return new DamagePositionsEventOutput(_damageSource, position, positionsToDamage);
        }

        public TokenEventOutput OnEvent(TokenEventInput @event)
        {
            return DamageRelativesPositions(@event.Position);
        }
    }
}
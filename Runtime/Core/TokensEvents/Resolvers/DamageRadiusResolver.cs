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
    public class DamageRadiusResolver : IEventResolver
    {
        [SerializeReference, SubclassSelector] private ITokenDestructionSource _damageSource;
        [SerializeField] [Min(1)] private int radius;
        
        private DamagePositionsEventOutput DamageRadius(Vector2Int position)
        {
            int xMin = position.x - radius;
            int xMax = position.x + radius;
            int yMin = position.y - radius;
            int yMax = position.y + radius;
            
            var positionsToDamage = new List<PositionDamageOrder>();
            for (int x = xMin; x <= xMax; x++)
            for (int y = yMin; y <= yMax; y++)
            {
                var currentPosition = new Vector2Int(x, y);
                int xRadius = Math.Abs(x - xMin - radius);
                int yRadius = Math.Abs(y - yMin - radius);
                int currentRadius = Math.Max(xRadius, yRadius);
                positionsToDamage.Add(new PositionDamageOrder(currentPosition, currentRadius));
            }
                    
            return new DamagePositionsEventOutput(_damageSource, position, positionsToDamage);
        }
        
        public TokenEventOutput OnEvent(TokenEventInput @event)
        {
            return DamageRadius(@event.Position);
        }
    }
}
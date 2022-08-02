using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Match3.Core.SerializableTuples;
using Match3.Core.TurnSteps.TokensDestructionSource;
using UnityEngine;

namespace Match3.Core.TokensEvents.Outputs
{
    [Serializable]
    public class DamagePositionsEventOutput : TokenEventOutput
    {
        [SerializeField] private ITokenDamageSource _damageSource;
        [SerializeField] private Vector2Int _sourcePosition;
        [SerializeField] private List<PositionToAttackOrder> _positionsToDamage;

        public ITokenDamageSource DamageSource => _damageSource;

        public Vector2Int SourcePosition => _sourcePosition;

        public ReadOnlyCollection<PositionToAttackOrder> PositionsToDamage => _positionsToDamage.AsReadOnly();

        public DamagePositionsEventOutput(ITokenDamageSource damageSource, Vector2Int sourcePosition, List<PositionToAttackOrder> positionsToDamage)
        {
            _damageSource = damageSource;
            _sourcePosition = sourcePosition;
            _positionsToDamage = positionsToDamage;
        }
    }
}
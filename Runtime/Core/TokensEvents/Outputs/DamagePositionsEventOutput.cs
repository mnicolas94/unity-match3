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
        [SerializeField] private ITokenDestructionSource _damageSource;
        [SerializeField] private Vector2Int _sourcePosition;
        [SerializeField] private List<PositionDamageOrder> _positionsToDamage;

        public ITokenDestructionSource DamageSource => _damageSource;

        public Vector2Int SourcePosition => _sourcePosition;

        public ReadOnlyCollection<PositionDamageOrder> PositionsToDamage => _positionsToDamage.AsReadOnly();

        public DamagePositionsEventOutput(ITokenDestructionSource damageSource, Vector2Int sourcePosition, List<PositionDamageOrder> positionsToDamage)
        {
            _damageSource = damageSource;
            _sourcePosition = sourcePosition;
            _positionsToDamage = positionsToDamage;
        }
    }
}
using System;
using UnityEngine;

namespace Match3.Core.SerializableTuples
{
    [Serializable]
    public class PositionToAttackOrder
    {
        [SerializeField] private Vector2Int _position;
        [SerializeField] private int _damageOrder;  // TODO remove this
        // TODO attack info. e.g.: attack type (fire, ice, etc)
        
        public Vector2Int Position => _position;

        public int DamageOrder => _damageOrder;

        public PositionToAttackOrder(Vector2Int position, int damageOrder)
        {
            _position = position;
            _damageOrder = damageOrder;
        }

        public void Deconstruct(out Vector2Int position, out int damageOrder)
        {
            position = _position;
            damageOrder = _damageOrder;
        }
    }
}
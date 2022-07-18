using System;
using UnityEngine;

namespace Match3.Core.SerializableTuples
{
    [Serializable]
    public class PositionDamageOrder
    {
        [SerializeField] private Vector2Int _position;
        [SerializeField] private int _damageOrder;

        public Vector2Int Position => _position;

        public int DamageOrder => _damageOrder;

        public PositionDamageOrder(Vector2Int position, int damageOrder)
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
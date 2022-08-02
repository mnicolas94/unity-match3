using System;
using Match3.Core.GameActions.TokensDamage;
using UnityEngine;

namespace Match3.Core.SerializableTuples
{
    [Serializable]
    public class PositionTokenDamageOrder
    {
        [SerializeField] private Vector2Int _position;
        [SerializeField] private Token _token;
        [SerializeField] private int _sortOrder;
        [SerializeField] private DamageInfo _damage;

        public Vector2Int Position => _position;

        public Token Token => _token;

        public int SortOrder => _sortOrder;
        
        public DamageInfo Damage => _damage;

        public PositionToken PositionToken => new PositionToken(_position, _token);
        
        public PositionTokenDamageOrder(Vector2Int position, Token token, int sortOrder, DamageInfo damage)
        {
            _position = position;
            _token = token;
            _sortOrder = sortOrder;
            _damage = damage;
        }
        
        public void Deconstruct(out Vector2Int position, out Token token, out int sortOrder, out DamageInfo damage)
        {
            position = _position;
            token = _token;
            sortOrder = _sortOrder;
            damage = _damage;
        }
        
        public void Deconstruct(out PositionToken positionToken, out int sortOrder, out DamageInfo damage)
        {
            positionToken = PositionToken;
            sortOrder = _sortOrder;
            damage = _damage;
        }
    }
}
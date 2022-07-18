using System;
using UnityEngine;

namespace Match3.Core.SerializableTuples
{
    [Serializable]
    public class PositionTokenDestructionOrder
    {
        [SerializeField] private Vector2Int _position;
        [SerializeField] private Token _token;
        [SerializeField] private int _sortOrder;

        public Vector2Int Position => _position;

        public Token Token => _token;

        public int SortOrder => _sortOrder;

        public PositionToken PositionToken => new PositionToken(_position, _token);
        
        public PositionTokenDestructionOrder(Vector2Int position, Token token, int sortOrder)
        {
            _position = position;
            _token = token;
            _sortOrder = sortOrder;
        }
        
        public void Deconstruct(out Vector2Int position, out Token token, out int sortOrder)
        {
            position = _position;
            token = _token;
            sortOrder = _sortOrder;
        }
        
        public void Deconstruct(out PositionToken positionToken, out int sortOrder)
        {
            positionToken = PositionToken;
            sortOrder = _sortOrder;
        }
    }
}
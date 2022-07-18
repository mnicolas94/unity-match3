using System;
using UnityEngine;

namespace Match3.Core.TokensEvents.Events
{
    [Serializable]
    public class TokenEventInput
    {
        [SerializeField] private Board _board;
        [SerializeField] private Token _token;
        [SerializeField] private Vector2Int _position;

        public Board Board => _board;
        public Token Token => _token;
        public Vector2Int Position => _position;

        public TokenEventInput(Board board, Token token, Vector2Int position)
        {
            _board = board;
            _token = token;
            _position = position;
        }
    }
}
using System;
using UnityEngine;

namespace Match3.Core.TokensEvents.Events
{
    [Serializable]
    public class EventSwapped : TokenEventInput
    {
        [SerializeField] private Token _other;
        [SerializeField] private Vector2Int _otherPosition;

        public EventSwapped(Board board, Token token, Vector2Int position, Token other, Vector2Int otherPosition)
            : base(board, token, position)
        {
            _other = other;
            _otherPosition = otherPosition;
        }
    }
}
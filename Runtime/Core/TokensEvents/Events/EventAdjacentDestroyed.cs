using System;
using UnityEngine;

namespace Match3.Core.TokensEvents.Events
{
    [Serializable]
    public class EventAdjacentDestroyed : TokenEventInput
    {
        [SerializeField] private Vector2Int _adjacentPosition;

        public Vector2Int AdjacentPosition => _adjacentPosition;

        public EventAdjacentDestroyed(Board board, Token token, Vector2Int position, Vector2Int adjacentPosition)
            : base(board, token, position)
        {
            _adjacentPosition = adjacentPosition;
        }
    }
}
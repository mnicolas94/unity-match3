using System;
using UnityEngine;

namespace Match3.Core.TokensEvents.Events
{
    [Serializable]
    public class EventDestroyed : TokenEventInput
    {
        public EventDestroyed(Board board, Token token, Vector2Int position) : base(board, token, position)
        {
        }
    }
}
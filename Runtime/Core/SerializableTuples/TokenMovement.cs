using System;
using UnityEngine;

namespace Match3.Core.SerializableTuples
{
    [Serializable]
    public class TokenMovement
    {
        [SerializeField] private Token token;
        [SerializeField] private Vector2Int fromPosition;
        [SerializeField] private Vector2Int toPosition;

        public Token Token
        {
            get => token;
            set => token = value;
        }

        public Vector2Int FromPosition
        {
            get => fromPosition;
            set => fromPosition = value;
        }

        public Vector2Int ToPosition
        {
            get => toPosition;
            set => toPosition = value;
        }

        public TokenMovement(Token token, Vector2Int fromPosition, Vector2Int toPosition)
        {
            this.token = token;
            this.fromPosition = fromPosition;
            this.toPosition = toPosition;
        }

        public void Deconstruct(out Token token, out Vector2Int from, out Vector2Int to)
        {
            token = this.token;
            from = fromPosition;
            to = toPosition;
        }
    }
}
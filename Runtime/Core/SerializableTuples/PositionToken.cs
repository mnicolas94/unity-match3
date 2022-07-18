using System;
using UnityEngine;

namespace Match3.Core.SerializableTuples
{
    [Serializable]
    public class PositionToken
    {
        [SerializeField] public Vector2Int position;
        [SerializeField] public Token token;

        public Vector2Int Position => position;

        public Token Token => token;

        public PositionToken(Vector2Int position, Token token)
        {
            this.position = position;
            this.token = token;    
        }

        public void Deconstruct(out Vector2Int position, out Token token)
        {
            position = this.position;
            token = this.token;
        }
    }
}
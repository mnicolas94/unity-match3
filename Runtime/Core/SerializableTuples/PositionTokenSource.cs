using System;
using UnityEngine;

namespace Match3.Core.SerializableTuples
{
    [Serializable]
    public class PositionTokenSource
    {
        public Vector2Int position;
        public TokenSource tokenSource;
        
        public void Deconstruct(out Vector2Int position, out TokenSource tokenSource)
        {
            position = this.position;
            tokenSource = this.tokenSource;
        }

        public override string ToString()
        {
            return $"[{position.x} ; {position.y}]";
        }
    }
}
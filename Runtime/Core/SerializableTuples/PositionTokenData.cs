using System;
using UnityEngine;

namespace Match3.Core.SerializableTuples
{
    [Serializable]
    public class PositionTokenData
    {
        public Vector2Int position;
        public TokenData tokenData;

        public void Deconstruct(out Vector2Int position, out TokenData tokenData)
        {
            position = this.position;
            tokenData = this.tokenData;
        }

        public override string ToString()
        {
            return $"[{position.x} ; {position.y}] {(tokenData == null ? "null" : tokenData.name)}";
        }
    }
}
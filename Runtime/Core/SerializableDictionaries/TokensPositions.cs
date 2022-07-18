using System;
using UnityEngine;
using Utils.Serializables;

namespace Match3.Core.SerializableDictionaries
{
    [Serializable]
    public class TokensPositions : SerializableDictionary<Vector2Int, Token>
    {
    }
}
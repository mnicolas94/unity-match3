using System;
using UnityEngine;
using Utils.Serializables;

namespace Match3.Core.SerializableDictionaries
{
    [Serializable]
    public class SourcesPositions : SerializableDictionary<Vector2Int, TokenSource>
    {
    }
}
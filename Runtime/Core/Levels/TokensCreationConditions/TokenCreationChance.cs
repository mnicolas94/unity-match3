using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3.Core.Levels.TokensCreationConditions
{
    [Serializable]
    public class TokenCreationChance : ITokenCreationPredicate
    {
        [SerializeField, Range(0.0f, 1.0f)] private float _chance;
        
        public bool IsMet((GameController, TokenSource, Vector2Int) input)
        {
            return Random.value <= _chance;
        }
    }
}
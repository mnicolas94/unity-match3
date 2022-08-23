using System;
using UnityEngine;
using Utils.Serializables;

namespace Match3.Core.Levels.TokensCreationConditions
{
    public interface ITokenCreationPredicate : ISerializablePredicate<(GameController, TokenSource, Vector2Int)>
    {
    }
    
    [Serializable]
    public class TokenCreationPredicateAlways : ITokenCreationPredicate
    {
        public bool IsMet((GameController, TokenSource, Vector2Int) input)
        {
            return true;
        }

        public override string ToString()
        {
            return "Always";
        }
    }
    
    [Serializable]
    public class TokenCreationPredicateNever : ITokenCreationPredicate
    {
        public bool IsMet((GameController, TokenSource, Vector2Int) input)
        {
            return false;
        }
        
        public override string ToString()
        {
            return "Never";
        }
    }
}
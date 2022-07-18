using System;
using System.Collections.Generic;
using Match3.Core.Levels.TokensCreationConditions;
using UnityEngine;

namespace Match3.Core.Levels
{
    [Serializable]
    public abstract class TokenCreationCondition
    {
        [SerializeReference, SubclassSelector] protected ITokenCreationPredicate _predicate;

        public bool IsMet(GameController controller, TokenSource tokenSource, Vector2Int position)
        {
            return _predicate.IsMet((controller, tokenSource, position));
        }
        
        public abstract TokenData CreateToken(GameController controller, TokenSource tokenSource, Vector2Int position);
        public abstract IEnumerable<TokenData> GetAvailableTokens();
    }
}
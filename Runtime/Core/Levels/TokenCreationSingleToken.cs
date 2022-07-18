using System;
using System.Collections.Generic;
using Match3.Core.Levels.TokensCreationConditions;
using Match3.Settings;
using UnityEngine;

namespace Match3.Core.Levels
{
    [Serializable]
    public class TokenCreationSingleToken : TokenCreationCondition, IHomologousTokenReplacer
    {
        [SerializeField] private TokenData _tokenData;

        public static TokenCreationSingleToken Create(TokenData tokenData, ITokenCreationPredicate predicate)
        {
            return new TokenCreationSingleToken
            {
                _predicate = predicate,
                _tokenData = tokenData
            };
        }
        
        public override TokenData CreateToken(GameController controller, TokenSource tokenSource, Vector2Int position)
        {
            return _tokenData;
        }

        public override IEnumerable<TokenData> GetAvailableTokens()
        {
            yield return _tokenData;
        }

        public void ReplaceToken(TokenData toReplace, TokenData replacement)
        {
            if (_tokenData == toReplace)
                _tokenData = replacement;
        }
    }
}
using System;
using System.Collections.Generic;
using Match3.Settings;
using UnityEngine;

namespace Match3.Core.Levels.TokenProviders
{
    [Serializable]
    public class TokenProviderSingleToken : ITokenCreationProvider, IHomologousTokenReplacer
    {
        [SerializeField] private TokenData _tokenData;

        public TokenData GetToken((GameController controller, TokenSource tokenSource, Vector2Int position) context)
        {
            return _tokenData;
        }

        public IEnumerable<TokenData> GetAvailableTokens()
        {
            yield return _tokenData;
        }

        public void ReplaceToken(TokenData toReplace, TokenData replacement)
        {
            if (_tokenData == toReplace)
                _tokenData = replacement;
        }

        public override string ToString()
        {
            return _tokenData == null ? "?" : _tokenData.name;
        }
    }
}
using System;
using UnityEngine;

namespace Match3.Core.Matches.TokenProviders
{
    [Serializable]
    public class TokenProvider : ITokenProvider
    {
        [SerializeField] private TokenData _tokenData;
        
        public TokenData GetToken()
        {
            return _tokenData;
        }

        public override string ToString()
        {
            return $"{_tokenData.name}";
        }
    }
}
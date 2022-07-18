using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Extensions;

namespace Match3.Core.Matches.TokenProviders
{
    [Serializable]
    public class RandomTokenProvider : ITokenProvider
    {
        [SerializeField] private List<TokenData> _tokens;
        
        public TokenData GetToken()
        {
            return _tokens.GetRandom();
        }
    }
}
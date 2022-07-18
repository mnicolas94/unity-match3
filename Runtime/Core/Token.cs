using System;
using UnityEngine;

namespace Match3.Core
{
    [Serializable]
    public class Token
    {
        [SerializeField] private TokenData _tokenData;

        public TokenData TokenData => _tokenData;

        public Token(TokenData tokenData)
        {
            _tokenData = tokenData;
        }

        public override string ToString()
        {
            return $"I: {_tokenData}";
        }
    }

    [Serializable]
    public class FakeToken : Token
    {
        public FakeToken() : base(null)
        {
        }
    }
}
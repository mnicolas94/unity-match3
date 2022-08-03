using System;
using UnityEngine;

namespace Match3.Core
{
    [Serializable]
    public class Token
    {
        [SerializeField] private TokenData _tokenData;
        [SerializeField] private int _healthPoints;

        public TokenData TokenData => _tokenData;

        public int HealthPoints => _healthPoints;

        public Token(TokenData tokenData)
        {
            _tokenData = tokenData;
            _healthPoints = tokenData == null ? 1 : tokenData.InitialHealth;
        }
        
        public int ApplyDamage(int damage)
        {
            if (_tokenData.IsIndestructible)
                return 0;

            int damageDone = Math.Min(_healthPoints, damage);
            _healthPoints -= damageDone;
            return damageDone;
        }

        public override string ToString()
        {
            return $"I: {_tokenData}";
        }
    }

    [Serializable]
    public class FakeToken : Token
    {
        public FakeToken() : base(ScriptableObject.CreateInstance<TokenData>())
        {
        }
    }
}
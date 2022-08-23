using System;
using System.Collections.Generic;
using Match3.Core.Levels.TokenProviders;
using Match3.Core.Levels.TokensCreationConditions;
using Match3.Settings;
using TNRD;
using UnityEngine;

namespace Match3.Core.Levels
{
    [Serializable]
    public class ConditionalTokenCreation : IHomologousTokenReplacer
    {
        [SerializeField] private SerializableInterface<ITokenCreationPredicate> _predicate;
        [SerializeField] private SerializableInterface<ITokenCreationProvider> _provider;
        [SerializeField] private bool _isPermanent;

        public bool IsPermanent => _isPermanent;

        public bool IsMet(GameController controller, TokenSource tokenSource, Vector2Int position)
        {
            return _predicate.Value.IsMet((controller, tokenSource, position));
        }
        
        public TokenData GetToken(GameController controller, TokenSource tokenSource, Vector2Int position)
        {
            var context = (controller, tokenSource, position);
            return _provider.Value.GetToken(context);
        }
        
        public IEnumerable<TokenData> GetAvailableTokens()
        {
            return _provider.Value.GetAvailableTokens();
        }

        public void ReplaceToken(TokenData toReplace, TokenData replacement)
        {
            if (_provider.Value is IHomologousTokenReplacer replacer)
            {
                replacer.ReplaceToken(toReplace, replacement);
            }
        }

        public override string ToString()
        {
            return $"if ({_predicate.Value}): {_provider.Value}";
        }
    }
}
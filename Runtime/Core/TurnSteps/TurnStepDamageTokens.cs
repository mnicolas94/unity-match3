using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Match3.Core.SerializableTuples;
using Match3.Core.TurnSteps.TokensDestructionSource;
using UnityEngine;

namespace Match3.Core.TurnSteps
{
    
    [Serializable]
    public class TurnStepDamageTokens : TurnStep
    {
        [SerializeField] private List<TokensDamaged> _tokensDamaged;
        
        public List<TokensDamaged> TokensDamaged => _tokensDamaged;

        public TurnStepDamageTokens(List<TokensDamaged> destroyedTokens)
        {
            _tokensDamaged = destroyedTokens;
        }

        public IEnumerable<PositionTokenDamageOrder> GetAllPositionsTokensDestructionOrders()
        {
            return _tokensDamaged
                .SelectMany(destruction => destruction.DestroyedTokens);
        }
        
        public IEnumerable<PositionToken> GetAllPositionsTokens()
        {
            return _tokensDamaged
                .SelectMany(destruction => destruction.DestroyedTokens)
                .Select(ptdo => ptdo.PositionToken);
        }
    }

    [Serializable]
    public class TokensDamaged
    {
        [SerializeReference] private ITokenDamageSource _source;
        [SerializeField] private Vector2Int _sourcePosition;
        [SerializeField] private List<PositionTokenDamageOrder> _destroyedTokens;

        public ITokenDamageSource Source => _source;

        public Vector2Int SourcePosition => _sourcePosition;

        public ReadOnlyCollection<PositionTokenDamageOrder> DestroyedTokens => _destroyedTokens.AsReadOnly();

        public TokensDamaged(ITokenDamageSource source, Vector2Int sourcePosition, List<PositionTokenDamageOrder> destroyedTokens)
        {
            _source = source;
            _sourcePosition = sourcePosition;
            _destroyedTokens = destroyedTokens;
        }
    }

    public static class TokensDestructionListExtensions
    {
        public static IEnumerable<PositionToken> GetAllPositionsTokens(this IList<TokensDamaged> destructions)
        {
            return destructions
                .SelectMany(destruction => destruction.DestroyedTokens)
                .Select(ptdo => new PositionToken(ptdo.Position, ptdo.Token)).ToList();
        }
    }
}
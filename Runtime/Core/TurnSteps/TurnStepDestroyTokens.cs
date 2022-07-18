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
    public class TurnStepDestroyTokens : TurnStep
    {
        [SerializeField] private List<TokensDestruction> _tokensDestructions;
        
        public List<TokensDestruction> TokensDestructions => _tokensDestructions;

        public TurnStepDestroyTokens(List<TokensDestruction> destroyedTokens)
        {
            _tokensDestructions = destroyedTokens;
        }

        public IEnumerable<PositionTokenDestructionOrder> GetAllPositionsTokensDestructionOrders()
        {
            return _tokensDestructions
                .SelectMany(destruction => destruction.DestroyedTokens);
        }
        
        public IEnumerable<PositionToken> GetAllPositionsTokens()
        {
            return _tokensDestructions
                .SelectMany(destruction => destruction.DestroyedTokens)
                .Select(ptdo => ptdo.PositionToken);
        }
    }

    [Serializable]
    public class TokensDestruction
    {
        [SerializeReference] private ITokenDestructionSource _source;
        [SerializeField] private Vector2Int _sourcePosition;
        [SerializeField] private List<PositionTokenDestructionOrder> _destroyedTokens;

        public ITokenDestructionSource Source => _source;

        public Vector2Int SourcePosition => _sourcePosition;

        public ReadOnlyCollection<PositionTokenDestructionOrder> DestroyedTokens => _destroyedTokens.AsReadOnly();

        public TokensDestruction(ITokenDestructionSource source, Vector2Int sourcePosition, List<PositionTokenDestructionOrder> destroyedTokens)
        {
            _source = source;
            _sourcePosition = sourcePosition;
            _destroyedTokens = destroyedTokens;
        }
    }

    public static class TokensDestructionListExtensions
    {
        public static IEnumerable<PositionToken> GetAllPositionsTokens(this IList<TokensDestruction> destructions)
        {
            return destructions
                .SelectMany(destruction => destruction.DestroyedTokens)
                .Select(ptdo => new PositionToken(ptdo.Position, ptdo.Token)).ToList();
        }
    }
}
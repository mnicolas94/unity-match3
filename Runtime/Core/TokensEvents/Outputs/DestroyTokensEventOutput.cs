using System;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.Core.TokensEvents.Outputs
{
    [Serializable]
    public class DestroyTokensEventOutput : TokenEventOutput
    {
        [SerializeField] private TokensDestruction _tokensToDestroy;

        public TokensDestruction TokensToDestroy => _tokensToDestroy;

        public DestroyTokensEventOutput(TokensDestruction tokensToDestroy)
        {
            _tokensToDestroy = tokensToDestroy;
        }
    }
}
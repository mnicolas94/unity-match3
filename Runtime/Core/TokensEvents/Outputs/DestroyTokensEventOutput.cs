using System;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.Core.TokensEvents.Outputs
{
    [Serializable]
    public class DestroyTokensEventOutput : TokenEventOutput
    {
        [SerializeField] private TokensDamaged _tokensToDestroy;

        public TokensDamaged TokensToDestroy => _tokensToDestroy;

        public DestroyTokensEventOutput(TokensDamaged tokensToDestroy)
        {
            _tokensToDestroy = tokensToDestroy;
        }
    }
}
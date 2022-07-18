using System;
using Match3.Core.Matches;
using UnityEngine;

namespace Match3.Core.TurnSteps.TokensDestructionSource
{
    [Serializable]
    public class DestructionSourceMatch : ITokenDestructionSource
    {
        [SerializeField] private Match _match;

        public Match Match => _match;

        public DestructionSourceMatch(Match match)
        {
            _match = match;
        }
    }
}
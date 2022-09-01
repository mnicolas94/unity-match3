using System;
using UnityEngine;

namespace Match3.Core.Levels.TokensCreationConditions
{
    [Serializable]
    public class TokenCreationPredicateTurnCount : ITokenCreationPredicate
    {
        [SerializeField] private int _turnCount;

        public TokenCreationPredicateTurnCount(int turnCount)
        {
            _turnCount = turnCount;
        }

        public TokenCreationPredicateTurnCount() : this(0)
        {
        }

        public bool IsMet((GameController, TokenSource, Vector2Int) input)
        {
            var (gameController, _, _) = input;
            return gameController.TurnCount >= _turnCount;
        }

        public override string ToString()
        {
            return $"turn=={_turnCount}";
        }
    }
}
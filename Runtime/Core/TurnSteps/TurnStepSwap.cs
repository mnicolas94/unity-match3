using System;
using UnityEngine;

namespace Match3.Core.TurnSteps
{
    [Serializable]
    public class TurnStepSwap : TurnStep
    {
        [SerializeField] private Token _tokenA;
        [SerializeField] private Token _tokenB;
        [SerializeField] private Vector2Int _tokenPositionA;
        [SerializeField] private Vector2Int _tokenPositionB;

        public Token TokenA => _tokenA;

        public Token TokenB => _tokenB;

        public Vector2Int TokenPositionA => _tokenPositionA;

        public Vector2Int TokenPositionB => _tokenPositionB;

        public TurnStepSwap(Token tokenA, Token tokenB, Vector2Int tokenPositionA, Vector2Int tokenPositionB)
        {
            _tokenA = tokenA;
            _tokenB = tokenB;
            _tokenPositionA = tokenPositionA;
            _tokenPositionB = tokenPositionB;
        }
    }
}
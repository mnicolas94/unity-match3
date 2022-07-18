using System;
using UnityEngine;

namespace Match3.Core.GameActions.Interactions
{
    [Serializable]
    public class SwapInteraction : IInteraction
    {
        [SerializeField] private Vector2Int _firstPosition;
        [SerializeField] private Vector2Int _secondPosition;

        public Vector2Int FirstPosition => _firstPosition;

        public Vector2Int SecondPosition => _secondPosition;

        public SwapInteraction(Vector2Int firstPosition, Vector2Int secondPosition)
        {
            _firstPosition = firstPosition;
            _secondPosition = secondPosition;
        }
    }
}
using System;
using UnityEngine;

namespace Match3.Core.GameActions.Interactions
{
    [Serializable]
    public class DoubleClickInteraction : IInteraction
    {
        [SerializeField] private Vector2Int _position;

        public Vector2Int Position => _position;

        public DoubleClickInteraction(Vector2Int position)
        {
            _position = position;
        }
    }
}
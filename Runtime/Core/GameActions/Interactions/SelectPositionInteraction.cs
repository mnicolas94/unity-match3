using System;
using UnityEngine;

namespace Match3.Core.GameActions.Interactions
{
    [Serializable]
    public class SelectPositionInteraction : IInteraction
    {
        [SerializeField] private Vector2Int _position;

        public Vector2Int Position => _position;

        public SelectPositionInteraction(Vector2Int position)
        {
            _position = position;
        }
    }
}
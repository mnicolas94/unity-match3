using System;
using Match3.Core;
using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// IPair of type `&lt;BoardCores.Core.GameController&gt;`. Inherits from `IPair&lt;BoardCores.Core.GameController&gt;`.
    /// </summary>
    [Serializable]
    public struct GameControllerPair : IPair<GameController>
    {
        public GameController Item1 { get => _item1; set => _item1 = value; }
        public GameController Item2 { get => _item2; set => _item2 = value; }

        [SerializeField]
        private GameController _item1;
        [SerializeField]
        private GameController _item2;

        public void Deconstruct(out GameController item1, out GameController item2) { item1 = Item1; item2 = Item2; }
    }
}
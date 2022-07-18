using System;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// IPair of type `&lt;BoardCores.Core.TurnSteps.TurnStep&gt;`. Inherits from `IPair&lt;BoardCores.Core.TurnSteps.TurnStep&gt;`.
    /// </summary>
    [Serializable]
    public struct TurnStepPair : IPair<TurnStep>
    {
        public TurnStep Item1 { get => _item1; set => _item1 = value; }
        public TurnStep Item2 { get => _item2; set => _item2 = value; }

        [SerializeField]
        private TurnStep _item1;
        [SerializeField]
        private TurnStep _item2;

        public void Deconstruct(out TurnStep item1, out TurnStep item2) { item1 = Item1; item2 = Item2; }
    }
}
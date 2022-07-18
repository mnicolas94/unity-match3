using UnityEngine;
using System;
using Match3.Core.TurnSteps;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Variable of type `BoardCores.Core.TurnSteps.TurnStep`. Inherits from `AtomVariable&lt;BoardCores.Core.TurnSteps.TurnStep, TurnStepPair, TurnStepEvent, TurnStepPairEvent, TurnStepTurnStepFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-lush")]
    [CreateAssetMenu(menuName = "Unity Atoms/Variables/TurnStep", fileName = "TurnStepVariable")]
    public sealed class TurnStepVariable : AtomVariable<TurnStep, TurnStepPair, TurnStepEvent, TurnStepPairEvent, TurnStepTurnStepFunction>
    {
        protected override bool ValueEquals(TurnStep other)
        {
            throw new NotImplementedException();
        }
    }
}

using Match3.Core.TurnSteps;
using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Variable Instancer of type `BoardCores.Core.TurnSteps.TurnStep`. Inherits from `AtomVariableInstancer&lt;TurnStepVariable, TurnStepPair, BoardCores.Core.TurnSteps.TurnStep, TurnStepEvent, TurnStepPairEvent, TurnStepTurnStepFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-hotpink")]
    [AddComponentMenu("Unity Atoms/Variable Instancers/TurnStep Variable Instancer")]
    public class TurnStepVariableInstancer : AtomVariableInstancer<
        TurnStepVariable,
        TurnStepPair,
        TurnStep,
        TurnStepEvent,
        TurnStepPairEvent,
        TurnStepTurnStepFunction>
    { }
}

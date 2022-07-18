using System;
using Match3.Core.TurnSteps;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event Reference of type `BoardCores.Core.TurnSteps.TurnStep`. Inherits from `AtomEventReference&lt;BoardCores.Core.TurnSteps.TurnStep, TurnStepVariable, TurnStepEvent, TurnStepVariableInstancer, TurnStepEventInstancer&gt;`.
    /// </summary>
    [Serializable]
    public sealed class TurnStepEventReference : AtomEventReference<
        TurnStep,
        TurnStepVariable,
        TurnStepEvent,
        TurnStepVariableInstancer,
        TurnStepEventInstancer>, IGetEvent 
    { }
}

using System;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event Reference of type `TurnStepPair`. Inherits from `AtomEventReference&lt;TurnStepPair, TurnStepVariable, TurnStepPairEvent, TurnStepVariableInstancer, TurnStepPairEventInstancer&gt;`.
    /// </summary>
    [Serializable]
    public sealed class TurnStepPairEventReference : AtomEventReference<
        TurnStepPair,
        TurnStepVariable,
        TurnStepPairEvent,
        TurnStepVariableInstancer,
        TurnStepPairEventInstancer>, IGetEvent 
    { }
}

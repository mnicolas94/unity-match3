using System;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event Reference of type `GameControllerPair`. Inherits from `AtomEventReference&lt;GameControllerPair, GameControllerVariable, GameControllerPairEvent, GameControllerVariableInstancer, GameControllerPairEventInstancer&gt;`.
    /// </summary>
    [Serializable]
    public sealed class GameControllerPairEventReference : AtomEventReference<
        GameControllerPair,
        GameControllerVariable,
        GameControllerPairEvent,
        GameControllerVariableInstancer,
        GameControllerPairEventInstancer>, IGetEvent 
    { }
}

using System;
using Match3.Core;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event Reference of type `BoardCores.Core.GameController`. Inherits from `AtomEventReference&lt;BoardCores.Core.GameController, GameControllerVariable, GameControllerEvent, GameControllerVariableInstancer, GameControllerEventInstancer&gt;`.
    /// </summary>
    [Serializable]
    public sealed class GameControllerEventReference : AtomEventReference<
        GameController,
        GameControllerVariable,
        GameControllerEvent,
        GameControllerVariableInstancer,
        GameControllerEventInstancer>, IGetEvent 
    { }
}

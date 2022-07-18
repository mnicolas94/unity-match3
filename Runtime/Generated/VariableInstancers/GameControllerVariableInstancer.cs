using Match3.Core;
using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Variable Instancer of type `BoardCores.Core.GameController`. Inherits from `AtomVariableInstancer&lt;GameControllerVariable, GameControllerPair, BoardCores.Core.GameController, GameControllerEvent, GameControllerPairEvent, GameControllerGameControllerFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-hotpink")]
    [AddComponentMenu("Unity Atoms/Variable Instancers/GameController Variable Instancer")]
    public class GameControllerVariableInstancer : AtomVariableInstancer<
        GameControllerVariable,
        GameControllerPair,
        GameController,
        GameControllerEvent,
        GameControllerPairEvent,
        GameControllerGameControllerFunction>
    { }
}

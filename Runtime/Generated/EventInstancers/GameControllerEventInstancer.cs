using Match3.Core;
using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event Instancer of type `BoardCores.Core.GameController`. Inherits from `AtomEventInstancer&lt;BoardCores.Core.GameController, GameControllerEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-sign-blue")]
    [AddComponentMenu("Unity Atoms/Event Instancers/GameController Event Instancer")]
    public class GameControllerEventInstancer : AtomEventInstancer<GameController, GameControllerEvent> { }
}

using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event Instancer of type `GameControllerPair`. Inherits from `AtomEventInstancer&lt;GameControllerPair, GameControllerPairEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-sign-blue")]
    [AddComponentMenu("Unity Atoms/Event Instancers/GameControllerPair Event Instancer")]
    public class GameControllerPairEventInstancer : AtomEventInstancer<GameControllerPair, GameControllerPairEvent> { }
}

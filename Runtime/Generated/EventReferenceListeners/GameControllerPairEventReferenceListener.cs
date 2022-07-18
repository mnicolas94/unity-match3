using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event Reference Listener of type `GameControllerPair`. Inherits from `AtomEventReferenceListener&lt;GameControllerPair, GameControllerPairEvent, GameControllerPairEventReference, GameControllerPairUnityEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-orange")]
    [AddComponentMenu("Unity Atoms/Listeners/GameControllerPair Event Reference Listener")]
    public sealed class GameControllerPairEventReferenceListener : AtomEventReferenceListener<
        GameControllerPair,
        GameControllerPairEvent,
        GameControllerPairEventReference,
        GameControllerPairUnityEvent>
    { }
}

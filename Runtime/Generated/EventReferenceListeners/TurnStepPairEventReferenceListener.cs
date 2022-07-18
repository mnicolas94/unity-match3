using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event Reference Listener of type `TurnStepPair`. Inherits from `AtomEventReferenceListener&lt;TurnStepPair, TurnStepPairEvent, TurnStepPairEventReference, TurnStepPairUnityEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-orange")]
    [AddComponentMenu("Unity Atoms/Listeners/TurnStepPair Event Reference Listener")]
    public sealed class TurnStepPairEventReferenceListener : AtomEventReferenceListener<
        TurnStepPair,
        TurnStepPairEvent,
        TurnStepPairEventReference,
        TurnStepPairUnityEvent>
    { }
}

using Match3.Core.TurnSteps;
using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event Reference Listener of type `BoardCores.Core.TurnSteps.TurnStep`. Inherits from `AtomEventReferenceListener&lt;BoardCores.Core.TurnSteps.TurnStep, TurnStepEvent, TurnStepEventReference, TurnStepUnityEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-orange")]
    [AddComponentMenu("Unity Atoms/Listeners/TurnStep Event Reference Listener")]
    public sealed class TurnStepEventReferenceListener : AtomEventReferenceListener<
        TurnStep,
        TurnStepEvent,
        TurnStepEventReference,
        TurnStepUnityEvent>
    { }
}

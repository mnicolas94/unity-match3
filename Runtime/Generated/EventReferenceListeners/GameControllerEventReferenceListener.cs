using Match3.Core;
using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event Reference Listener of type `BoardCores.Core.GameController`. Inherits from `AtomEventReferenceListener&lt;BoardCores.Core.GameController, GameControllerEvent, GameControllerEventReference, GameControllerUnityEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-orange")]
    [AddComponentMenu("Unity Atoms/Listeners/GameController Event Reference Listener")]
    public sealed class GameControllerEventReferenceListener : AtomEventReferenceListener<
        GameController,
        GameControllerEvent,
        GameControllerEventReference,
        GameControllerUnityEvent>
    { }
}

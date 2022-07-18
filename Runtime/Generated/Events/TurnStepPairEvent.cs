using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event of type `TurnStepPair`. Inherits from `AtomEvent&lt;TurnStepPair&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/TurnStepPair", fileName = "TurnStepPairEvent")]
    public sealed class TurnStepPairEvent : AtomEvent<TurnStepPair>
    {
    }
}

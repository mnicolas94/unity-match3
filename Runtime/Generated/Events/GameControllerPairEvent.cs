using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event of type `GameControllerPair`. Inherits from `AtomEvent&lt;GameControllerPair&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/GameControllerPair", fileName = "GameControllerPairEvent")]
    public sealed class GameControllerPairEvent : AtomEvent<GameControllerPair>
    {
    }
}

using Match3.Core;
using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event of type `BoardCores.Core.GameController`. Inherits from `AtomEvent&lt;BoardCores.Core.GameController&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/GameController", fileName = "GameControllerEvent")]
    public sealed class GameControllerEvent : AtomEvent<GameController>
    {
    }
}

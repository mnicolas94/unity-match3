using Match3.Core.TurnSteps;
using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event of type `BoardCores.Core.TurnSteps.TurnStep`. Inherits from `AtomEvent&lt;BoardCores.Core.TurnSteps.TurnStep&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/TurnStep", fileName = "TurnStepEvent")]
    public sealed class TurnStepEvent : AtomEvent<TurnStep>
    {
    }
}

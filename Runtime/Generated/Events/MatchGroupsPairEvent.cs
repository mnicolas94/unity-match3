using UnityEngine;
using Match3.Core.Matches;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event of type `MatchGroupsPair`. Inherits from `AtomEvent&lt;MatchGroupsPair&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/MatchGroupsPair", fileName = "MatchGroupsPairEvent")]
    public sealed class MatchGroupsPairEvent : AtomEvent<MatchGroupsPair>
    {
    }
}

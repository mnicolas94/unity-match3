using UnityEngine;
using Match3.Core.Matches;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event of type `Match3.Core.Matches.MatchGroups`. Inherits from `AtomEvent&lt;Match3.Core.Matches.MatchGroups&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/MatchGroups", fileName = "MatchGroupsEvent")]
    public sealed class MatchGroupsEvent : AtomEvent<Match3.Core.Matches.MatchGroups>
    {
    }
}

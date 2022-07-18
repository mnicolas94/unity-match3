using UnityEngine;
using UnityAtoms.BaseAtoms;
using Match3.Core.Matches;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Variable Instancer of type `Match3.Core.Matches.MatchGroups`. Inherits from `AtomVariableInstancer&lt;MatchGroupsVariable, MatchGroupsPair, Match3.Core.Matches.MatchGroups, MatchGroupsEvent, MatchGroupsPairEvent, MatchGroupsMatchGroupsFunction&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-hotpink")]
    [AddComponentMenu("Unity Atoms/Variable Instancers/MatchGroups Variable Instancer")]
    public class MatchGroupsVariableInstancer : AtomVariableInstancer<
        MatchGroupsVariable,
        MatchGroupsPair,
        Match3.Core.Matches.MatchGroups,
        MatchGroupsEvent,
        MatchGroupsPairEvent,
        MatchGroupsMatchGroupsFunction>
    { }
}

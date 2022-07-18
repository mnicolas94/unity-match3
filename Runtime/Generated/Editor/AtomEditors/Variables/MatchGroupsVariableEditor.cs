using UnityEditor;
using UnityAtoms.Editor;
using Match3.Core.Matches;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Variable Inspector of type `Match3.Core.Matches.MatchGroups`. Inherits from `AtomVariableEditor`
    /// </summary>
    [CustomEditor(typeof(MatchGroupsVariable))]
    public sealed class MatchGroupsVariableEditor : AtomVariableEditor<Match3.Core.Matches.MatchGroups, MatchGroupsPair> { }
}

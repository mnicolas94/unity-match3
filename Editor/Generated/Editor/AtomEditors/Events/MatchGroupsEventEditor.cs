#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;
using Match3.Core.Matches;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `Match3.Core.Matches.MatchGroups`. Inherits from `AtomEventEditor&lt;Match3.Core.Matches.MatchGroups, MatchGroupsEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(MatchGroupsEvent))]
    public sealed class MatchGroupsEventEditor : AtomEventEditor<Match3.Core.Matches.MatchGroups, MatchGroupsEvent> { }
}
#endif

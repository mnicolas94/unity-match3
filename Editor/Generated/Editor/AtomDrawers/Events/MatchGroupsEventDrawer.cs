#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `Match3.Core.Matches.MatchGroups`. Inherits from `AtomDrawer&lt;MatchGroupsEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(MatchGroupsEvent))]
    public class MatchGroupsEventDrawer : AtomDrawer<MatchGroupsEvent> { }
}
#endif

#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Variable property drawer of type `Match3.Core.Matches.MatchGroups`. Inherits from `AtomDrawer&lt;MatchGroupsVariable&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(MatchGroupsVariable))]
    public class MatchGroupsVariableDrawer : VariableDrawer<MatchGroupsVariable> { }
}
#endif

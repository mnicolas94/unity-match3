#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Constant property drawer of type `Match3.Core.Matches.MatchGroups`. Inherits from `AtomDrawer&lt;MatchGroupsConstant&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(MatchGroupsConstant))]
    public class MatchGroupsConstantDrawer : VariableDrawer<MatchGroupsConstant> { }
}
#endif

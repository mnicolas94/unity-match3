#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;
using Match3.Core.Matches;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `MatchGroupsPair`. Inherits from `AtomEventEditor&lt;MatchGroupsPair, MatchGroupsPairEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(MatchGroupsPairEvent))]
    public sealed class MatchGroupsPairEventEditor : AtomEventEditor<MatchGroupsPair, MatchGroupsPairEvent> { }
}
#endif

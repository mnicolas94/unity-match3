#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityEngine.UIElements;
using UnityAtoms.Editor;
using Match3.Core;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `Match3.Core.TokenData`. Inherits from `AtomEventEditor&lt;Match3.Core.TokenData, TokenDataEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomEditor(typeof(TokenDataEvent))]
    public sealed class TokenDataEventEditor : AtomEventEditor<Match3.Core.TokenData, TokenDataEvent> { }
}
#endif

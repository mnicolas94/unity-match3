#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Event property drawer of type `Match3.Core.TokenData`. Inherits from `AtomDrawer&lt;TokenDataEvent&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(TokenDataEvent))]
    public class TokenDataEventDrawer : AtomDrawer<TokenDataEvent> { }
}
#endif

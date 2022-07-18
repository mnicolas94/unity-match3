#if UNITY_2019_1_OR_NEWER
using UnityEditor;
using UnityAtoms.Editor;

namespace UnityAtoms.BaseAtoms.Editor
{
    /// <summary>
    /// Value List property drawer of type `Match3.Core.TokenData`. Inherits from `AtomDrawer&lt;TokenDataValueList&gt;`. Only availble in `UNITY_2019_1_OR_NEWER`.
    /// </summary>
    [CustomPropertyDrawer(typeof(TokenDataValueList))]
    public class TokenDataValueListDrawer : AtomDrawer<TokenDataValueList> { }
}
#endif

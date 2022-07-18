using UnityEngine;
using Match3.Core;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Value List of type `Match3.Core.TokenData`. Inherits from `AtomValueList&lt;Match3.Core.TokenData, TokenDataEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-piglet")]
    [CreateAssetMenu(menuName = "Unity Atoms/Value Lists/TokenData", fileName = "TokenDataValueList")]
    public sealed class TokenDataValueList : AtomValueList<Match3.Core.TokenData, TokenDataEvent> { }
}

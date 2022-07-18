using UnityEngine;
using Match3.Core;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event of type `Match3.Core.TokenData`. Inherits from `AtomEvent&lt;Match3.Core.TokenData&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-cherry")]
    [CreateAssetMenu(menuName = "Unity Atoms/Events/TokenData", fileName = "TokenDataEvent")]
    public sealed class TokenDataEvent : AtomEvent<Match3.Core.TokenData>
    {
    }
}

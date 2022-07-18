using System;
using UnityEngine.Events;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// None generic Unity Event of type `TurnStepPair`. Inherits from `UnityEvent&lt;TurnStepPair&gt;`.
    /// </summary>
    [Serializable]
    public sealed class TurnStepPairUnityEvent : UnityEvent<TurnStepPair> { }
}

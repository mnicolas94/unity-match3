using System;
using Match3.Core.TurnSteps;
using UnityEngine.Events;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// None generic Unity Event of type `BoardCores.Core.TurnSteps.TurnStep`. Inherits from `UnityEvent&lt;BoardCores.Core.TurnSteps.TurnStep&gt;`.
    /// </summary>
    [Serializable]
    public sealed class TurnStepUnityEvent : UnityEvent<TurnStep> { }
}

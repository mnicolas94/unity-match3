using System;
using Match3.Core;
using UnityEngine.Events;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// None generic Unity Event of type `BoardCores.Core.GameController`. Inherits from `UnityEvent&lt;BoardCores.Core.GameController&gt;`.
    /// </summary>
    [Serializable]
    public sealed class GameControllerUnityEvent : UnityEvent<GameController> { }
}

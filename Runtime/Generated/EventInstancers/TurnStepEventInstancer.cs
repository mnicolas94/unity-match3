using Match3.Core.TurnSteps;
using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event Instancer of type `BoardCores.Core.TurnSteps.TurnStep`. Inherits from `AtomEventInstancer&lt;BoardCores.Core.TurnSteps.TurnStep, TurnStepEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-sign-blue")]
    [AddComponentMenu("Unity Atoms/Event Instancers/TurnStep Event Instancer")]
    public class TurnStepEventInstancer : AtomEventInstancer<TurnStep, TurnStepEvent> { }
}

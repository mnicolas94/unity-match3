using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Event Instancer of type `TurnStepPair`. Inherits from `AtomEventInstancer&lt;TurnStepPair, TurnStepPairEvent&gt;`.
    /// </summary>
    [EditorIcon("atom-icon-sign-blue")]
    [AddComponentMenu("Unity Atoms/Event Instancers/TurnStepPair Event Instancer")]
    public class TurnStepPairEventInstancer : AtomEventInstancer<TurnStepPair, TurnStepPairEvent> { }
}

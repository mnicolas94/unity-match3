using Match3.Core.TurnSteps;
using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Adds Variable Instancer's Variable of type BoardCores.Core.TurnSteps.TurnStep to a Collection or List on OnEnable and removes it on OnDestroy. 
    /// </summary>
    [AddComponentMenu("Unity Atoms/Sync Variable Instancer to Collection/Sync TurnStep Variable Instancer to Collection")]
    [EditorIcon("atom-icon-delicate")]
    public class SyncTurnStepVariableInstancerToCollection : SyncVariableInstancerToCollection<TurnStep, TurnStepVariable, TurnStepVariableInstancer> { }
}

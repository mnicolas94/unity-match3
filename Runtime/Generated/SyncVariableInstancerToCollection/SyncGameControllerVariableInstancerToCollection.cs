using Match3.Core;
using UnityEngine;

namespace UnityAtoms.BaseAtoms
{
    /// <summary>
    /// Adds Variable Instancer's Variable of type BoardCores.Core.GameController to a Collection or List on OnEnable and removes it on OnDestroy. 
    /// </summary>
    [AddComponentMenu("Unity Atoms/Sync Variable Instancer to Collection/Sync GameController Variable Instancer to Collection")]
    [EditorIcon("atom-icon-delicate")]
    public class SyncGameControllerVariableInstancerToCollection : SyncVariableInstancerToCollection<GameController, GameControllerVariable, GameControllerVariableInstancer> { }
}

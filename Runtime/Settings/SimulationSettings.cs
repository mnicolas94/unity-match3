using Match3.Core;
using UnityEngine;
using Utils;

namespace Match3.Settings
{
    public class SimulationSettings : ScriptableObjectSingleton<SimulationSettings>
    {
        [SerializeField] private GameContextAsset _defaultSimulationContext;

        public GameContextAsset DefaultSimulationContext => _defaultSimulationContext;
    }
}
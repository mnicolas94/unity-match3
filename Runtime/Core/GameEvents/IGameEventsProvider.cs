using UnityAtoms.BaseAtoms;

namespace Match3.Core.GameEvents
{
    public interface IGameEventsProvider
    {
        GameControllerEvent GameStartedEvent { get; }
        
        GameControllerEvent GameEndedEvent { get; }
        
        TurnStepEvent TurnStepEvent { get; }
    }
}
using Match3.Core.TurnSteps;

namespace Match3.Core.GameEvents.Observers
{
    public interface ITurnStepObserver : IGameObserver
    {
        void OnTurnStep(TurnStep step);
    }
}
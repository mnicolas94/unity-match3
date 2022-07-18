namespace Match3.Core.GameEvents.Observers
{
    public interface IGameEndedObserver
    {
        void OnGameEnded(GameController controller);
    }
}

namespace Match3.Core.GameEvents.Observers
{
    public interface IGameStartObserver : IGameObserver
    {
        void OnGameStarted(GameController controller);
    }
}
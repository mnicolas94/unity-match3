using UnityEngine;

namespace Match3.Core
{
    [CreateAssetMenu(fileName = "GameContext", menuName = "Facticus/Match3/GameContext", order = 0)]
    public class GameContextAsset : ScriptableObject
    {
        [SerializeField] private GameContext _gameContext;

        public GameContext GameContext => new GameContext(_gameContext);
    }
}
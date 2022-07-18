using System.Collections;
using Match3.Core.Levels;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3.View
{
    public class BoardLevelInitializer : MonoBehaviour
    {
        [SerializeField] private GameControllerView gameController;
        [SerializeField] private Level level;
        [SerializeField] private int seed;
        [SerializeField] private bool _initializeLastLevelOnStart;

        public Level Level
        {
            get => level;
            set => level = value;
        }

        private IEnumerator Start()
        {
            yield return null;
            
            if (_initializeLastLevelOnStart)
                InitializeLevel();
        }

        [NaughtyAttributes.Button]
        public void InitializeLevel()
        {
            InitializeLevel(level);
        }

        private void InitializeLevel(Level lvl)
        {
            if (seed != 0)
            {
                var temp = Random.state;
                Random.InitState(seed);
                gameController.StartGameInLevel(lvl);
                Random.state = temp;
            }
            else
            {
                gameController.StartGameInLevel(lvl);
            }
        }
    }
}
using System;
using Match3.Core.GameEvents.Observers;
using Match3.Core.TurnSteps;
using UnityEngine;

namespace Match3.Core.GameSerialization
{
    [Serializable]
    public class SerializeGameObserver : IGameStartObserver, IGameEndedObserver, ITurnStepObserver
    {
        private SerializableGame _game;
        
        public void OnGameStarted(GameController controller)
        {
            _game = new SerializableGame(controller.CurrentLevel, Copy(controller.Board));
        }

        public void OnGameEnded(GameController controller)
        {
            SaveCurrentGame();
        }
        
        public void OnTurnStep(TurnStep turnStep)
        {
            _game.AddTurnStep(turnStep);
        }

        private void SaveCurrentGame()
        {
            if (_game != null)
            {
                SerializedGames.Instance.Add(_game);
            }
        }

        private T Copy<T>(T original)
        {
            var json = JsonUtility.ToJson(original);
            var copy = JsonUtility.FromJson<T>(json);
            return copy;
        }
    }
}
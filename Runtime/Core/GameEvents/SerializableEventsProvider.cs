using System;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Match3.Core.GameEvents
{
    [CreateAssetMenu(fileName = "SerializableEventsProvider", menuName = "Facticus/Match3/Events/SerializableEventsProvider", order = 0)]
    public class SerializableEventsProvider : ScriptableObject, IGameEventsProvider
    {
        [SerializeField] private GameControllerEvent _gameStartedEvent;
        [SerializeField] private GameControllerEvent _gameEndedEvent;
        [SerializeField] private TurnStepEvent _turnStepEvent;
        
        public GameControllerEvent GameStartedEvent => _gameStartedEvent;

        public GameControllerEvent GameEndedEvent => _gameEndedEvent;

        public TurnStepEvent TurnStepEvent => _turnStepEvent;

        public static SerializableEventsProvider Create()
        {
            SerializableEventsProvider provider = null;
            // sometimes this is called during serialization and will throw an exception
            try
            {
                provider = CreateInstance<SerializableEventsProvider>();
                provider._gameStartedEvent = CreateInstance<GameControllerEvent>();
                provider._gameEndedEvent = CreateInstance<GameControllerEvent>();
                provider._turnStepEvent = CreateInstance<TurnStepEvent>();
            }
            catch
            {
                // ignored
            }

            return provider;
        }
    }
}
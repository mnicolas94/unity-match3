using System.Collections.Generic;
using Match3.Core.GameEvents;
using Match3.Core.GameEvents.Observers;
using UnityEngine;

namespace Match3.View
{
    public class GameObserversRegister : MonoBehaviour
    {
        [SerializeField] private SerializableEventsProvider _provider;
        [SerializeReference, SubclassSelector] private List<IGameObserver> _observers;
        
        public void AddObserver(IGameObserver observer)
        {
            _observers.Add(observer);
            if (enabled)
                RegisterObserver(observer);
        }

        private void OnEnable()
        {
            _observers.ForEach(RegisterObserver);
        }
        
        private void OnDisable()
        {
            _observers.ForEach(UnregisterObserver);
        }

        private void RegisterObserver(IGameObserver observer)
        {
            if (observer is IGameStartObserver startObserver)
            {
                _provider.GameStartedEvent.Register(startObserver.OnGameStarted);
            }
            
            if (observer is IGameEndedObserver endObserver)
            {
                _provider.GameEndedEvent.Register(endObserver.OnGameEnded);
            }
            
            if (observer is ITurnStepObserver turnObserver)
            {
                _provider.TurnStepEvent.Register(turnObserver.OnTurnStep);
            }
        }
        
        private void UnregisterObserver(IGameObserver observer)
        {
            if (observer is IGameStartObserver startObserver)
            {
                _provider.GameStartedEvent.Unregister(startObserver.OnGameStarted);
            }
            
            if (observer is IGameEndedObserver endObserver)
            {
                _provider.GameEndedEvent.Unregister(endObserver.OnGameEnded);
            }
            
            if (observer is ITurnStepObserver turnObserver)
            {
                _provider.TurnStepEvent.Unregister(turnObserver.OnTurnStep);
            }
        }
    }
}
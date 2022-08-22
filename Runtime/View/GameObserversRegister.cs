using System.Collections.Generic;
using Match3.Core.GameEvents;
using Match3.Core.GameEvents.Observers;
using TNRD;
using UnityEngine;

namespace Match3.View
{
    public class GameObserversRegister : MonoBehaviour
    {
        [SerializeField] private SerializableEventsProvider _provider;
        [SerializeField] private List<SerializableInterface<IGameObserver>> _observers;
        
        public void AddObserver(IGameObserver observer)
        {
            var reference = new SerializableInterface<IGameObserver>
            {
                Value = observer
            };
            _observers.Add(reference);
            if (enabled)
                RegisterObserver(reference);
        }

        private void OnEnable()
        {
            _observers.ForEach(RegisterObserver);
        }
        
        private void OnDisable()
        {
            _observers.ForEach(UnregisterObserver);
        }

        private void RegisterObserver(SerializableInterface<IGameObserver> observerReference)
        {
            var observer = observerReference.Value;
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
        
        private void UnregisterObserver(SerializableInterface<IGameObserver> observerReference)
        {
            var observer = observerReference.Value;
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
using UnityEngine;
using UnityEngine.Events;

namespace Match3.View
{
    public class PoolableParticles : MonoBehaviour
    {
        [SerializeField] public UnityEvent OnEnableCallback;
        [SerializeField] public UnityEvent OnParticleSystemStoppedCallback;
        
        private void OnEnable()
        {
            OnEnableCallback?.Invoke();
        }

        private void OnParticleSystemStopped()
        {
            OnParticleSystemStoppedCallback?.Invoke();
        }
    }
}
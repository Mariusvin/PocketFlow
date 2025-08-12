using System;
using UnityEngine;

namespace Utility
{
    public class ParticlesStopCallback : MonoBehaviour
    {
        private event Action OnParticlesStop;

        public void AddStopCallback(Action _callback)
        {
            OnParticlesStop += _callback;
        }
        
        public void OnParticleSystemStopped()
        {
            if (OnParticlesStop == null)
            {
                Debug.LogError("Particles played with empty callback. This might be inefficient");
            }
            
            OnParticlesStop?.Invoke();
        }
    }
}
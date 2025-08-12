using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class ParticlesEventPlayer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<ParticleSystem> particles;

        // Used in animation events
        public void PlayParticles(int _particlesIndex)
        {
            if (particles.Count <= 0)
            {
                Debug.Log("Particles list is empty. Skipping playing");
                return;
            }

            if (_particlesIndex < 0 || _particlesIndex >= particles.Count)
            {
                Debug.Log("Incorrect index provided. Skipping playing");
                return;
            }

            ParticleSystem _particleSystem = particles[_particlesIndex];
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _particleSystem.Play();
        }
    }
}
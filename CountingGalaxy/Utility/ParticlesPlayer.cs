using System;
using UnityEngine;

namespace Utility
{
    public class ParticlesPlayer : MonoSingleton<ParticlesPlayer>
    {
        public static ParticleSystem PlayParticlesSimple(ParticleSystem _target, Vector3 _targetLocation, Transform _parent = null, bool _destroyAfterPlaying = true)
        {
            if (!_target)
            {
                Debug.Log("Particles object is null");
                return null;
            }
            
            return PlayParticlesAt(_target, _targetLocation, _parent, _destroyAfterPlaying);
        }

        public static ParticleSystem PlayParticlesSimple(ParticleSystem _target, Vector3 _targetLocation, Color _color, float _scaleMultiplier = 1.0f, Transform _parent = null, bool _destroyAfterPlaying = true)
        {
            if (!_target)
            {
                Debug.Log("Particles object is null");
                return null;
            }
            
            return PlayParticlesAt(_target, _targetLocation, _color, _scaleMultiplier, _parent, _destroyAfterPlaying);
        }
        
        public static ParticleSystem PlayParticlesCallback(ParticleSystem _target, Vector3 _targetLocation, Transform _parent = null, Action _stopCallback = null)
        {
            if (!_target)
            {
                Debug.Log("Particles object is null");
                return null;
            }
            
            return PlayParticlesAt(_target, _targetLocation, _parent, _stopCallback);
        }

        private static ParticleSystem PlayParticlesAt(ParticleSystem _target, Vector3 _targetLocation, Transform _parent, bool _destroyAfterPlaying)
        {
            ParticleSystem _ps = Instantiate(_target);
            if (_parent)
            {
                _ps.transform.SetParent(_parent);
            }

            _ps.transform.position = _targetLocation;

            if (_destroyAfterPlaying)
            {
                ParticleSystem.MainModule _psMain = _ps.main;
                _psMain.stopAction = ParticleSystemStopAction.Destroy;
            }
            
            _ps.Play();
            return _ps;
        }
        
        private static ParticleSystem PlayParticlesAt(ParticleSystem _target, Vector3 _targetLocation, Color _mainColor, float _scaleMultiplier, Transform _parent, bool _destroyAfterPlaying)
        {
            ParticleSystem _ps = Instantiate(_target);
            if (_parent)
            {
                _ps.transform.SetParent(_parent);
            }

            _ps.transform.position = _targetLocation;
            _ps.transform.localScale = Vector3.one * _scaleMultiplier;

            if (_destroyAfterPlaying)
            {
                ParticleSystem.MainModule _psMain = _ps.main;
                _psMain.startColor = _mainColor;
                _psMain.stopAction = ParticleSystemStopAction.Destroy;
            }
            
            _ps.Play();
            return _ps;
        }
        
        private static ParticleSystem PlayParticlesAt(ParticleSystem _target, Vector3 _targetLocation, Transform _parent, Action _stopCallback)
        {
            ParticleSystem _ps = Instantiate(_target, _targetLocation, Quaternion.identity);
            if (_parent)
            {
                _ps.transform.SetParent(_parent);
            }

            if (_stopCallback != null)
            {
                ParticlesStopCallback stopCallbackComponent = _ps.gameObject.AddComponent<ParticlesStopCallback>();
                stopCallbackComponent.AddStopCallback(_stopCallback);
                
                ParticleSystem.MainModule _psMain = _ps.main;
                _psMain.stopAction = ParticleSystemStopAction.Callback;
            }
            
            _ps.Play();
            return _ps;
        }
    }
}
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utility
{
    public class FloatingObject : MonoBehaviour
    {
        [SerializeField] private bool floatOnIdle = true;
        [SerializeField] private FloatSettings floatSettings;
        [SerializeField] private float initDelay;
        
        private const float PERLIN_NOISE_SCALE_X = 100f;
        private const float PERLIN_NOISE_SCALE_Y = 200f;
        
        private Vector3 initLocalPos;
        private float currentDelay;
        private float amplitude;
        private float speed;
        
        private void Awake()
        {
            if(!floatSettings.FloatXAxis && !floatSettings.FloatYAxis)
            {
                Debug.Log("FloatingObject: Both X and Y axes are disabled for floating. Disabling the component...");
                enabled = false;
            }
        }

        private void Start()
        {
            Begin();
        }

        [ContextMenu("Begin Floating")]
        public void Begin()
        {
            initLocalPos = transform.localPosition;
            amplitude = floatSettings.GenerateAmplitude();
            speed = floatSettings.GenerateSpeed();
            enabled = true;
        }

        private void Update()
        {
            if (!floatOnIdle)
            {
                return;
            }

            if (currentDelay < initDelay)
            {
                currentDelay += Time.deltaTime;
                return;
            }
            
            Vector3 _pos = initLocalPos;
            float _xNoise = Mathf.PerlinNoise(Time.time * speed, PERLIN_NOISE_SCALE_X);
            float _yNoise = Mathf.PerlinNoise(Time.time * speed, PERLIN_NOISE_SCALE_Y);
            float _xOffset = _xNoise * 2f - 1f;
            float _yOffset = _yNoise * 2f - 1f;
    
            if (floatSettings.FloatXAxis)
            {
                _pos.x += _xOffset * amplitude;
            }

            if (floatSettings.FloatYAxis)
            {
                _pos.y += _yOffset * amplitude;
            }

            transform.localPosition = _pos;
        }
        
        [Serializable]
        private class FloatSettings
        {
            [SerializeField] private float minAmplitude = 1f;
            [SerializeField] private float maxAmplitude = 1f;
            [SerializeField] private float minSpeed = 1f;
            [SerializeField] private float maxSpeed = 1f;
            [SerializeField] private bool floatXAxis = true;
            [SerializeField] private bool floatYAxis = true;

            public bool FloatXAxis => floatXAxis;
            
            public bool FloatYAxis => floatYAxis;
            
            public float GenerateAmplitude()
            {
                return maxAmplitude - minAmplitude == 0 ? minAmplitude : Random.Range(minAmplitude, maxAmplitude);
            }

            public float GenerateSpeed()
            {
                return maxSpeed - minSpeed == 0 ? minSpeed : Random.Range(minSpeed, maxSpeed);
            }
        }
    }
}
using UnityEngine;
using Utility.Extensions;

namespace Utility
{
    public class FullScreenSprite : MonoBehaviour
    {
        [Header("Right click --> Fit Screen")]
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private bool expandHorizontally;
        [SerializeField] private bool fitScreenOnEnable = true;
        [SerializeField] private bool resizeOnDimensionsChange = true;
        
        [Header("If 0 = current camera's size")]
        [SerializeField] private float targetOrthographicSize;

        private static Camera mainCam;
        
        private static Camera MainCamera => mainCam ? mainCam : mainCam = Camera.main;
        
        private SpriteRenderer Sr => sr ? sr : sr = GetComponent<SpriteRenderer>();
        
        private void OnEnable()
        {
            if (fitScreenOnEnable)
            {
                FitScreen();
            }

            if (resizeOnDimensionsChange)
            {
                DimensionsChangeListener.OnDimensionsChanged += FitScreen;
            }
        }

        private void OnDisable()
        {
            if (resizeOnDimensionsChange)
            {
                DimensionsChangeListener.OnDimensionsChanged -= FitScreen;
            }
        }

        public void SetSprite(Sprite _sprite, bool _expandHorizontally = false)
        {
            expandHorizontally = _expandHorizontally;
            Sr.sprite = _sprite;
            FitScreen();
        }
        
        [ContextMenu("Fit Screen")]
        public void FitScreen()
        {
            if (!Sr || !Sr.sprite || !MainCamera)
            {
                return;
            }
            
            float _orthSize = targetOrthographicSize == 0 ? MainCamera.orthographicSize : targetOrthographicSize;
            float _worldSpriteHeight = Sr.sprite.bounds.size.y;
            float _worldSpriteWidth = Sr.sprite.bounds.size.x;
            float _worldScreenHeight = _orthSize * 2.0f;

            float _screenHeight = ScreenExtensions.Height;
            float _screenWidth = ScreenExtensions.Width;
            float _worldScreenWidth = _worldScreenHeight / _screenHeight * _screenWidth;
            float _spriteAspect = _worldSpriteWidth / _worldSpriteHeight;
            float _screenAspect = _worldScreenWidth / _worldScreenHeight;
            Vector3 _newScale = Vector3.one;

            if (expandHorizontally)
            {
                _newScale.x = _worldScreenWidth / _worldSpriteWidth;
                _newScale.y = _worldScreenHeight / _worldSpriteHeight;
            }
            else
            {
                if (_screenAspect < _spriteAspect)
                {
                    // scale sprite to match top/bottom
                    _newScale.y = _worldScreenHeight / _worldSpriteHeight;
                    _newScale.x = _newScale.y;
                }
                else
                {
                    // scale sprite to match left/right
                    _newScale.x = _worldScreenWidth / _worldSpriteWidth;
                    _newScale.y = _newScale.x;
                }
            }
            transform.localScale = _newScale;
        }
    }
}
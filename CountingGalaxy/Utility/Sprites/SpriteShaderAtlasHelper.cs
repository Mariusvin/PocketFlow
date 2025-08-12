using UnityEngine;

namespace Utility.Sprites
{
    /// <summary>
    /// This script automatically finds a sprite's UV boundaries within an atlas
    /// and passes them to a shader with the that uses sprites UVs.
    /// This makes the sprite shader compatible with sprite atlases.
    /// To make it work you must have a shader that uses the properties "_SpriteUvMin" and "_SpriteUvMax":
    /// [HideInInspector] _SpriteUvMin ("Sprite UV Min", Vector) = (0,0,0,0)
    /// [HideInInspector] _SpriteUvMax ("Sprite UV Max", Vector) = (1,1,0,0)
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteShaderAtlasHelper : MonoBehaviour
    {
        [SerializeField] private bool checkOnUpdate;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private MaterialPropertyBlock propertyBlock;

        private static readonly int UV_MIN_PROP = Shader.PropertyToID("_SpriteUvMin");
        private static readonly int UV_MAX_PROP = Shader.PropertyToID("_SpriteUvMax");

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
            ApplyUV();
        }

        private void LateUpdate()
        {
            if (!checkOnUpdate)
            {
                return;
            }
        
            ApplyUV();
        }

        public void ApplyUV()
        {
            if (!spriteRenderer || !spriteRenderer.sprite)
            {
                Debug.Log("Cannot apply UVs: SpriteRenderer or sprite is not set.");
                return;
            }
            
            spriteRenderer.GetPropertyBlock(propertyBlock);
            Vector2[] _uvz = spriteRenderer.sprite.uv;
            if (_uvz == null || _uvz.Length == 0)
            {
                return;
            }

            Vector2 _min = _uvz[0];
            Vector2 _max = _uvz[0];
            for (int i = 1; i < _uvz.Length; i++)
            {
                _min = Vector2.Min(_min, _uvz[i]);
                _max = Vector2.Max(_max, _uvz[i]);
            }

            propertyBlock.SetVector(UV_MIN_PROP, new Vector4(_min.x, _min.y, 0, 0));
            propertyBlock.SetVector(UV_MAX_PROP, new Vector4(_max.x, _max.y, 0, 0));
            spriteRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    public static class SpriteToAnimation
    {
        private const float FRAME_RATE = 6f;
        
        [MenuItem("Assets/Sprites to Animations")]
        private static void CreateAnimationsFromSprites()
        {
            string[] _guids = Selection.assetGUIDs;
            List<Texture2D> _assets = new();
            foreach (string _guid in _guids)
            {
                GuidToList(_guid, _assets);
            }
 
            if (_assets.Count == 0)
            {
                return;
            }
 
            foreach (Texture2D _asset in _assets)
            {
                CreateAnimation(_asset);
            }
        }
 
        private static void CreateAnimation(Texture2D _texture)
        {
            string _path = AssetDatabase.GetAssetPath(_texture);
            Sprite[] _sprites = AssetDatabase.LoadAllAssetsAtPath(_path).OfType<Sprite>().ToArray();
            if (_sprites.Length == 0)
            {
                return;
            }
 
            // Create animations folder.
            string _animationPath = Path.Combine(Path.GetDirectoryName(_path)!, "Animations");
            if (!Directory.Exists(_animationPath))
            {
                Directory.CreateDirectory(_animationPath);
            }
 
            // Create animation asset.
            AnimationClip _clip = new();
 
            // Create binding curve.
            EditorCurveBinding _curveBinding = new()
            {
                path = "",
                propertyName = "m_Sprite",
                type = typeof(SpriteRenderer)
            };

            // Create keyframes.
            List<ObjectReferenceKeyframe> _objectReferences = new();
            for (int i = 0; i <= _sprites.Length; i++)
            {
                // Allows adding a hold frame to make sure the last sprite frame doesn't get cut off too soon.
                int _index = i;
                if (_index >= _sprites.Length)
                {
                    _index = _sprites.Length - 1;
                }
 
                Sprite _sprite = _sprites[_index];
                ObjectReferenceKeyframe _newKeyframe = new()
                {
                    value = _sprite,
                    time = i * (1f / FRAME_RATE)
                };

                _objectReferences.Add(_newKeyframe);
            }
            AnimationUtility.SetObjectReferenceCurve(_clip, _curveBinding, _objectReferences.ToArray());
 
            // Save.
            AssetDatabase.CreateAsset(_clip, Path.Combine(_animationPath, _texture.name + ".anim"));
            Debug.Log("Created animation: " + _texture.name + ".anim in " + _animationPath);
        }
 
        private static void GuidToList(string _guid, List<Texture2D> _assetList)
        {
            string _path = AssetDatabase.GUIDToAssetPath(_guid);
            Object _asset = AssetDatabase.LoadAssetAtPath<Object>(_path);
            if (_asset == null)
            {
                return;
            }
 
            if (_asset is Texture2D _texture)
            {
                _assetList.Add(_texture);
            }
        }
    }
}
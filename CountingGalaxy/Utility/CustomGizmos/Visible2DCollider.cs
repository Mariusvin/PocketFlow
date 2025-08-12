#if UNITY_EDITOR
using UnityEngine;

namespace Utility.CustomGizmos
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Collider2D))]
    public class Visible2DCollider : MonoBehaviour
    {
        [SerializeField] private Color gizmoColor = Color.yellow;
        [SerializeField] private float lineThickness = 0.02f;

        private Collider2D collider2d;

        private bool TryValidate()
        {
            if (!collider2d)
            {
                collider2d = GetComponent<Collider2D>();
            }

            return collider2d;
        }
        
        private void OnDrawGizmos()
        {
            if (!TryValidate())
            {
                return;
            }
            
            DrawCollider(collider2d);
        }

        private void DrawCollider(Collider2D _collider)
        {
            Gizmos.color = gizmoColor;

            switch (_collider)
            {
                case BoxCollider2D _boxCol2D:
                    DrawBoxCollider(_boxCol2D);
                    break;
                case CircleCollider2D _circCol2D:
                    DrawCircleCollider(_circCol2D);
                    break;
                case PolygonCollider2D _polCol2D:
                    DrawPolygonCollider(_polCol2D);
                    break;
            }
        }

        private void DrawBoxCollider(BoxCollider2D _collider)
        {
            Vector3 _position = transform.position;
            Vector3 _size = new(_collider.size.x, _collider.size.y, 1f);
            Quaternion _rotation = transform.rotation;

            Gizmos.matrix = Matrix4x4.TRS(_position, _rotation, _size);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        private void DrawCircleCollider(CircleCollider2D _collider)
        {
            Vector3 _position = transform.position;
            float _radius = _collider.radius;

            Gizmos.matrix = Matrix4x4.TRS(_position, Quaternion.identity, Vector3.one * _radius * 2f);
            Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
        }

        private void DrawPolygonCollider(PolygonCollider2D _collider)
        {
            // Draw each path in the polygon collider
            for (int _i = 0; _i < _collider.pathCount; _i++)
            {
                Vector2[] _path = _collider.GetPath(_i);
                for (int _j = 0; _j < _path.Length; _j++)
                {
                    int _k = (_j + 1) % _path.Length;
                    DrawLine(_path[_j], _path[_k]);
                }
            }
        }

        private void DrawLine(Vector2 _p1, Vector2 _p2)
        {
            Vector2 _dir = _p2 - _p1;
            float _length = _dir.magnitude;
            float _width = lineThickness * _length;
            Vector3 _center = (_p1 + _p2) * 0.5f;
            Quaternion _rotation = Quaternion.LookRotation(Vector3.forward, _dir);
            Gizmos.matrix = Matrix4x4.TRS(_center, _rotation, new Vector3(_width, _length, 1f));
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}
#endif
using System.Collections.Generic;
using UnityEngine;

namespace Utility.Optimization
{
    [AddComponentMenu("2D Collider Optimization/ Polygon Collider Optimizer")]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class PolygonColliderOptimizer : MonoBehaviour
    {
        [SerializeField] [Range(0f, 1f)] private float tolerance;
        private PolygonCollider2D coll;
        private readonly List<List<Vector2>> originalPaths = new();

        private void OnValidate()
        {
            if (coll == null)
            {
                coll = GetComponent<PolygonCollider2D>();
                for (int i = 0; i < coll.pathCount; i++)
                {
                    List<Vector2> _path = new(coll.GetPath(i));
                    originalPaths.Add(_path);
                }
            }

            if (tolerance <= 0)
            {
                for (int i = 0; i < originalPaths.Count; i++)
                {
                    List<Vector2> _path = originalPaths[i];
                    coll.SetPath(i, _path.ToArray());
                }

                return;
            }

            for (int i = 0; i < originalPaths.Count; i++)
            {
                List<Vector2> _path = originalPaths[i];
                _path = ShapeOptimizationHelper.DouglasPeuckerReduction(_path, tolerance);
                coll.SetPath(i, _path.ToArray());
            }
        }
    }
}
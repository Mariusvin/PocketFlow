using System.Collections.Generic;
using UnityEngine;

namespace Activities.Shared.Restaurant
{
    public static class ObjectsDistributor
    {
        private const int MAX_SCATTER_ATTEMPTS = 100;
        private const float MAX_ROTATION = 360f;
        private const float MIN_SCATTER_DIST = 0.5f;
        private const float SORT_Z_OFFSET = -0.001f;
        
        private static readonly List<Vector3> USED_POSITIONS = new();
        private static float GoldenAngle => Mathf.PI * (3f - Mathf.Sqrt(5f)); // The golden angle in radians
        
        /// <summary>
        /// Scatters the objects in the given area 2D rectangle area
        /// </summary>
        /// <param name="_objects"> Objects to scatter </param>
        /// <param name="_topLeft"> Bounds Top Left corner </param>
        /// <param name="_bottomRight"> Bounds Bottom Right corner </param>
        /// <param name="_minScatterDist"> Minimum distance between items. Default 0.5f </param>
        /// <param name="_maxRotation"> Adds random rotation during the scatter. 0 = rotation disabled </param>
        /// <param name="_sortZOffset"> Adds an offset to each item on Z axis to avoid Z fighting. 0 = keep Z axis original </param>
        public static void ScatterObjectsInRectangle(List<Transform> _objects, Vector3 _topLeft, Vector3 _bottomRight, float _minScatterDist = MIN_SCATTER_DIST, float _maxRotation = MAX_ROTATION, float _sortZOffset = SORT_Z_OFFSET)
        {
            foreach (Transform _object in _objects)
            {
                Vector3 _newPosition = _object.position;
                for (int _attempts = 0; _attempts < MAX_SCATTER_ATTEMPTS; _attempts++) // Prevent infinite loop. Try to scatter the ingredients without overlapping
                {
                    _newPosition = new Vector3(
                        Random.Range(_topLeft.x, _bottomRight.x),
                        Random.Range(_topLeft.y, _bottomRight.y),
                          _newPosition.z);

                    if (IsPositionValid(_newPosition, _minScatterDist))
                    {
                        _newPosition.z += USED_POSITIONS.Count * _sortZOffset;
                        break;
                    }
                }

                USED_POSITIONS.Add(_newPosition);
                _object.position = _newPosition;
                _object.rotation = Quaternion.Euler(0, 0, Random.Range(0, _maxRotation));
            }
            
            CleanUp();
        }
        
        /// <summary>
        /// Scatters the objects in the given area 2D circle area
        /// </summary>
        /// <param name="_objects"> Objects to scatter </param>
        /// <param name="_center"> Center point of the circle area </param>
        /// <param name="_radius"> Distance from the center to spread </param>
        /// <param name="_minScatterDist"> Minimum distance between items. Default 0.5f </param>
        /// <param name="_maxRotation"> Adds random rotation during the scatter. 0 = rotation disabled </param>
        /// <param name="_sortZOffset"> Adds an offset to each item on Z axis to avoid Z fighting. 0 = keep Z axis original </param>
        public static void ScatterObjectsInCircle(List<Transform> _objects, Vector3 _center, float _radius, float _minScatterDist = MIN_SCATTER_DIST, float _maxRotation = MAX_ROTATION, float _sortZOffset = SORT_Z_OFFSET)
        {
            foreach (Transform _object in _objects)
            {
                Vector3 _newPosition = _object.position;

                for (int _attempts = 0; _attempts < MAX_SCATTER_ATTEMPTS; _attempts++)
                {
                    float _angle = Random.Range(0f, Mathf.PI * 2f);
                    float _distance = Mathf.Sqrt(Random.Range(0f, 1f)) * _radius;

                    _newPosition = new Vector3(
                        _center.x + Mathf.Cos(_angle) * _distance,
                        _center.y + Mathf.Sin(_angle) * _distance,
                        _newPosition.z
                    );

                    if (IsPositionValid(_newPosition, _minScatterDist))
                    {
                        _newPosition.z += USED_POSITIONS.Count * _sortZOffset;
                        break;
                    }
                }

                USED_POSITIONS.Add(_newPosition);
                _object.position = _newPosition;
                _object.rotation = Quaternion.Euler(0, 0, Random.Range(0, _maxRotation));
            }

            CleanUp();
        }

        /// <summary>
        /// Distributes given transforms in a natural-looking 2D spiral around a center point
        /// </summary>
        public static void DistributeInSunflowerSpiral(List<Transform> _objects, Vector3 _center, Vector2 _xMinMax, Vector2 _yMinMax, float _jitter = 0.2f)
        {
            int _count = _objects.Count;
            for (int i = 0; i < _count; i++)
            {
                float _theta = i * GoldenAngle;
                float _rootNormalized = Mathf.Sqrt(i) / Mathf.Sqrt(_count);
                float _mappedX = _xMinMax.x + (_xMinMax.y - _xMinMax.x) * _rootNormalized;
                float _mappedY = _yMinMax.x + (_yMinMax.y - _yMinMax.x) * _rootNormalized;
                _mappedX += Random.Range(-_jitter, _jitter);
                _mappedY += Random.Range(-_jitter, _jitter);
                
                float _newX = _center.x + _mappedX * Mathf.Cos(_theta);
                float _newY = _center.y + _mappedY * Mathf.Sin(_theta);
                Vector3 _newPos = new(_newX, _newY, _center.z);
                _objects[i].position = _newPos;
            }
        }
        
        private static bool IsPositionValid(Vector3 _position, float _minScatterDist)
        {
            foreach (Vector3 _usedPosition in USED_POSITIONS)
            {
                if(Vector3.SqrMagnitude(_position - _usedPosition) < _minScatterDist * _minScatterDist)
                {
                    return false;
                }
            }

            return true;
        }
        
        private static void CleanUp()
        {
            USED_POSITIONS.Clear();
        }
    }
}

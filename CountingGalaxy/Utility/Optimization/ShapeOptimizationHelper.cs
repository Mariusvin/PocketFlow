using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility.Optimization
{
    public static class ShapeOptimizationHelper
    {
        public static List<Vector2> DouglasPeuckerReduction(List<Vector2> _points, double _tolerance)
        {
            if (_points == null || _points.Count < 3)
            {
                return _points;
            }

            int lastPoint = _points.Count - 1;
            List<int> pointIndexesToKeep = new()
            {
                //Add the first and last index to the keepers
                0,
                lastPoint
            };

            //The first and the last point cannot be the same
            while (_points[0].Equals(_points[lastPoint]))
            {
                lastPoint--;
            }

            DouglasPeuckerReductionRecursive(_points, 0, lastPoint, _tolerance, ref pointIndexesToKeep);
            pointIndexesToKeep.Sort();
            return pointIndexesToKeep.Select(_index => _points[_index]).ToList();
        }

        private static void DouglasPeuckerReductionRecursive(List<Vector2> _points, int _firstPoint, int _lastPoint, double _tolerance, ref List<int> _pointIndexsToKeep)
        {
            while (true)
            {
                double maxDistance = 0;
                int indexFarthest = 0;

                for (int index = _firstPoint; index < _lastPoint; index++)
                {
                    double distance = PerpendicularDistance(_points[_firstPoint], _points[_lastPoint], _points[index]);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        indexFarthest = index;
                    }
                }

                if (maxDistance > _tolerance && indexFarthest != 0)
                {
                    //Add the largest point that exceeds the tolerance
                    _pointIndexsToKeep.Add(indexFarthest);
                    DouglasPeuckerReductionRecursive(_points, _firstPoint, indexFarthest, _tolerance, ref _pointIndexsToKeep);
                    _firstPoint = indexFarthest;
                    continue;
                }

                break;
            }
        }

        public static double PerpendicularDistance(Vector2 _point1, Vector2 _point2, Vector2 _point)
        {
            double area = Math.Abs(.5f * (_point1.x * _point2.y + _point2.x *
                _point.y + _point.x * _point1.y - _point2.x * _point1.y - _point.x *
                _point2.y - _point1.x * _point.y));
            double bottom = Math.Sqrt(Mathf.Pow(_point1.x - _point2.x, 2f) + Math.Pow(_point1.y - _point2.y, 2f));
            double height = area / bottom * 2f;
            return height;
        }
    }
}
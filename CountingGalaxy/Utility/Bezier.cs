using UnityEngine;

namespace Utility
{
    public struct BezierControlPoints
    {
        public Vector3 controlPoint1;
        public Vector3 controlPoint2;
    }
    
    public static class Bezier
    {
        /// <summary>
        /// Calculates two control points for an ASYMMETRIC S-shaped Bezier curve by adding randomness to each control point's offset.
        /// </summary>
        /// <param name="_startPoint">The starting position of the curve.</param>
        /// <param name="_endPoint">The final position of the curve.</param>
        /// <param name="_curveStrength">The base width of the curve. A value of 0.3 is a good start.</param>
        /// <param name="_randomnessFactor">How much to vary the curve's symmetry. 0 is perfectly symmetrical, 0.4 is noticeably random.</param>
        /// <returns>A struct containing the two calculated control points.</returns>
        public static BezierControlPoints GetAsymmetricSCurveControlPoints(Vector3 _startPoint, Vector3 _endPoint, float _curveStrength = 0.3f, float _randomnessFactor = 0.4f)
        {
            Vector3 _direction = _endPoint - _startPoint;
            Vector3 _perpendicular = new Vector3(-_direction.y, _direction.x, 0).normalized;
            float _baseOffset = _direction.magnitude * _curveStrength;
            float _randomOffset1 = _baseOffset * (1f + Random.Range(-_randomnessFactor, _randomnessFactor));
            float _randomOffset2 = _baseOffset * (1f + Random.Range(-_randomnessFactor, _randomnessFactor));
            Vector3 _point1 = _startPoint + _direction * 0.25f + _perpendicular * _randomOffset1;
            Vector3 _point2 = _startPoint + _direction * 0.75f - _perpendicular * _randomOffset2;
            return new BezierControlPoints { controlPoint1 = _point1, controlPoint2 = _point2 };
        }
        
        public static Vector2 EvaluateCubic(Vector2 _a, Vector2 _b, Vector2 _c, Vector2 _d, float _t1)
        {
            float _t = Mathf.Clamp01(_t1);
            float _t2 = 1f - _t;

            float _a1X = _a.x * _t2 + _b.x * _t;
            float _a1Y = _a.y * _t2 + _b.y * _t;
            float _b1X = _b.x * _t2 + _c.x * _t;
            float _b1Y = _b.y * _t2 + _c.y * _t;
            float _f1X = _a1X * _t2 + _b1X * _t;
            float _f1Y = _a1Y * _t2 + _b1Y * _t;

            float _a2X = _b.x * _t2 + _c.x * _t;
            float _a2Y = _b.y * _t2 + _c.y * _t;
            float _b2X = _c.x * _t2 + _d.x * _t;
            float _b2Y = _c.y * _t2 + _d.y * _t;
            float _f2X = _a2X * _t2 + _b2X * _t;
            float _f2Y = _a2Y * _t2 + _b2Y * _t;

            return new Vector2(_f1X * _t2 + _f2X * _t, _f1Y * _t2 + _f2Y * _t);
        }

        public static Vector3 EvaluateCubic3D(Vector3 _a, Vector3 _b, Vector3 _c, Vector3 _d, float _t1)
        {
            float _t = Mathf.Clamp01(_t1);
            float _t2 = 1f - _t;

            float _a1X = _a.x * _t2 + _b.x * _t;
            float _a1Y = _a.y * _t2 + _b.y * _t;
            float _a1Z = _a.z * _t2 + _b.z * _t;
            float _b1X = _b.x * _t2 + _c.x * _t;
            float _b1Y = _b.y * _t2 + _c.y * _t;
            float _b1Z = _b.z * _t2 + _c.z * _t;
            float _f1X = _a1X * _t2 + _b1X * _t;
            float _f1Y = _a1Y * _t2 + _b1Y * _t;
            float _f1Z = _a1Z * _t2 + _b1Z * _t;

            float _a2X = _b.x * _t2 + _c.x * _t;
            float _a2Y = _b.y * _t2 + _c.y * _t;
            float _a2Z = _b.z * _t2 + _c.z * _t;
            float _b2X = _c.x * _t2 + _d.x * _t;
            float _b2Y = _c.y * _t2 + _d.y * _t;
            float _b2Z = _c.z * _t2 + _d.z * _t;
            float _f2X = _a2X * _t2 + _b2X * _t;
            float _f2Y = _a2Y * _t2 + _b2Y * _t;
            float _f2Z = _a2Z * _t2 + _b2Z * _t;

            return new Vector3(_f1X * _t2 + _f2X * _t, _f1Y * _t2 + _f2Y * _t, _f1Z * _t2 + _f2Z * _t);
        }

        public static Vector2 EvaluateQuadratic(Vector2 _a, Vector2 _b, Vector2 _c, float _t1)
        {
            float _t = Mathf.Clamp01(_t1);
            float _t2 = 1f - _t;

            float _x1 = _t2 * _t2 * _a.x;
            float _x2 = 2f * _t2 * _t * _b.x;
            float _x3 = _t * _t * _c.x;

            float _y1 = _t2 * _t2 * _a.y;
            float _y2 = 2f * _t2 * _t * _b.y;
            float _y3 = _t * _t * _c.y;

            return new Vector2(_x1 + _x2 + _x3, _y1 + _y2 + _y3);
        }

        public static Vector3 EvaluateQuadratic3D(Vector3 _a, Vector3 _b, Vector3 _c, float _t1)
        {
            float _t = Mathf.Clamp01(_t1);
            float _t2 = 1f - _t;

            float _x1 = _t2 * _t2 * _a.x;
            float _x2 = 2f * _t2 * _t * _b.x;
            float _x3 = _t * _t * _c.x;

            float _y1 = _t2 * _t2 * _a.y;
            float _y2 = 2f * _t2 * _t * _b.y;
            float _y3 = _t * _t * _c.y;

            float _z1 = _t2 * _t2 * _a.z;
            float _z2 = 2f * _t2 * _t * _b.z;
            float _z3 = _t * _t * _c.z;

            return new Vector3(_x1 + _x2 + _x3, _y1 + _y2 + _y3, _z1 + _z2 + _z3);
        }
    }
}
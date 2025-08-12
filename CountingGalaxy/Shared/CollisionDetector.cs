using System;
using UnityEngine;

namespace Activities.Shared
{
    public class CollisionDetector : MonoBehaviour
    {
        public event Action<Collision> OnCollisionDetected;
        public event Action<Collision> OnCollisionLeft;

        private void OnCollisionEnter(Collision _collision)
        {
            OnCollisionDetected?.Invoke(_collision);
        }

        private void OnCollisionExit(Collision _collision)
        {
            OnCollisionLeft?.Invoke(_collision);
        }
    }
}
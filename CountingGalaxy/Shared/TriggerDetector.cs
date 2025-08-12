using System;
using UnityEngine;

namespace Activities.Shared
{
    public class TriggerDetector : MonoBehaviour
    {
        public event Action<Collider> OnColliderDetected;
        public event Action<Collider> OnColliderLeft;

        private void OnTriggerEnter(Collider _collider)
        {
            OnColliderDetected?.Invoke(_collider);
        }

        private void OnTriggerExit(Collider _collider)
        {
            OnColliderLeft?.Invoke(_collider);
        }
    }
}
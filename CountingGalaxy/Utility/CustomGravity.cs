using UnityEngine;

namespace Utility
{
    [RequireComponent(typeof(Rigidbody))]
    public class CustomGravity : MonoBehaviour
    {
        [SerializeField] private float gravityScale = 1.0f;

        private float globalGravity = -9.81f;
        private Rigidbody rb;
        
        [field: SerializeField] public bool IsEnabled { get; set; } = true;

#if UNITY_EDITOR
        private void OnValidate()
        {
            ValidateRB();
        }
#endif
    
        private void OnEnable ()
        {
            ValidateRB();
            globalGravity = Physics.gravity.y;
        }

        private void FixedUpdate ()
        {
            if (!IsEnabled)
            {
                return;
            }
            
            Vector3 _gravity = globalGravity * gravityScale * Vector3.up;
            rb.AddForce(_gravity, ForceMode.Acceleration);
        }

        private void ValidateRB()
        {
            if (!rb)
            {
                rb = GetComponent<Rigidbody>();
            }

            if (!rb)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }

            rb.useGravity = false;
        }
    }
}
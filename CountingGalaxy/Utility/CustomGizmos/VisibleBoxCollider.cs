using UnityEngine;

namespace Utility.CustomGizmos
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BoxCollider))]
    public class VisibleBoxCollider : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private Color color = Color.black;
        
        private BoxCollider boxColl;
        
        private bool TryValidate()
        {
            if (!boxColl)
            {
                boxColl = GetComponent<BoxCollider>();
            }

            return boxColl;
        }

        private void OnDrawGizmos()
        {
            if (!TryValidate())
            {
                return;
            }

            Matrix4x4 _rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = _rotationMatrix;
            Gizmos.color = color;
            Gizmos.DrawWireCube(boxColl.center, boxColl.size);
        }
#endif
    }
}

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Utility.UI
{
    public class InvertedUIMask : Image
    {
        private Material mat;
        
        private static readonly int STENCIL_COMP = Shader.PropertyToID("_StencilComp");
        private static readonly int STENCIL_ID = Shader.PropertyToID("_Stencil");
        private static readonly int STENCIL_WRITE_MASK = Shader.PropertyToID("_StencilWriteMask");
        private static readonly int STENCIL_READ_MASK = Shader.PropertyToID("_StencilReadMask");

        public override Material materialForRendering
        {
            get
            {
                if (mat != null)
                {
                    return mat;
                }

                mat = new Material(base.materialForRendering);
                mat.SetInt(STENCIL_COMP, (int)CompareFunction.NotEqual);
                mat.SetInt(STENCIL_ID, 1);
                mat.SetInt(STENCIL_WRITE_MASK, 0);
                mat.SetInt(STENCIL_READ_MASK, 1);
                return mat;
            }
        }
    }
}
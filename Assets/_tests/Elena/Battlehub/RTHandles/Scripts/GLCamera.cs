using UnityEngine;
using System.Collections;

namespace Battlehub.RTHandles
{
    [ExecuteInEditMode]
    public class GLCamera : MonoBehaviour
    {
        private void OnPostRender()
        { 
            if(GLRenderer.Instance != null)
            {
                GLRenderer.Instance.Draw();
            }
        }

    }
}


using UnityEngine;

public class ClearDepthBuffer : MonoBehaviour {

    private void OnPostRender()
    {
        GL.Clear(true, false, Color.black);
    }
}

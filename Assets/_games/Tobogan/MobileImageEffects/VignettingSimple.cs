using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Simple Vignette and Chromatic Aberration")]

public class VignettingSimple : MonoBehaviour
{

    public float vignetting
    {
        get { return intensity; }
        set { intensity = value; }
    }

    public float intensity = 0.375f;
    public Color color = Color.black;

    public Shader vignetteShader;
    private Material vignetteMaterial;

    protected void Awake()
    {
        vignetteMaterial = new Material(vignetteShader);
    }


    void OnPostRender()
    {
        vignetteMaterial.SetFloat("_Intensity", intensity);
        vignetteMaterial.SetColor("_Color", color);

        GL.PushMatrix();
        vignetteMaterial.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.QUADS);

        GL.TexCoord3(0, 0, 0);
        GL.Vertex3(0, 0, 0);

        GL.TexCoord3(0, 1, 0);
        GL.Vertex3(0, 1, 0);

        GL.TexCoord3(1, 1, 0);
        GL.Vertex3(1, 1, 0);

        GL.TexCoord3(1, 0, 0);
        GL.Vertex3(1, 0, 0);

        GL.End();
        GL.PopMatrix();
    }
}
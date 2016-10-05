using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Simple Vignette and Chromatic Aberration")]

public class VignettingSimple /* And Chromatic Aberration */ : PostEffectsBase
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

    protected override bool CheckResources()
    {
        CheckSupport(false);

        vignetteMaterial = CheckShaderAndCreateMaterial(vignetteShader, vignetteMaterial);

        if (!isSupported)
            ReportAutoDisable();
        return isSupported;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (CheckResources() == false)
        {
            Graphics.Blit(source, destination);
            return;
        }
        
        vignetteMaterial.SetFloat("_Intensity", intensity);
        vignetteMaterial.SetColor("_Color", color);	
        Graphics.Blit(source, destination, vignetteMaterial, 0);

        source.wrapMode = TextureWrapMode.Clamp;
    }
}
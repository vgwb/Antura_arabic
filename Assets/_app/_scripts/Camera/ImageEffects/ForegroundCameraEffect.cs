using UnityEngine;

namespace Antura.CameraEffects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class ForegroundCameraEffect : UnityStandardAssets.ImageEffects.PostEffectsBase
    {
        //public RenderTextureOutput foreground;
        public Shader mergeShader = null;

        [Range(0,1)]
        public float t = 0;
        private Material mergeMaterial = null;


        public override bool CheckResources()
        {
            CheckSupport(false);

            mergeMaterial = CheckShaderAndCreateMaterial(mergeShader, mergeMaterial);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        public void OnDisable()
        {
            if (mergeMaterial)
                DestroyImmediate(mergeMaterial);
        }

        public void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            //mergeMaterial.SetTexture("_Foreground", foreground.output);
            mergeMaterial.SetFloat("_T", 1- Mathf.Pow(1-t, 8));

            Graphics.Blit(source, destination, mergeMaterial, 0);
        }
    }
}

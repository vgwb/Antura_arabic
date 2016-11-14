using UnityEngine;
using System.Collections;

namespace EA4S.ColorTickle
{
    public class TestingFeatures : MonoBehaviour
    {
        public Texture2D tOriginal;
        public TMPro.TMP_Text oTextComp;
        public Material mat;
        [Header("Function stages Output")]
        public Texture2D tSource;
        public RenderTexture rt;
        public Texture2D t2D;
        public Texture2D tAlpha;
       


        // Use this for initialization
        void Start()
        {
            tOriginal = oTextComp.fontMaterial.mainTexture as Texture2D;
            
        }

        // Update is called once per frame
        void Update()
        {

               if(Input.GetKeyUp(KeyCode.R))
                {
                 TestBilt();
                }

        }

        void TestBilt()
        {


            //NEW

            t2D = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);



            rt = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);

            tSource = new Texture2D(tOriginal.width, tOriginal.height, TextureFormat.Alpha8, false);
            tSource.SetPixels(tOriginal.GetPixels());
            tSource.Apply();


            string matName = oTextComp.fontSharedMaterial.name;
            string shaderName = oTextComp.fontSharedMaterial.shader.name;

            Material smat = oTextComp.fontSharedMaterial;

#if UNITY_EDITOR
            //float ztprop = smat.GetFloat(TMPro.ShaderUtilities.ShaderTag_ZTestMode);
            int propcount = UnityEditor.ShaderUtil.GetPropertyCount(smat.shader);

            string[] allprop = new string[propcount];
            for (int i = 0; i < propcount; ++i) {
                allprop[i] = UnityEditor.ShaderUtil.GetPropertyName(smat.shader, i);
            }
#endif


            //smat.SetFloat(TMPro.ShaderUtilities.ShaderTag_ZTestMode, 4);



            /*Graphics.Blit(tSource, //The source is setted as the shader _MainTex property; this must be the original Atlas 
                rt, //The RenderTexture where the result will be written
                mat  //The material with the shader to use
                );
            */
            Graphics.BlitMultiTap(tSource, //The source is setted as the shader _MainTex property; this must be the original Atlas 
                rt, //The RenderTexture where the result will be written
                mat,  //The material with the shader to use
                oTextComp.textInfo.meshInfo[0].uvs0 //The uv coordinates to ne used by the shader for the quad vertex
                );
            
            RenderTexture.active = rt;

            t2D.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            t2D.Apply();

            tAlpha = new Texture2D(t2D.width, t2D.height, TextureFormat.Alpha8, false);
            tAlpha.SetPixels(t2D.GetPixels());
            tAlpha.Apply();
        }
    }
}

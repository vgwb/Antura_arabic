using UnityEngine;
using System.Collections;
namespace EA4S.ColorTickle
{
    public class ColorFading : MonoBehaviour
    {
        [SerializeField]
        private Texture2D m_tTargetToFade;
        [SerializeField]
        private Material m_oFadingMaterialToApply;
        [SerializeField]
        private Color m_oBaseColor = Color.white;
        [SerializeField]
        private float m_fFadingSpeed = 0.5f;

        private RenderTexture m_tTextureOuput;
        
        void Start()
        {
           
            m_tTextureOuput = new RenderTexture(m_tTargetToFade.width, m_tTargetToFade.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        }

        
        void Update()
        {
            Graphics.Blit(m_tTargetToFade, m_tTextureOuput, m_oFadingMaterialToApply);
            Graphics.CopyTexture(m_tTextureOuput, m_tTargetToFade);
        }
    }
}

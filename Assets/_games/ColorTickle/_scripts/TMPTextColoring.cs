using UnityEngine;
using System.Collections;
using System;

namespace EA4S.ColorTickle
{
    public class TMPTextColoring : MonoBehaviour
    {
        //At this moment the coloring only applies for the first letter in the text, if more letters
        //are present in the text then all of them will have as side effect the same pattern

        #region EXPOSED MEMBERS
        [SerializeField]
        private ColoringParameters m_oBrush; //The brush used to color
        [SerializeField]
        private TMPro.TMP_Text m_oTextMeshObject; //TMP_Text to work with
        [SerializeField]
        private int m_iPixelPerUnit = 5; //The number of pixels to fit in 1 unit
        [SerializeField]
        [Range(0, 100)]
        private int m_iPercentageRequiredToWin = 95; //The target percentage used to determinate if the letter is finished
        [SerializeField]
        private bool m_bEnableColor = true; //Flag used to enable the coloring functions 
        #endregion

        #region PRIVATE MEMBERS
        private Texture2D m_tLetterDynamicTexture; //Generated texture to color by touch
        private Texture2D m_tBaseLetterTexture; //The texture from wich the letter is rendered (likely an atlas with all the alphabet)
        private Texture2D m_tBaseLetterTextureScaledToDynamic; //Generated texture for the single letter scaled to match the one to color
        private RaycastHit m_oRayHit; //Store the data on the last collision
        private MeshCollider m_oLetterMeshCollider; //The mesh used for the letter raycast
        private Vector2[] m_aUVLetterInMainTexture; //The original UVs of the letter's base texture
        private int m_iTotalShapePixels = 0; //The number of pixels in the m_tBaseLetterTextureScaledToDynamic to constitute the letter body
        private int m_iCurrentShapePixelsColored = 0; //The number of pixels on the letter shape colored
        #endregion

        #region EVENTS
        public event Action<bool> OnShapeHit; //event launched upon touching the face/letter
        public event Action OnShapeCompleted; //event launched upon reaching the requested percentage of coverage for the letter
        #endregion

        #region GETTER/SETTER
        public TMPro.TMP_Text textMeshObject
        {
            get { return m_oTextMeshObject; }
            set { m_oTextMeshObject = value; }
        }

        public int pixelPerUnit
        {
            get { return m_iPixelPerUnit; }
            set { m_iPixelPerUnit = value; }
        }

        public int totalShapePixels
        {
            get { return m_iTotalShapePixels; }
        }

        public int currentShapePixelsColored
        {
            get { return m_iCurrentShapePixelsColored; }
        }

        public bool enableColor
        {
            get { return m_bEnableColor; }
            set { m_bEnableColor = value; }
        }
        #endregion

        #region INTERNALS
        void Start()
        {

            if (m_oTextMeshObject.fontMaterial.mainTexture is Texture2D)
            {
                m_tBaseLetterTexture = m_oTextMeshObject.fontMaterial.mainTexture as Texture2D;

            }

            SetupLetterMeshCollider(); //prepare the letter mesh for the raycast

            SetupLetterColorTexture(); //prepare the texture to color upon succesfull raycast

            SetupLetterTexture(); //prepare the scaled letter texture used for 1:1 matching
            
        }

        void Update()
        {
           
            if (Input.GetMouseButton(0)) //On touch 
            {

                Ray _mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition); //Ray with direction camera->screenpoint

                Debug.DrawRay(_mouseRay.origin, _mouseRay.direction * 100, Color.yellow, 10);

                //check for ray collision
                if (m_oLetterMeshCollider.Raycast(_mouseRay,out m_oRayHit, Mathf.Infinity)) //Populate hit data on the letter texture
                {
                    Debug.Log("Hitted " + m_oRayHit.collider.name + " at " + m_oRayHit.textureCoord.x + " ; " + m_oRayHit.textureCoord.y);

                    //Now we find out which color we hitted to check if we are inside the letter outline
                    //To do this we must combine the letter uvs from the main texture (the outer rect) with the uvs of the dynamic texture(a sub rect)
                    Vector2 fullUV = TextureUtilities.CombineSubUV(m_oRayHit.textureCoord, m_aUVLetterInMainTexture[0], m_aUVLetterInMainTexture[1].y - m_aUVLetterInMainTexture[0].y, m_aUVLetterInMainTexture[2].x - m_aUVLetterInMainTexture[1].x);

                    //If we are outside the letter
                    if (m_tBaseLetterTexture.GetPixelBilinear(fullUV.x, fullUV.y).a == 0)
                    {
                    
                        Debug.Log("OUTSIDE!!!Color Hitted " + m_tBaseLetterTexture.GetPixelBilinear(fullUV.x, fullUV.y) + " at coordinates: " + m_oRayHit.textureCoord.x + " " + m_oRayHit.textureCoord.y);

                        if (OnShapeHit != null)
                        {
                            OnShapeHit(false);
                        }
                    }
                    //else color the texture
                    else 
                    {

                        Debug.Log("COLORING!!!");

                        ColorLetterTexturePoint(m_oRayHit.textureCoord); //paint a single brush

                        //CheckLetterCompletation(); //check for letter coverage

                        if (OnShapeHit != null)
                        {
                            OnShapeHit(true);
                        }
                    }

                    //launch event for letter completation
                    if(GetRachedCoverage()*100 >= m_iPercentageRequiredToWin && OnShapeCompleted!=null)
                    {
                        OnShapeCompleted();
                    }
                }
            }
        }
        #endregion

        #region PUBLIC FUNCTIONS
        /// <summary>
        /// Return the current percentage of shape colored.
        /// </summary>
        /// <returns>Percentage between 0 and 1</returns>
        public float GetRachedCoverage()
        {
            return m_iCurrentShapePixelsColored / (float)m_iTotalShapePixels;
        }
        #endregion

        #region PRIVATE FUNCTIONS

        /// <summary>
        /// Checks if the letter has been colored to the indicated target percentage.
        /// This checks each pixel of the colored texture with the relative on the letter one.
        /// Note: pixels processed sequentially, potentially heavy.
        /// </summary>
        /// <returns></returns>
        private bool CheckLetterCompletation()
        {
            int _iCounter = 0;
            Color[] _aColoredMatrix = m_tLetterDynamicTexture.GetPixels(); //Format is ARGB32
            Color[] _aLetterShapeMatrix = m_tBaseLetterTextureScaledToDynamic.GetPixels(); //Format is Alpha8


            for (int idx = 0; idx < _aLetterShapeMatrix.Length; ++idx)
            {
                //increase the counter each time the pixel of the letter is inside (alpha > 0) and the color is not white (only white has grayscale==1)
                _iCounter += Mathf.CeilToInt(_aLetterShapeMatrix[idx].a) * (1 - Mathf.FloorToInt(_aColoredMatrix[idx].grayscale));
            }

            Debug.Log("Completation check: " + _iCounter + "/" + m_iTotalShapePixels);

            if (_iCounter >= m_iTotalShapePixels * m_iPercentageRequiredToWin / 100f)
            {
                Debug.Log("SUCCESS!!! Required number was: " + (m_iTotalShapePixels * m_iPercentageRequiredToWin / 100f));
                return true;
            }

            return false;
        }
       

        #region MEMBERS INITIALIZATIONS

        /// <summary>
        /// Builds and link the mesh used for raycasting
        /// </summary>
        private void SetupLetterMeshCollider()
        {
            //At this moment the mesh of the text isn't rendered yet, so we force it to retrieve vertex data.
            m_oTextMeshObject.ForceMeshUpdate();

            //Make a copy of the text letter's mesh
            Mesh _oMeshColliderStructure = Instantiate<Mesh>(m_oTextMeshObject.textInfo.meshInfo[0].mesh);

            //UV's need to be correctly setted since the original data takes the uv of the specific letter in the alphabet's atlas;
            //the uvs (and vertices eventually) cannot be directly setted, a separate array must be modified and assigned
            m_aUVLetterInMainTexture = _oMeshColliderStructure.uv; //save a copy of these for later
            Vector2[] newUV = new Vector2[_oMeshColliderStructure.uv.Length];
            newUV[0].Set(0, 0);
            newUV[1].Set(0, 1);
            newUV[2].Set(1, 1);
            newUV[3].Set(1, 0);

            _oMeshColliderStructure.uv = newUV;

            _oMeshColliderStructure.RecalculateBounds();

            //link the constructed mesh to the meshcollider (add it at runtime or build a prefab?)
            m_oLetterMeshCollider = m_oTextMeshObject.GetComponent<MeshCollider>();
            m_oLetterMeshCollider.sharedMesh = _oMeshColliderStructure;

        }

        /// <summary>
        /// Builds and link the texture used to color the letter.
        /// </summary>
        private void SetupLetterColorTexture()
        {
            //Generate the texture to color using the letter actual size
            //- Format can be very low, we only need some base color not a full 32 bit but setPixel/Pixels works 
            //  only on ARGB32, RGB24 and Alpha8 texture formats
            Vector3[] _meshVertices = m_oLetterMeshCollider.sharedMesh.vertices;
            m_tLetterDynamicTexture = new Texture2D(
                Mathf.FloorToInt(Mathf.Abs(_meshVertices[0].x - _meshVertices[3].x) * m_iPixelPerUnit),
                Mathf.FloorToInt(Mathf.Abs(_meshVertices[0].y - _meshVertices[1].y) * m_iPixelPerUnit),
                TextureFormat.ARGB32,
                false);
            m_tLetterDynamicTexture.SetPixels(TextureUtilities.FillTextureWithColor(m_tLetterDynamicTexture, Color.white)); //initialiaze it to white
            m_tLetterDynamicTexture.Apply();

            Debug.Log("Dynamic Tex size are " + m_tLetterDynamicTexture.width + "," + m_tLetterDynamicTexture.height);

            //link the dynamic texture to the TextMeshPro Text as the material's face texture 
            m_oTextMeshObject.fontMaterial.SetTexture("_FaceTex", m_tLetterDynamicTexture);
        }

        /// <summary>
        /// The texture to color and the actual portion of the letter texture are most likely different;
        /// use this to precalculate a scaled texture of the letter to match the sizes 1:1 and avoid 
        /// frequent use of GetPixelBilinear() at each frame when accessing letter texture data.
        /// </summary>
        private void SetupLetterTexture()
        {
            //here scale the letter alpha texture to match the size of the dynamic one and having a 1:1 matching
            m_tBaseLetterTextureScaledToDynamic = new Texture2D(m_tLetterDynamicTexture.width, m_tLetterDynamicTexture.height, TextureFormat.Alpha8, false);

            //retrive the letter size and width in pixels on the original texture
            int _iBaseLetterWidth_SingleLetter = Mathf.FloorToInt(Mathf.Abs(m_aUVLetterInMainTexture[0].x - m_aUVLetterInMainTexture[3].x) * m_tBaseLetterTexture.width);
            int _iBaseLetterHeight_SingleLetter = Mathf.FloorToInt(Mathf.Abs(m_aUVLetterInMainTexture[0].y - m_aUVLetterInMainTexture[1].y) * m_tBaseLetterTexture.height);

            //retrive the colors(shape) of the letter
            Color[] _aColorSingleLetter = m_tBaseLetterTexture.GetPixels(Mathf.FloorToInt(m_aUVLetterInMainTexture[0].x * m_tBaseLetterTexture.width), Mathf.FloorToInt(m_aUVLetterInMainTexture[0].y * m_tBaseLetterTexture.height), _iBaseLetterWidth_SingleLetter, _iBaseLetterHeight_SingleLetter);

            //generate a texture with the founded size and colors
            Texture2D _tBaseLetterTexture_SingleLetter = new Texture2D(_iBaseLetterWidth_SingleLetter, _iBaseLetterHeight_SingleLetter, TextureFormat.Alpha8, false);
            _tBaseLetterTexture_SingleLetter.SetPixels(_aColorSingleLetter);
            _tBaseLetterTexture_SingleLetter.Apply();

            //finally scale the texture 
            Color[] _aColorSingleLetterScaled = TextureUtilities.ScaleTexture(_tBaseLetterTexture_SingleLetter, m_tBaseLetterTextureScaledToDynamic.width / (float)_tBaseLetterTexture_SingleLetter.width, m_tBaseLetterTextureScaledToDynamic.height / (float)_tBaseLetterTexture_SingleLetter.height);

            //find out how many of the pixels in the texture compose the letter (alpha!=0)
            m_iTotalShapePixels = 0;
            for (int idx = 0; idx < _aColorSingleLetterScaled.Length; ++idx)
            {
                m_iTotalShapePixels += Mathf.CeilToInt(_aColorSingleLetterScaled[idx].a);
            }

            m_tBaseLetterTextureScaledToDynamic.SetPixels(_aColorSingleLetterScaled);
            m_tBaseLetterTextureScaledToDynamic.Apply();

        }
        #endregion

        /// <summary>
        /// Color the texture with the given brush on the hitted point.
        /// This also checks if the colored pixel is a new one on the letter and increase the counter to the target coverage.
        /// Note: pixels processed sequentially, potentially heavy.
        /// </summary>
        /// <param name="targetUV">Where the brush must be painted</param>
        private void ColorLetterTexturePoint(Vector2 targetUV)
        {

            if(!m_bEnableColor)
            {
                return;
            }

            //Before color with the brush's texture we need to verify that it fits 
            //in the target's bound and eventually split it

            //store pixel touched in the texture
            int _iXPixelTouched = Mathf.FloorToInt(targetUV.x * m_tLetterDynamicTexture.width);
            int _iYPixelTouched = Mathf.FloorToInt(targetUV.y * m_tLetterDynamicTexture.height);

            //extract pixels coordinates of brush limits and clamp to fit the texture bounds
            int _iLBrushBound = Mathf.Clamp(_iXPixelTouched - m_oBrush.brushShapeScaled.width / 2, 0, m_tLetterDynamicTexture.width);
            int _iRBrushBound = Mathf.Clamp(_iXPixelTouched + m_oBrush.brushShapeScaled.width / 2, 0, m_tLetterDynamicTexture.width);
            int _iBBrushBound = Mathf.Clamp(_iYPixelTouched - m_oBrush.brushShapeScaled.height / 2, 0, m_tLetterDynamicTexture.height);
            int _iTBrushBound = Mathf.Clamp(_iYPixelTouched + m_oBrush.brushShapeScaled.height / 2, 0, m_tLetterDynamicTexture.height);

            //brush's dimensions in pixels for this frame
            int _iBrushWidth = _iRBrushBound - _iLBrushBound;
            int _iBrushHeight = _iTBrushBound - _iBBrushBound;

            //offset for the new center of the brush from the base
            int _iXCenterOffset = -(_iXPixelTouched - (_iRBrushBound - _iBrushWidth / 2));
            int _iYCenterOffset = -(_iYPixelTouched - (_iTBrushBound - _iBrushHeight / 2));

            //Retrive the current array of pixels from the texture to color fitting the brush shape and position
            Color[] _aColors_CurrentDynamicTex = m_tLetterDynamicTexture.GetPixels(_iLBrushBound, _iBBrushBound, _iBrushWidth, _iBrushHeight);

            //Retrieve the current array of pixels from the clamped brush's shape
            Color[] _aColors_BrushClampedShape = m_oBrush.brushShapeScaled.GetPixels(
                Mathf.Clamp(m_oBrush.brushShapeScaled.width / 2 + _iXCenterOffset - _iBrushWidth / 2, 0, m_oBrush.brushShapeScaled.width - _iBrushWidth), //for rounding errors this is clamped to fit the bounds
                Mathf.Clamp(m_oBrush.brushShapeScaled.height / 2 + _iYCenterOffset - _iBrushHeight / 2, 0, m_oBrush.brushShapeScaled.height - _iBrushHeight), // --
                _iBrushWidth,
                _iBrushHeight);

            ////Retrieve the current array of pixels from the clamped single letter texture to check target coverage
            Color[] _aColors_SingleLetterTex = m_tBaseLetterTextureScaledToDynamic.GetPixels(_iLBrushBound, _iBBrushBound, _iBrushWidth, _iBrushHeight);


            //Note that these operations can become quite heavy; we are operating on pixels using the cpu, essentially writing a pixel-shader
            //Adjust the parameters of pixels per unit, brush scale and texture dimensions to process a reasonable number of pixels
            //Alternatively delegate this to a coroutine or split the calculations on more frames

            float _fAlphaOver = 0f;
            float _fAlphaBack = 0f;

            for (int idx = 0; idx < _aColors_BrushClampedShape.Length; ++idx)
            {
                //increase the counter of pixels colored if the pixel is inside the letter (alpha>0), the current color is white (grayscale=1), and the pixel is covered by the brush (alpha>0)
                m_iCurrentShapePixelsColored += Mathf.CeilToInt(_aColors_SingleLetterTex[idx].a) * Mathf.FloorToInt(_aColors_CurrentDynamicTex[idx].grayscale) * Mathf.CeilToInt(_aColors_BrushClampedShape[idx].a);

                //Blend brush color with texture
                _fAlphaOver = _aColors_BrushClampedShape[idx].a;
                _fAlphaBack = _aColors_CurrentDynamicTex[idx].a;
                _aColors_BrushClampedShape[idx] = _aColors_BrushClampedShape[idx] * _fAlphaOver + _aColors_CurrentDynamicTex[idx] * _fAlphaBack*(1 - _fAlphaOver);
                _aColors_BrushClampedShape[idx].a = _fAlphaOver + _fAlphaBack*(1 - _fAlphaOver);
            }

            //finally color the texture with the resulting matrix
            m_tLetterDynamicTexture.SetPixels(_iLBrushBound, _iBBrushBound, _iBrushWidth, _iBrushHeight, _aColors_BrushClampedShape);
            m_tLetterDynamicTexture.Apply();

        }

        #endregion

    }
}

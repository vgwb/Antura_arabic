using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

namespace EA4S.ColorTickle
{
    public class TMPTextColoring : MonoBehaviour
    {
        //At this moment the coloring only applies for the first letter in the text, if more letters
        //are present in the text then all of them will have as side effect the same pattern

        enum eHitState
        {
            NO_HIT=0, LETTER_INSIDE_HIT, LETTER_OUTSIDE_HIT, BODY_HIT 
        }


        #region EXPOSED MEMBERS
        [SerializeField]
        private TMPro.TMP_Text m_oTextMeshObject; //TMP_Text to work with
        [SerializeField]
        private MeshCollider m_oBody; //The touchable body of the LL
        [SerializeField]
        private SkinnedMeshRenderer m_oFaceRenderer; //The face that will render the body texture
        [SerializeField]
        private int m_iPixelPerUnit = 5; //The number of pixels to fit in 1 unit
        [SerializeField]
        [Range(0, 100)]
        private int m_iPercentageRequiredToWin = 95; //The target percentage used to determinate if the letter is finished

        [Header("Brush")]

        [SerializeField]
        private Texture2D m_tBrushShape; //The texture defining the brush shape (alpha>0 are the shape)
        [SerializeField]
        private float m_fBrushScaling = 1; //The scaling of the brush texture
        [SerializeField]
        private Color m_oDrawingColor = Color.red; //The color used by the brush

        #endregion

        #region PRIVATE MEMBERS
        private Texture2D m_tScaledBrush; //Generated texture of the scaled brush
        private Texture2D m_tLetterDynamicTexture; //Generated texture to color by touch
        private Texture2D m_tLLBodyTexture; //Generated texture to color on the LL body
        private Texture2D m_tBaseLetterTexture; //The texture from wich the letter is rendered (likely an atlas with all the alphabet)
        private Texture2D m_tBaseLetterTextureScaledToDynamic; //Generated texture for the single letter scaled to match the one to color
        private RaycastHit m_oRayHit; //Store the data on the last collision
        private MeshCollider m_oLetterMeshCollider; //The mesh used for the letter raycast
        private Vector2[] m_aUVLetterInMainTexture; //The original UVs of the letter's base texture
        private int m_iTotalShapePixels = 0; //The number of pixels in the m_tBaseLetterTextureScaledToDynamic to constitute the letter body
        private int m_iCurrentShapePixelsColored = 0; //The number of pixels on the letter shape colored

        private eHitState m_eHitState = eHitState.NO_HIT; //Resulting state of the raycasting after the last update
        private bool m_bEnableColor = true; //Flag used to enable the coloring functions 

        #endregion

        #region EVENTS
        public event Action OnTouchOutside; //event launched upon touching the face/letter
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

        public Texture2D brushShape
        {
            get { return m_tBrushShape; }
            set { m_tBrushShape = value; }
        }

        public float brushScaling
        {
            get { return m_fBrushScaling; }
            set { m_fBrushScaling = value; }
        }

        public Color drawingColor
        {
            get { return m_oDrawingColor; }
        }

        public int totalAlphaPixels
        {
            get { return m_iTotalShapePixels; }
        }

        public int currentAlphaPixelsColored
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

            BuildBrush();

            SetupLetterMeshCollider(); //prepare the letter mesh for the raycast

            SetupLetterColorTexture(); //prepare the texture to color upon succesfull raycast

            SetupLLBodyColorTexture(); //prepare the texture of the body

            SetupLetterTexture(); //prepare the scaled letter texture used for 1:1 matching
            
        }

        void Update()
        {
           
            if (Input.GetMouseButton(0)) //On touch 
            {

                m_eHitState = eHitState.NO_HIT; //initialize to true to avoid launching event when not hitting

                Debug.Log("MousePress");

                Ray _mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition); //Ray with direction camera->screenpoint

                Debug.DrawRay(_mouseRay.origin, _mouseRay.direction * 100, Color.yellow, 10);

                //FIRST CHECK CASE OF LETTER COLLIDER
                if(m_oLetterMeshCollider.Raycast(_mouseRay,out m_oRayHit, Mathf.Infinity)) //Populate hit data on the letter texture
                {
                    Debug.Log("Hitted " + m_oRayHit.collider.name + " at " + m_oRayHit.textureCoord.x + " ; " + m_oRayHit.textureCoord.y);

                    //Now we find out which color we hitted to check if we are inside the letter outline
                    //To do this we must combine the letter uvs from the main texture (the outer rect) with the uvs of the dynamic texture(a sub rect)
                    Vector2 fullUV = CombineSubUV(m_oRayHit.textureCoord, m_aUVLetterInMainTexture[0], m_aUVLetterInMainTexture[1].y - m_aUVLetterInMainTexture[0].y, m_aUVLetterInMainTexture[2].x - m_aUVLetterInMainTexture[1].x);

                    //If we are outside the letter launch event
                    if (m_tBaseLetterTexture.GetPixelBilinear(fullUV.x, fullUV.y).a == 0)
                    {
                        m_eHitState = eHitState.LETTER_OUTSIDE_HIT;

                        Debug.Log("OUTSIDE!!!Color Hitted " + m_tBaseLetterTexture.GetPixelBilinear(fullUV.x, fullUV.y) + " at coordinates: " + m_oRayHit.textureCoord.x + " " + m_oRayHit.textureCoord.y);

                    }
                    //else color the texture
                    else 
                    {
                        m_eHitState = eHitState.LETTER_INSIDE_HIT;

                        Debug.Log("COLORING!!!");

                        ColorLetterTexturePoint(m_oRayHit.textureCoord); //paint a single brush

                        //CheckLetterCompletation(); //check for letter coverage
                    }
                }

                //THEN CHECK CASE OF BODY COLLIDER
                if (m_oBody.Raycast(_mouseRay, out m_oRayHit, Mathf.Infinity))
                {
                    Debug.Log("Hitted " + m_oRayHit.collider.name + " at " + m_oRayHit.textureCoord.x + " ; " + m_oRayHit.textureCoord.y);

                    ColorLLBodyTexturePoint(m_oRayHit.textureCoord);

                    if (m_eHitState==eHitState.NO_HIT)
                    {

                        m_eHitState = eHitState.BODY_HIT;
                        Debug.Log("ON BODY!!!");

                    }
                }
                
                 //Launch event if we didn't hit the letter
                if(m_eHitState==eHitState.BODY_HIT || m_eHitState == eHitState.LETTER_OUTSIDE_HIT)
                {
                    if (OnTouchOutside != null)
                    {
                        OnTouchOutside();
                    }
                }

            }
        }
        #endregion

        #region PUBLIC FUNCTIONS

        /// <summary>
        /// Sets the brush's texture color to the given one.
        /// Alpha is not modified to preserve the brush shape.
        /// </summary>
        /// <param name="newValue">The new color for the brush</param>
        public void SetBrushColor(Color newValue)
        {
            m_oDrawingColor = newValue; //Keep consistency
           
            Color[] _aColorMatrix = m_tScaledBrush.GetPixels();

            Color _alphaKeeper = newValue;

            //Reassign the pixels color but keep the alpha value to preserve the shape
            for (int idx = 0; idx < _aColorMatrix.Length; ++idx)
            {
                _alphaKeeper.a = _aColorMatrix[idx].a;
                _aColorMatrix[idx] = _alphaKeeper;
            }

            m_tScaledBrush.SetPixels(_aColorMatrix);
            m_tScaledBrush.Apply();
        }

        /// <summary>
        /// Builds the brush to color with by using the stored texture, scaling and color.
        /// Must be called to apply any changes on the brush.
        /// </summary>
        public void BuildBrush()
        {

            if(!m_tBrushShape) //if not given, use a default square
            {
                m_tBrushShape = Texture2D.whiteTexture;
            }

            //Generate the scaled texture
            m_tScaledBrush = new Texture2D(
                Mathf.FloorToInt(m_tBrushShape.width * m_fBrushScaling),
                Mathf.FloorToInt(m_tBrushShape.height * m_fBrushScaling),
                TextureFormat.ARGB32,
                false);

            m_tScaledBrush.SetPixels(ScaleTexture(m_tBrushShape, m_fBrushScaling, m_fBrushScaling));

            m_tScaledBrush.Apply();

            SetBrushColor(m_oDrawingColor);

        }
        #endregion

        #region PRIVATE FUNCTIONS

        #region UTILITIES
        /// <summary>
        /// Given the uvs coordinates of a point relative to a rectangle, calculates the resulting 
        /// uvs for a container of that rectangle.
        /// </summary>
        /// <param name="v2CoordinatesOfInnerPoint">Inner point uvs</param>
        /// <param name="v2BottomLeftCornerOfInnerRect">Bottom left corner uvs of inner rectangle relative to the container</param>
        /// <param name="fHeightOfect">Inner rectangle height or distance between bottom-top corners</param>
        /// <param name="fWidthOfRect">Inner rectangle whidth or distance between left-right corners</param>
        /// <returns>UVs coordinates of inner point relative to the container</returns>
        private Vector2 CombineSubUV(Vector2 v2CoordinatesOfInnerPoint, Vector2 v2BottomLeftCornerOfInnerRect, float fHeightOfect, float fWidthOfRect)
        {
            Vector2 finalOuterUV = Vector2.zero;
            finalOuterUV.x = v2BottomLeftCornerOfInnerRect.x + fWidthOfRect * (v2CoordinatesOfInnerPoint.x);
            finalOuterUV.y = v2BottomLeftCornerOfInnerRect.y + fHeightOfect * (v2CoordinatesOfInnerPoint.y);
            return finalOuterUV;
        }

        /// <summary>
        /// Returns the resulting pixels of the given texture after filling all of them with the same color.
        /// Note: pixels processed sequentially, potentially heavy.
        /// </summary>
        /// <param name="source">The original texture to fill</param>
        /// <param name="newValue">The color to apply</param>
        /// <returns>Resulting pixels</returns>
        private Color[] FillTextureWithColor(Texture2D source, Color newValue)
        {
            Color[] resultMatrix = new Color[source.width * source.height];

            for (int idx = 0; idx < resultMatrix.Length; ++idx)
            {
                resultMatrix[idx] = newValue;
            }

            return resultMatrix;
        }


        /// <summary>
        /// Returns the resulting pixels of the given texture after scaling.
        /// Note: pixels processed sequentially, potentially heavy.
        /// </summary>
        /// <param name="source">The original texture to scale</param>
        /// <param name="fXScaling">The scaling factor for the width</param>
        /// <param name="fYScaling">The scaling factor for the width</param>
        /// <returns>Resulting pixels</returns>
        private Color[] ScaleTexture(Texture2D source, float fXScaling, float fYScaling)
        {
            int _iScaledWidth = Mathf.FloorToInt(source.width * fXScaling);
            int _iScaledHeight = Mathf.FloorToInt(source.height * fYScaling);

            float _fXSampleUnit = 1f / _iScaledWidth;
            float _fYSampleUnit = 1f / _iScaledHeight;

            Color[] resultMatrix = new Color[ _iScaledWidth*_iScaledHeight ];

            //Fill the color by extracting the bilinear value in the original texture for each sample
            for (int j = 0; j < _iScaledHeight; ++j)
            {
                for (int i = 0; i < _iScaledWidth; ++i)
                {
                    resultMatrix[i + j * _iScaledWidth] = source.GetPixelBilinear(_fXSampleUnit * i, _fYSampleUnit * j);
                }
            }

            return resultMatrix;
        }

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
        #endregion

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
            m_tLetterDynamicTexture.SetPixels(FillTextureWithColor(m_tLetterDynamicTexture, Color.white)); //initialiaze it to white
            m_tLetterDynamicTexture.Apply();

            Debug.Log("Dynamic Tex size are " + m_tLetterDynamicTexture.width + "," + m_tLetterDynamicTexture.height);

            //link the dynamic texture to the TextMeshPro Text as the material's face texture 
            m_oTextMeshObject.fontMaterial.SetTexture("_FaceTex", m_tLetterDynamicTexture);
        }

        /// <summary>
        /// Builds and link the texture used to color the body.
        /// </summary>
        private void SetupLLBodyColorTexture()
        {
            //Generate the body texture to color using the face actual size
            m_tLLBodyTexture = new Texture2D(
                Mathf.FloorToInt(m_oBody.sharedMesh.bounds.size.x * m_iPixelPerUnit),
                Mathf.FloorToInt(m_oBody.sharedMesh.bounds.size.z * m_iPixelPerUnit),
                TextureFormat.ARGB32,
                false);
            m_tLLBodyTexture.SetPixels(FillTextureWithColor(m_tLLBodyTexture, Color.white)); //initialiaze it to white
            m_tLLBodyTexture.Apply();

            Debug.Log("Body Tex size are " + m_tLLBodyTexture.width + "," + m_tLLBodyTexture.height);

            //link the body texture as the material's main texture
            m_oFaceRenderer.material.SetTexture("_MainTex", m_tLLBodyTexture);
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
            Color[] _aColorSingleLetterScaled = ScaleTexture(_tBaseLetterTexture_SingleLetter, m_tBaseLetterTextureScaledToDynamic.width / (float)_tBaseLetterTexture_SingleLetter.width, m_tBaseLetterTextureScaledToDynamic.height / (float)_tBaseLetterTexture_SingleLetter.height);

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

        #region COLORING FUNCTIONS
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
            int _iLBrushBound = Mathf.Clamp(_iXPixelTouched - m_tScaledBrush.width / 2, 0, m_tLetterDynamicTexture.width);
            int _iRBrushBound = Mathf.Clamp(_iXPixelTouched + m_tScaledBrush.width / 2, 0, m_tLetterDynamicTexture.width);
            int _iBBrushBound = Mathf.Clamp(_iYPixelTouched - m_tScaledBrush.height / 2, 0, m_tLetterDynamicTexture.height);
            int _iTBrushBound = Mathf.Clamp(_iYPixelTouched + m_tScaledBrush.height / 2, 0, m_tLetterDynamicTexture.height);

            //brush's dimensions in pixels for this frame
            int _iBrushWidth = _iRBrushBound - _iLBrushBound;
            int _iBrushHeight = _iTBrushBound - _iBBrushBound;

            //offset for the new center of the brush from the base
            int _iXCenterOffset = -(_iXPixelTouched - (_iRBrushBound - _iBrushWidth / 2));
            int _iYCenterOffset = -(_iYPixelTouched - (_iTBrushBound - _iBrushHeight / 2));

            //Retrive the current array of pixels from the texture to color fitting the brush shape and position
            Color[] _aColors_CurrentDynamicTex = m_tLetterDynamicTexture.GetPixels(_iLBrushBound, _iBBrushBound, _iBrushWidth, _iBrushHeight);

            //Retrieve the current array of pixels from the clamped brush's shape
            Color[] _aColors_BrushClampedShape = m_tScaledBrush.GetPixels(
                Mathf.Clamp(m_tScaledBrush.width / 2 + _iXCenterOffset - _iBrushWidth / 2, 0, m_tScaledBrush.width - _iBrushWidth), //for rounding errors this is clamped to fit the bounds
                Mathf.Clamp(m_tScaledBrush.height / 2 + _iYCenterOffset - _iBrushHeight / 2, 0, m_tScaledBrush.height - _iBrushHeight), // --
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

            Debug.Log("Completation check: " + m_iCurrentShapePixelsColored + "/" + m_iTotalShapePixels);

            if (m_iCurrentShapePixelsColored >= m_iTotalShapePixels * m_iPercentageRequiredToWin / 100f)
            {
                Debug.Log("SUCCESS!!! Required number was: " + (m_iTotalShapePixels * m_iPercentageRequiredToWin / 100f));
             
            }
        }

        /// <summary>
        /// Just like the letter case, but we don't need to check for completation of the shape.
        /// </summary>
        /// <param name="targetUV">Where the brush must be painted</param>
        private void ColorLLBodyTexturePoint(Vector2 targetUV)
        {

            if (!m_bEnableColor)
            {
                return;
            }

            //Before color with the brush's texture we need to verify that it fits 
            //in the target's bound and eventually split it

            //store pixel touched in the texture
            int _iXPixelTouched = Mathf.FloorToInt(targetUV.x * m_tLLBodyTexture.width);
            int _iYPixelTouched = Mathf.FloorToInt(targetUV.y * m_tLLBodyTexture.height);

            //extract pixels coordinates of brush limits and clamp to fit the texture bounds
            int _iLBrushBound = Mathf.Clamp(_iXPixelTouched - m_tScaledBrush.width / 2, 0, m_tLLBodyTexture.width);
            int _iRBrushBound = Mathf.Clamp(_iXPixelTouched + m_tScaledBrush.width / 2, 0, m_tLLBodyTexture.width);
            int _iBBrushBound = Mathf.Clamp(_iYPixelTouched - m_tScaledBrush.height / 2, 0, m_tLLBodyTexture.height);
            int _iTBrushBound = Mathf.Clamp(_iYPixelTouched + m_tScaledBrush.height / 2, 0, m_tLLBodyTexture.height);

            //brush's dimensions in pixels for this frame
            int _iBrushWidth = _iRBrushBound - _iLBrushBound;
            int _iBrushHeight = _iTBrushBound - _iBBrushBound;

            //offset for the new center of the brush from the base
            int _iXCenterOffset = -(_iXPixelTouched - (_iRBrushBound - _iBrushWidth / 2));
            int _iYCenterOffset = -(_iYPixelTouched - (_iTBrushBound - _iBrushHeight / 2));

            //Retrive the current array of pixels from the texture to color fitting the brush shape and position
            Color[] _aColors_CurrentDynamicTex = m_tLLBodyTexture.GetPixels(_iLBrushBound, _iBBrushBound, _iBrushWidth, _iBrushHeight);

            //Retrieve the current array of pixels from the clamped brush's shape
            Color[] _aColors_BrushClampedShape = m_tScaledBrush.GetPixels(
                Mathf.Clamp(m_tScaledBrush.width / 2 + _iXCenterOffset - _iBrushWidth / 2, 0, m_tScaledBrush.width - _iBrushWidth), //for rounding errors this is clamped to fit the bounds
                Mathf.Clamp(m_tScaledBrush.height / 2 + _iYCenterOffset - _iBrushHeight / 2, 0, m_tScaledBrush.height - _iBrushHeight), // --
                _iBrushWidth,
                _iBrushHeight);

            //Note that these operations can become quite heavy; we are operating on pixels using the cpu, essentially writing a pixel-shader
            //Adjust the parameters of pixels per unit, brush scale and texture dimensions to process a reasonable number of pixels
            //Alternatively delegate this to a coroutine or split the calculations on more frames

            float _fAlphaOver = 0f;
            float _fAlphaBack = 0f;

            for (int idx = 0; idx < _aColors_BrushClampedShape.Length; ++idx)
            {
                //Blend brush color with texture
                _fAlphaOver = _aColors_BrushClampedShape[idx].a;
                _fAlphaBack = _aColors_CurrentDynamicTex[idx].a;
                _aColors_BrushClampedShape[idx] = _aColors_BrushClampedShape[idx] * _fAlphaOver + _aColors_CurrentDynamicTex[idx] * _fAlphaBack * (1 - _fAlphaOver);
                _aColors_BrushClampedShape[idx].a = _fAlphaOver + _fAlphaBack * (1 - _fAlphaOver);
            }

            //finally color the texture with the resulting matrix
            m_tLLBodyTexture.SetPixels(_iLBrushBound, _iBBrushBound, _iBrushWidth, _iBrushHeight, _aColors_BrushClampedShape);
            m_tLLBodyTexture.Apply();

        }
        #endregion

        #endregion

    }
}

using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

namespace EA4S.ColorTickle
{
    public class TMPTextColoring : MonoBehaviour
    {
        //At this moment the coloring only applies for the first letter in the text, if more letters
        //are present in the text then all of them will have as side effect 

        #region EXPOSED MEMBERS
        [SerializeField]
        private TMPro.TMP_Text m_oTextMeshObject; //TMP_Text to work with
        [SerializeField]
        private Collider m_oBody; //The touchable body of the letter   
        [SerializeField]
        private Animator m_oAnimator; //The touchable body of the letter   
        [SerializeField]
        private int m_iPixelPerUnit = 5; //The number of pixels to fit in 1 unit

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
        private Texture2D m_tDynamicTexture; //The texture to color by touch
        private Texture2D m_tBaseLetterTexture; //The texture from wich the letter is rendered (likely an atlas with all the alphabet)
        private Texture2D m_tBaseLetterTextureScaledToDynamic; //TO-DO
        private RaycastHit m_oRayHit; //Store the data on the last collision
        private Mesh m_oMeshColliderStructure; //The mesh used for the letter raycast
        private Vector2[] m_aUVLetterInMainTexture; //The original UVs of the letter's base texture
        #endregion

        public event Action OnTouchOutside; //event launched upon touching the face/letter

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
        #endregion

        #region INTERNALS
        void Start()
        {

            if (m_oTextMeshObject.fontMaterial.mainTexture is Texture2D)
            {
                m_tBaseLetterTexture = m_oTextMeshObject.fontMaterial.mainTexture as Texture2D;

            }

            BuildBrush();

            SetupMeshCollider(); //prepare the letter mesh for the raycast

            SetupColorTexture(); //prapare the texture to color upon succesfull raycast

            //SetupLetterTexture(); //prepare the scaled letter texture used for 1:1 matching
            
        }


        void Update()
        {
            if (Input.GetMouseButton(0)) //On touch 
            {
                Debug.Log("MousePress");

                Ray _mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition); //Ray with direction camera->screenpoint

                Debug.DrawRay(_mouseRay.origin, _mouseRay.direction * 100, Color.yellow, 10);

                //when we hit the letter color the texture (maybe add some checks to be sure of having hit the letter rather than others objects) 
                if (Physics.Raycast(_mouseRay, out m_oRayHit)) //Populate hit data on the texture
                {
                    Debug.Log("Hitted "+ m_oRayHit.collider.name + " at " + m_oRayHit.textureCoord.x + " ; " + m_oRayHit.textureCoord.y);

                    //when we are on the letter
                    if (m_oRayHit.collider.gameObject.GetInstanceID() == m_oTextMeshObject.gameObject.GetInstanceID())
                    {

                        //Now we find out which color we hitted to check if we are inside the letter outline
                        //To do this we must combine the letter uvs from the main texture (the outer rect) with the uvs of the dynamic texture(a sub rect)
                        Vector2 fullUV = CombineSubUV(m_oRayHit.textureCoord, m_aUVLetterInMainTexture[0], m_aUVLetterInMainTexture[1].y - m_aUVLetterInMainTexture[0].y, m_aUVLetterInMainTexture[2].x - m_aUVLetterInMainTexture[1].x);

                        //If we are outside the letter launch event
                        if (m_tBaseLetterTexture.GetPixelBilinear(fullUV.x, fullUV.y).a == 0)
                        {
                            Debug.Log("OUTSIDE!!!Color Hitted " + m_tBaseLetterTexture.GetPixelBilinear(fullUV.x, fullUV.y) + " at coordinates: " + m_oRayHit.textureCoord.x + " " + m_oRayHit.textureCoord.y);
                            if (OnTouchOutside != null)
                            {
                                OnTouchOutside();
                            }
                        }
                        //else color the texture
                        else if (m_oAnimator.GetInteger("State") == 0 && !m_oAnimator.IsInTransition(0))
                        {
                            ColorTexturePoint(); //paint a single brush
                        }

                    }

                    //case of touch outside the letter mesh but still on the body, launch event
                    else if (m_oRayHit.collider.GetInstanceID() == m_oBody.GetInstanceID())
                    {
                        Debug.Log("ON BODY!!!");
                        if (OnTouchOutside != null)
                        {
                            OnTouchOutside();
                        }
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
        /// Builds and link the mesh used for raycasting
        /// </summary>
        private void SetupMeshCollider()
        {
            //At this moment the mesh of the text isn't rendered yet, so we force it to retrieve vertex data.
            m_oTextMeshObject.ForceMeshUpdate();

            //Make a copy of the text letter's mesh
            m_oMeshColliderStructure = Instantiate<Mesh>(m_oTextMeshObject.textInfo.meshInfo[0].mesh);

            //UV's need to be correctly setted since the original data takes the uv of the specific letter in the alphabet's atlas;
            //the uvs (and vertices eventually) cannot be directly setted, a separate array must be modified and assigned
            m_aUVLetterInMainTexture = m_oMeshColliderStructure.uv; //save a copy of these for later
            Vector2[] newUV = new Vector2[m_oMeshColliderStructure.uv.Length];
            newUV[0].Set(0, 0);
            newUV[1].Set(0, 1);
            newUV[2].Set(1, 1);
            newUV[3].Set(1, 0);

            m_oMeshColliderStructure.uv = newUV;

            m_oMeshColliderStructure.RecalculateBounds();

            //link the constructed mesh to the meshcollider (add it at runtime or build a prefab?)
            m_oTextMeshObject.GetComponent<MeshCollider>().sharedMesh = m_oMeshColliderStructure;

        }

        /// <summary>
        /// Builds and link the texture used for coloring
        /// </summary>
        private void SetupColorTexture()
        {
            //Generate the texture to color using the letter actual size
            //- Format can be very low, we only need some base color not a full 32 bit but setPixel/Pixels works 
            //  only on ARGB32, RGB24 and Alpha8 texture formats
            m_tDynamicTexture = new Texture2D(
                Mathf.FloorToInt(Mathf.Abs(m_oMeshColliderStructure.vertices[0].x - m_oMeshColliderStructure.vertices[3].x) * m_iPixelPerUnit),
                Mathf.FloorToInt(Mathf.Abs(m_oMeshColliderStructure.vertices[0].y - m_oMeshColliderStructure.vertices[1].y) * m_iPixelPerUnit),
                TextureFormat.ARGB32,
                false);
            m_tDynamicTexture.SetPixels(FillTextureWithColor(m_tDynamicTexture, Color.white)); //initialiaze it to white
            m_tDynamicTexture.Apply();

            Debug.Log("Dynamic Tex size are " + m_tDynamicTexture.width + "," + m_tDynamicTexture.height);

            //link the dynamic texture to the TextMeshPro Text as the material's face texture 
            m_oTextMeshObject.fontMaterial.SetTexture("_FaceTex", m_tDynamicTexture);
        }

        /// <summary>
        /// The texture to color and the actual portion of the letter texture are most likely different;
        /// use this to precalculate a scaled texture of the letter to match the sizes 1:1 and avoid 
        /// frequent use of GetPixelBilinear() at each frame when accessing letter texture data.
        /// </summary>
        private void SetupLetterTexture()
        {
            //here scale the letter alpha texture to match the size of dynamic and having a 1:1 matching
            m_tBaseLetterTextureScaledToDynamic = new Texture2D(m_tDynamicTexture.width, m_tDynamicTexture.height, TextureFormat.Alpha8, false);

            int _iBaseLetterWidth_SingleLetter = Mathf.FloorToInt(Mathf.Abs(m_aUVLetterInMainTexture[0].x - m_aUVLetterInMainTexture[3].x) * m_tBaseLetterTexture.width);
            int _iBaseLetterHeight_SingleLetter = Mathf.FloorToInt(Mathf.Abs(m_aUVLetterInMainTexture[0].y - m_aUVLetterInMainTexture[1].y) * m_tBaseLetterTexture.height);

            Color[] _aColorSingleLetter = m_tBaseLetterTexture.GetPixels(Mathf.FloorToInt(m_aUVLetterInMainTexture[0].x * m_tBaseLetterTexture.width), Mathf.FloorToInt(m_aUVLetterInMainTexture[0].y * m_tBaseLetterTexture.height), _iBaseLetterWidth_SingleLetter, _iBaseLetterHeight_SingleLetter);
            Texture2D m_tBaseLetterTexture_SingleLetter = new Texture2D(_iBaseLetterWidth_SingleLetter, _iBaseLetterHeight_SingleLetter, TextureFormat.Alpha8, false);

            m_tBaseLetterTextureScaledToDynamic.SetPixels(ScaleTexture(m_tBaseLetterTexture_SingleLetter, m_tBaseLetterTextureScaledToDynamic.width / (float)m_tBaseLetterTexture_SingleLetter.width, m_tBaseLetterTextureScaledToDynamic.height / (float)m_tBaseLetterTexture_SingleLetter.height));

        }

        /// <summary>
        /// Color the texture with the given brush on the hitted point.
        /// Note: pixels processed sequentially, potentially heavy.
        /// </summary>
        private void ColorTexturePoint()
        {
            //Before color with the brush's texture we need to verify that it fits 
            //in the target's bound and eventually split it

            //store pixel touched in the texture
            int _iXPixelTouched = Mathf.FloorToInt(m_oRayHit.textureCoord.x * m_tDynamicTexture.width);
            int _iYPixelTouched = Mathf.FloorToInt(m_oRayHit.textureCoord.y * m_tDynamicTexture.height);

            //extract pixels coordinates of brush limits and clamp to fit the texture bounds
            int _iLBrushBound = Mathf.Clamp(_iXPixelTouched - m_tScaledBrush.width / 2, 0, m_tDynamicTexture.width);
            int _iRBrushBound = Mathf.Clamp(_iXPixelTouched + m_tScaledBrush.width / 2, 0, m_tDynamicTexture.width);
            int _iBBrushBound = Mathf.Clamp(_iYPixelTouched - m_tScaledBrush.height / 2, 0, m_tDynamicTexture.height);
            int _iTBrushBound = Mathf.Clamp(_iYPixelTouched + m_tScaledBrush.height / 2, 0, m_tDynamicTexture.height);

            //brush's dimensions in pixels for this frame
            int _iBrushWidth = _iRBrushBound - _iLBrushBound;
            int _iBrushHeight = _iTBrushBound - _iBBrushBound;

            //offset for the new center of the brush from the base
            int _iXCenterOffset = -(_iXPixelTouched - (_iRBrushBound - _iBrushWidth / 2));
            int _iYCenterOffset = -(_iYPixelTouched - (_iTBrushBound - _iBrushHeight / 2));

            //Retrive the current array of pixels from the texture to color fitting the brush shape and position
            Color[] _aColors_CurrentDynamicTex = m_tDynamicTexture.GetPixels(_iLBrushBound, _iBBrushBound, _iBrushWidth, _iBrushHeight);

            //Retrieve the current array of pixels from the clamped brush's shape
            Color[] _aColors_BrushClampedShape = m_tScaledBrush.GetPixels(
                Mathf.Clamp(m_tScaledBrush.width / 2 + _iXCenterOffset - _iBrushWidth / 2, 0, m_tScaledBrush.width - _iBrushWidth), //for rounding errors this is clamped to fit the bounds
                Mathf.Clamp(m_tScaledBrush.height / 2 + _iYCenterOffset - _iBrushHeight / 2, 0, m_tScaledBrush.height - _iBrushHeight), // --
                _iBrushWidth,
                _iBrushHeight);

            ////Retrieve the current array of pixels from the clamped single letter texture to check objective of 95%
            //Color[] _aColors_SingleLetterTex = m_tBaseLetterTextureScaledToDynamic.GetPixels(_iLBrushBound, _iBBrushBound, _iBrushWidth, _iBrushHeight);


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
                _aColors_BrushClampedShape[idx] = _aColors_BrushClampedShape[idx] * _fAlphaOver + _aColors_CurrentDynamicTex[idx] * _fAlphaBack*(1 - _fAlphaOver);
                _aColors_BrushClampedShape[idx].a = _fAlphaOver + _fAlphaBack*(1 - _fAlphaOver);
            }

            //finally color the texture with the resulting matrix
            m_tDynamicTexture.SetPixels(_iLBrushBound, _iBBrushBound, _iBrushWidth, _iBrushHeight, _aColors_BrushClampedShape);
            m_tDynamicTexture.Apply();
        }
        #endregion
    }
}

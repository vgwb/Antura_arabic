using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EA4S.ColorTickle
{
    public class ColorsUIManager : MonoBehaviour
    {

        #region PUBLIC MEMBERS
        [SerializeField]
        private Button m_SampleButton;
        [SerializeField]
        private float m_ButtonSelectedSizeScale = 2.0f;

        [Header("Max Colors = 9")]
        public Color[] m_Colors;
        public event System.Action<Color> SetBrushColor;

        #endregion

        #region PRIVATE MEMBERS

        private Button[] m_Buttons;
        private int m_NumberOfColors;
        private int m_PreviousColor;
        private Vector2 m_DefaultButtonSize;
        private int m_TripleColors = 0;

        #endregion

        #region GETTER/SETTERS

        public Color defaultColor
        {
            get { return m_Buttons[0].image.color; }
        }

        #endregion

        // Use this for initialization
        void Awake()
		{
            m_NumberOfColors = 3;
            m_Buttons = new Button[m_NumberOfColors];
	        
            //float DistBetwButtons = (Screen.height / 2) / m_NumberOfColors;
            //Vector3 buttonStartPosition = new Vector3(Screen.width / 2 - DistBetwButtons, 0, 0);

            float DistBetwButtons = (Camera.main.pixelHeight / 2) / m_NumberOfColors;
            Vector3 buttonStartPosition = new Vector3(Camera.main.pixelWidth / 2 - DistBetwButtons, 0, 0);

            Debug.Log(Camera.main.pixelHeight);

            for (int i = 0; i < m_NumberOfColors; ++i)
            {
                m_Buttons[i] = Instantiate(m_SampleButton);
                m_Buttons[i].transform.SetParent(gameObject.transform);               
                m_Buttons[i].transform.position = gameObject.transform.position;
                m_Buttons[i].transform.position += buttonStartPosition;
                m_Buttons[i].transform.position += new Vector3(0, - DistBetwButtons * i, 0);

                m_Colors[i].a = 255.0f;
                m_Buttons[i].image.color = m_Colors[i];

                int buttonNumber = i;
                m_Buttons[i].onClick.AddListener(delegate { ButtonClick(buttonNumber); });
            }
            m_DefaultButtonSize = m_Buttons[0].image.rectTransform.sizeDelta;
            m_Buttons[0].image.rectTransform.sizeDelta *= m_ButtonSelectedSizeScale;
            m_PreviousColor = 0;
        }

        // Update is called once per frame
        void Update()
        {


        }

        void ButtonClick(int buttonNumber)
        {
            m_Buttons[m_PreviousColor].image.rectTransform.sizeDelta = m_DefaultButtonSize;
            m_PreviousColor = buttonNumber;
            m_Buttons[buttonNumber].image.rectTransform.sizeDelta *= m_ButtonSelectedSizeScale;

            if (SetBrushColor != null)
            {
                SetBrushColor(m_Buttons[buttonNumber].image.color);
            }
        }

        public void ChangeButtonsColor()
        {
            m_TripleColors += 3;
            if (m_TripleColors > 8)
            {
                m_TripleColors = 0;
            }

            for (int i = 0; i < m_NumberOfColors; ++i)
            {
                m_Colors[m_TripleColors + i].a = 255.0f;
                m_Buttons[i].image.color = m_Colors[m_TripleColors + i];
            }

            m_Buttons[m_PreviousColor].image.rectTransform.sizeDelta = m_DefaultButtonSize;
            m_PreviousColor = 0;
            m_Buttons[0].image.rectTransform.sizeDelta *= m_ButtonSelectedSizeScale;
        }


    }
}
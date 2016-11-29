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
		//[SerializeField]
		//private int m_NumberOfButtons = 4;
		[SerializeField]
		private int m_YDefaultResolution = 720;
        [SerializeField]
        private float m_OutlineSize = 1.2f;
        [SerializeField]
        private Color m_OutlineColor = new Color(0, 0, 0, 255);
        [SerializeField]
        private Button m_SamplePercentageButton;
        //[SerializeField]
        //private bool m_EnablePercentageButton = false;

        [Header("Max Colors = Number of Buttons * Rounds")]
        public Color[] m_Colors;
        public event System.Action<Color> SetBrushColor;

        #endregion

        #region PRIVATE MEMBERS

		Button m_PercentageButton;
        Button[] m_Buttons;
        Button m_OutlineButton;
        int m_PreviousColor;
        int m_ColorNumber = 0;
        int m_NumberOfButtons = 4;
        #endregion

        #region GETTER/SETTERS

        public Color defaultColor
        {
            get { return m_Buttons[0].image.color; }
        }

		public Button percentageColoredButton
		{
			get { return m_PercentageButton; }
		}
        #endregion

        // Use this for initialization
        void Awake()
		{
            m_Buttons = new Button[m_NumberOfButtons];	        
            float distBetwButtons = (Screen.height / 2) / m_NumberOfButtons;
            Vector3 buttonStartPosition = new Vector3(Screen.width / 2 - distBetwButtons, 0, 0);
			float buttonSize = Screen.height / (float)m_YDefaultResolution;

            BuildOutlineButton(buttonStartPosition, buttonSize);

            m_PercentageButton = null;
            BuildButtons(buttonStartPosition, distBetwButtons, buttonSize);
        }

        // Update is called once per frame
        void Update()
        {			
        }

        void BuildOutlineButton(Vector3 buttonStartPosition, float buttonSize)
        {
            m_OutlineButton = Instantiate(m_SampleButton);
            m_OutlineButton.transform.SetParent(gameObject.transform);
            m_OutlineButton.transform.position = gameObject.transform.position;
            m_OutlineButton.transform.position += buttonStartPosition;
            m_OutlineButton.transform.position -= Vector3.forward;
            m_OutlineButton.image.rectTransform.sizeDelta *= buttonSize * m_OutlineSize;
            Color newcolor = m_OutlineColor;
            m_OutlineButton.image.color = newcolor;
        }

        void BuildButtons(Vector3 buttonStartPosition, float distBetwButtons, float buttonSize)
        {
            for (int i = 0; i < m_NumberOfButtons; ++i)
            {
                m_Buttons[i] = Instantiate(m_SampleButton);
                m_Buttons[i].transform.SetParent(gameObject.transform);
                m_Buttons[i].transform.position = gameObject.transform.position;
                m_Buttons[i].transform.position += buttonStartPosition;
                m_Buttons[i].transform.position += new Vector3(0, -distBetwButtons * i, 0);
                m_Buttons[i].image.rectTransform.sizeDelta *= buttonSize;

                m_Colors[i].a = 255.0f;
                m_Buttons[i].image.color = m_Colors[i];

                int buttonNumber = i;
                m_Buttons[i].onClick.AddListener(delegate { ButtonClick(buttonNumber); });
            }

            m_ColorNumber = m_NumberOfButtons - 1;

            if (m_PercentageButton)
            {
                m_PercentageButton = Object.Instantiate(m_SamplePercentageButton);
                m_PercentageButton.GetComponentInChildren<Text>().fontSize = (m_PercentageButton.GetComponentInChildren<Text>().fontSize * Mathf.FloorToInt(buttonSize * 100)) / 100;
                m_PercentageButton.transform.SetParent(gameObject.transform);
                m_PercentageButton.transform.position = gameObject.transform.position;
                m_PercentageButton.transform.position += buttonStartPosition;
                m_PercentageButton.transform.position += new Vector3(0, 1.5f * distBetwButtons, 0);
                m_PercentageButton.image.rectTransform.sizeDelta *= buttonSize;
                Debug.Log(m_PercentageButton.GetComponentInChildren<Text>().fontSize);
            }
        }

        void ButtonClick(int buttonNumber)
        {
            m_OutlineButton.transform.position = m_Buttons[buttonNumber].transform.position;

            if (SetBrushColor != null)
            {
                SetBrushColor(m_Buttons[buttonNumber].image.color);
            }
        }

        public void ChangeButtonsColor()
        {
            for (int i = 0; i < m_NumberOfButtons; ++i)
            {
                m_ColorNumber++;
                if (m_ColorNumber >= m_Colors.Length)
                {
                    m_ColorNumber = 0;
                }
                m_Colors[m_ColorNumber].a = 255.0f;
                m_Buttons[i].image.color = m_Colors[m_ColorNumber];
            }
            m_OutlineButton.transform.position = m_Buttons[0].transform.position;
        }




    }
}
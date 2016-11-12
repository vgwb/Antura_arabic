using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EA4S.ColorTickle
{
    public class ColorsUIManager : MonoBehaviour
    {

        #region PUBLIC MEMBERS
		[SerializeField]
		private Button m_SamplePercentageButton;
        [SerializeField]
		private Button m_SampleButton;
		[SerializeField]
		private int m_NumberOfButtons = 4;
		[SerializeField]
		private int m_YDefaultResolution = 720;
		[SerializeField]
		private float m_ButtonSelectedSizeScale = 2.0f;

        [Header("Max Colors = Number of Buttons * Rounds")]
        public Color[] m_Colors;
        public event System.Action<Color> SetBrushColor;

        #endregion

        #region PRIVATE MEMBERS

		Button m_PercentageButton;
        Button[] m_Buttons;
        int m_PreviousColor;
        Vector2 m_DefaultButtonSize;
        int m_TripleColors = 0;

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
            float DistBetwButtons = (Screen.height / 2) / m_NumberOfButtons;
            Vector3 buttonStartPosition = new Vector3(Screen.width / 2 - DistBetwButtons, 0, 0);
			float buttonSize = Screen.height / (float)m_YDefaultResolution;

            for (int i = 0; i < m_NumberOfButtons; ++i)
            {
                m_Buttons[i] = Instantiate(m_SampleButton);
                m_Buttons[i].transform.SetParent(gameObject.transform);               
                m_Buttons[i].transform.position = gameObject.transform.position;
                m_Buttons[i].transform.position += buttonStartPosition;
                m_Buttons[i].transform.position += new Vector3(0, - DistBetwButtons * i, 0);
				m_Buttons[i].image.rectTransform.sizeDelta *= buttonSize;

                m_Colors[i].a = 255.0f;
                m_Buttons[i].image.color = m_Colors[i];

                int buttonNumber = i;
                m_Buttons[i].onClick.AddListener(delegate { ButtonClick(buttonNumber); });
            }

            m_DefaultButtonSize = m_Buttons[0].image.rectTransform.sizeDelta;
            m_Buttons[0].image.rectTransform.sizeDelta *= m_ButtonSelectedSizeScale;
            m_PreviousColor = 0;

			m_PercentageButton = Object.Instantiate (m_SamplePercentageButton);
			m_PercentageButton.transform.SetParent(gameObject.transform);               
			m_PercentageButton.transform.position = gameObject.transform.position;
			m_PercentageButton.transform.position += buttonStartPosition;
			m_PercentageButton.transform.position += new Vector3(0, 1.5f * DistBetwButtons, 0);
			m_PercentageButton.image.rectTransform.sizeDelta *= buttonSize;
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
			m_TripleColors += m_NumberOfButtons;
			if (m_Colors.Length - m_TripleColors < m_TripleColors)
            {
                m_TripleColors = 0;
            }

            for (int i = 0; i < m_NumberOfButtons; ++i)
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
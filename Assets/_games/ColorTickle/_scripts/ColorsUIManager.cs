using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EA4S.ColorTickle
{
    public class ColorsUIManager : MonoBehaviour
    {

        #region PUBLIC MEMBERS

        public Button m_SampleButton;
        public float m_ButtonSizeScale = 2.0f;

        [Header("Max Colors = 10")]
        public Color[] m_Colors;

        public event System.Action<Color> SetBrushColor;

        #endregion

        #region PRIVATE MEMBERS

        private Button[] m_Buttons;
        private int m_NumberOfColors;
        private int m_PreviousColor;
        private Vector2 m_DefaultButtonSize;

        #endregion

        #region GETTER/SETTERS

        public Color DefaultColor
        {
            get { return m_Buttons[0].image.color; }
        }

        #endregion

        // Use this for initialization
        void Awake()
		{
            m_NumberOfColors = m_Colors.Length;
            m_Buttons = new Button[m_NumberOfColors];

			float PanelHeight = gameObject.GetComponent<RectTransform>().rect.height;
			float DistBetwButtons = PanelHeight / m_NumberOfColors;

            for (int i = 0; i < m_NumberOfColors; ++i)
            {
                m_Buttons[i] = Instantiate(m_SampleButton);

                m_Buttons[i].transform.SetParent(gameObject.transform);
                m_Buttons[i].transform.position = gameObject.transform.position;
                m_Buttons[i].transform.position += new Vector3(0, PanelHeight / 2 + (DistBetwButtons / 2) - (DistBetwButtons * (1 + i)), 0);
	
                m_Colors[i].a = 255.0f;
                m_Buttons[i].image.color = m_Colors[i];

                int buttonNumber = i;
                m_Buttons[i].onClick.AddListener(delegate { ButtonClick(buttonNumber); });
            }

            m_DefaultButtonSize = m_Buttons[0].image.rectTransform.sizeDelta;

            m_Buttons[0].image.rectTransform.sizeDelta *= m_ButtonSizeScale;
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
            m_Buttons[buttonNumber].image.rectTransform.sizeDelta *= m_ButtonSizeScale;

            if (SetBrushColor != null)
            {
                SetBrushColor(m_Buttons[buttonNumber].image.color);
            }
        }
    }
}
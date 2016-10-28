using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace EA4S.ColorTickle
{
    public class ColorsUIManager : MonoBehaviour
    {

        #region PUBLIC MEMBERS

        public Button m_SampleButton;
        [Header("Max Colors = 10")]
        public Color[] m_Colors;
        public event System.Action<Color> SetBrushColor;

        #endregion

        #region PRIVATE MEMBERS

        private Button[] m_Buttons;
        private float m_PanelHeight;
        private float m_PanelWidth;
        private float m_DistBetwButtons;
        private int m_NumberOfColors;
        private Color m_SelectedColor;

        #endregion

        public Color SelectedColor
        {
            get { return m_SelectedColor; }
            set { m_SelectedColor = SelectedColor; }
        }

        // Use this for initialization
        void Start()
        {

            m_NumberOfColors = m_Colors.Length;

            Debug.Log(m_NumberOfColors);

            m_Buttons = new Button[m_NumberOfColors];

            m_PanelHeight = gameObject.GetComponent<RectTransform>().rect.height;

            m_DistBetwButtons = m_PanelHeight / m_NumberOfColors;

            //Debug.Log(m_DistBetwButtons);

            for (int i = 0; i < m_NumberOfColors; ++i)
            {
                m_Buttons[i] = Instantiate(m_SampleButton);

                m_Buttons[i].transform.SetParent(gameObject.transform);
                m_Buttons[i].transform.position = gameObject.transform.position;
                m_Buttons[i].transform.position += new Vector3(0, m_PanelHeight / 2 + (m_DistBetwButtons / 2) - (m_DistBetwButtons * (1 + i)), 0);
                m_Colors[i].a = 255.0f;
                m_Buttons[i].image.color = m_Colors[i];

                int buttonNumber = i;
                m_Buttons[i].onClick.AddListener(delegate { ButtonClick(buttonNumber); });
            }
        }

        // Update is called once per frame
        void Update()
        {


        }

        void ButtonClick(int buttonNumber)
        {
            if (SetBrushColor != null)
            {
                SetBrushColor(m_Buttons[buttonNumber].image.color);
            }

            //Debug.Log(m_SelectedColor);
        }
    }
}
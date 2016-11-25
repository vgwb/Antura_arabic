using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using ArabicSupport;

namespace EA4S
{
    public class TextRender : MonoBehaviour
    {
        public string text {
            get { return m_text; }
            set { if (m_text == value) return; m_text = value; updateText(); }
        }
        [SerializeField]
        protected string m_text;

        public bool isTMPro = true;
        public bool isUI;
        public bool isArabic;

        void Awake()
        {
            checkConfiguration();
            updateText();
        }

        public void setText(string _text, bool arabic = false)
        {
            isArabic = arabic;
            text = _text;
        }

        void checkConfiguration()
        {
            if (isTMPro && isUI && isArabic) {
                if (!gameObject.GetComponent<TextMeshProUGUI>().isRightToLeftText) {
                    Debug.LogWarning("TextMeshPro on component " + gameObject.name + " isn't RTL");
                }
            }
        }

        void updateText()
        {
            if (isTMPro) {
                if (isArabic) {
                    if (isUI) {
                        gameObject.GetComponent<TextMeshProUGUI>().text = ArabicAlphabetHelper.PrepareArabicStringForDisplay(m_text);
                    } else {
                        gameObject.GetComponent<TextMeshPro>().text = ArabicAlphabetHelper.PrepareArabicStringForDisplay(m_text);
                    }
                } else {
                    if (isUI) {
                        gameObject.GetComponent<TextMeshProUGUI>().text = m_text;
                    } else {
                        gameObject.GetComponent<TextMeshPro>().text = m_text;
                    }
                }
            } else {
                if (isArabic) {
                    if (isUI) {
                        gameObject.GetComponent<Text>().text = ArabicFixer.Fix(m_text);
                    } else {
                        gameObject.GetComponent<TextMesh>().text = ArabicAlphabetHelper.PrepareArabicStringForDisplay(m_text);
                    }
                } else {
                    if (isUI) {
                        gameObject.GetComponent<Text>().text = m_text;
                    } else {
                        gameObject.GetComponent<TextMesh>().text = m_text;
                    }
                }
            }
        }

        public void SetColor(Color color)
        {
            if (isTMPro) {
                if (isUI) {
                    gameObject.GetComponent<TextMeshProUGUI>().color = color;
                } else {
                    gameObject.GetComponent<TextMeshPro>().color = color;
                }
            }
        }

    }
}
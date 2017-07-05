using Antura.Core;
using Antura.Helpers;
using Antura.MinigamesAPI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// TODO refactor: remove reference to Arabic
namespace Antura.UI
{
    public class TextRender : MonoBehaviour
    {
        public string text
        {
            get { return m_text; }
            set {
                if (m_text == value) return;
                m_text = value;
                updateText();
            }
        }

        public float Alpha
        {
            get {
                if (isTMPro) {
                    if (isUI) {
                        return gameObject.GetComponent<TextMeshProUGUI>().alpha;
                    } else {
                        return gameObject.GetComponent<TextMeshPro>().alpha;
                    }
                }
                return 0;
            }
            set {
                if (isTMPro) {
                    if (isUI) {
                        gameObject.GetComponent<TextMeshProUGUI>().alpha = value;
                    } else {
                        gameObject.GetComponent<TextMeshPro>().alpha = value;
                    }
                }
            }
        }

        [SerializeField]
        protected string m_text;

        public bool isTMPro = true;
        public bool isUI;
        public bool isArabic;
        public bool isEnglishSubtitle;

        public Database.LocalizationDataId LocalizationId;

        void Awake()
        {
            if (isEnglishSubtitle) {
                gameObject.SetActive(AppManager.I.AppSettings.EnglishSubtitles);
            }

            checkConfiguration();

            if (LocalizationId != Database.LocalizationDataId.None) {
                SetSentence(LocalizationId);
            }
            updateText();
        }

        public void SetText(string _text, bool arabic = false)
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

        public void SetTextUnfiltered(string text)
        {
            if (isTMPro) {
                if (isUI) {
                    gameObject.GetComponent<TextMeshProUGUI>().text = text;
                } else {
                    gameObject.GetComponent<TextMeshPro>().text = text;
                }
            } else {
                if (isUI) {
                    gameObject.GetComponent<Text>().text = text;
                } else {
                    gameObject.GetComponent<TextMesh>().text = text;
                }
            }
        }

        private void updateText()
        {
            if (isTMPro) {
                if (isArabic) {
                    if (isUI) {
                        gameObject.GetComponent<TextMeshProUGUI>().text = ArabicAlphabetHelper.ProcessArabicString(m_text);
                    } else {
                        gameObject.GetComponent<TextMeshPro>().text = ArabicAlphabetHelper.ProcessArabicString(m_text);
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
                        gameObject.GetComponent<Text>().text = ArabicAlphabetHelper.ProcessArabicString(m_text);
                    } else {
                        gameObject.GetComponent<TextMesh>().text = ArabicAlphabetHelper.ProcessArabicString(m_text);
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

        public void SetLetterData(ILivingLetterData livingLetterData)
        {
            isArabic = false;

            if (isUI) {
                gameObject.GetComponent<TextMeshProUGUI>().isRightToLeftText = true;
            } else {
                gameObject.GetComponent<TextMeshPro>().isRightToLeftText = true;
            }
            if (livingLetterData.DataType == LivingLetterDataType.Letter) {
                text = livingLetterData.TextForLivingLetter;
            } else if (livingLetterData.DataType == LivingLetterDataType.Word) {
                text = livingLetterData.TextForLivingLetter;
            }
        }

        public void SetSentence(Database.LocalizationDataId sentenceId)
        {
            // Debug.Log("SetSentence " + sentenceId);
            isArabic = true;
            text = LocalizationManager.GetTranslation(sentenceId);
        }
    }
}
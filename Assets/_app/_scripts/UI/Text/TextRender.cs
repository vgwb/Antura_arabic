using EA4S.Core;
using EA4S.Helpers;
using EA4S.MinigamesAPI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// refactor: remove reference to Arabic
namespace EA4S.UI
{
    public class TextRender : MonoBehaviour
    {
        public string text {
            get { return m_text; }
            set { if (m_text == value) return; m_text = value; updateText(); }
        }

        public float Alpha {
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
        public bool AdjustDiacriticPos;

        public Database.LocalizationDataId LocalizationId;

        void Awake()
        {
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
                    if (AdjustDiacriticPos) {
                        AdjustDiacriticPositions();
                    }
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

        void updateText()
        {
            if (isTMPro) {
                if (isArabic) {
                    if (isUI) {
                        gameObject.GetComponent<TextMeshProUGUI>().text = ArabicAlphabetHelper.ProcessArabicString(m_text);
                        if (AdjustDiacriticPos) {
                            AdjustDiacriticPositions();
                        }
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
            Database.LocalizationData row = LocalizationManager.GetLocalizationData(sentenceId);
            isArabic = true;
            text = row.Arabic;
        }

        /// <summary>
        /// The m text component.
        /// </summary>

        TMP_Text m_TextComponent;
        TMP_TextInfo textInfo;
        public int DeltaYPerc;

        void AdjustDiacriticPositions()
        {
            m_TextComponent = gameObject.GetComponent<TMP_Text>();
            m_TextComponent.ForceMeshUpdate();
            textInfo = m_TextComponent.textInfo;

            int characterCount = textInfo.characterCount;

            if (characterCount > 1) {

                for (int i = 0; i < characterCount; i++) {
                    //Debug.Log("CAHR " + characterCount + ": " + TMPro.TMP_TextUtilities.StringToInt(textInfo.characterInfo[characterCount].character.ToString()));
                    Debug.Log("DIACRITIC: " + i
                              //+ "index: " + textInfo.characterInfo[i].index
                              + " char: " + textInfo.characterInfo[i].character.ToString()
                              + " UNICODE: " + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[i].character)
                             );
                }

                int newDeltaY = 0;
                int charPosition = 1;

                if ((ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[0].character) == "0627"
                    && ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[1].character) == "064B") ||
                    (ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[0].character) == "FE8E"
                     && ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[1].character) == "064B")) {
                    newDeltaY = 70;
                }

                if ((ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[0].character) == "0623"
                   && ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[1].character) == "064E") ||
                   (ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[0].character) == "FE84"
                    && ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[1].character) == "064E")) {
                    newDeltaY = 200;
                }

                if (ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[0].character) == "0639"
                    && ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[1].character) == "0650") {
                    newDeltaY = DeltaYPerc;
                }

                if (newDeltaY != 0) {
                    //TMP_CharacterInfo charInfo = textInfo.characterInfo[charPosition];

                    // Cache the vertex data of the text object as the shift is applied to the original position of the characters.
                    TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
                    // Get the index of the material used by the current character.
                    int materialIndex = textInfo.characterInfo[charPosition].materialReferenceIndex;
                    // Get the index of the first vertex used by this text element.
                    int vertexIndex = textInfo.characterInfo[charPosition].vertexIndex;

                    // Get the cached vertices of the mesh used by this text element (character or sprite).
                    Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
                    float dy = (sourceVertices[vertexIndex + 2].y - sourceVertices[vertexIndex + 0].y) * newDeltaY / 100f;
                    Vector3 offset = new Vector3(0f, dy, 0f);

                    Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
                    destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] + offset;
                    destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + offset;
                    destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + offset;
                    destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + offset;

                    m_TextComponent.UpdateVertexData();

                    Debug.Log("DIACRITIC: pos fixed for " + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[1].character) + " by " + newDeltaY + " / " + dy);
                }
            }

        }

    }
}
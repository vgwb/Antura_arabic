using UnityEngine;
using UnityEngine.UI;
using EA4S.Helpers;
using TMPro;

namespace EA4S.UI
{
    public class TextRenderUtility : MonoBehaviour
    {
        TMP_Text m_TextComponent;
        TMP_TextInfo textInfo;

        public float yOffset = 0f;

        public void ShowInfo()
        {
            m_TextComponent = gameObject.GetComponent<TMP_Text>();
            textInfo = m_TextComponent.textInfo;

            int characterCount = textInfo.characterCount;

            if (characterCount > 1) {
                for (int i = 0; i < characterCount; i++) {
                    //Debug.Log("CAHR " + characterCount + ": " + TMPro.TMP_TextUtilities.StringToInt(textInfo.characterInfo[characterCount].character.ToString()));
                    Debug.Log("CHAR: " + i
                              + "index: " + textInfo.characterInfo[i].index
                              + "char: " + textInfo.characterInfo[i].character.ToString()
                              + "UNICODE: " + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[i].character)
                             );
                }
                //textInfo.characterInfo[1].textElement.yOffset += yOffset;
            }


        }
    }
}
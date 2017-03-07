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

        public int yOffset = 10;

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

        public void AdjustDiacriticPositions()
        {
            m_TextComponent = gameObject.GetComponent<TMP_Text>();
            textInfo = m_TextComponent.textInfo;

            int characterCount = textInfo.characterCount;

            if (characterCount > 1) {
                int newYOffset = 0;

                if (ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[0].character) == "0627"
                    && ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[1].character) == "064B") {
                    newYOffset = yOffset;
                }

                if (ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[0].character) == "0623"
                    && ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[1].character) == "064E") {
                    newYOffset = yOffset;
                }

                if (newYOffset != 0) {
                    // textInfo.textComponent.ForceMeshUpdate();
                    textInfo.characterInfo[1].textElement.yOffset = newYOffset;

                    Debug.Log("diacritic pos fixed for " + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[1].character) + " by " + newYOffset);
                }


                for (int i = 0; i < characterCount; i++) {
                    //Debug.Log("CAHR " + characterCount + ": " + TMPro.TMP_TextUtilities.StringToInt(textInfo.characterInfo[characterCount].character.ToString()));
                    Debug.Log("CHAR: " + i
                              //+ "index: " + textInfo.characterInfo[i].index
                              + " char: " + textInfo.characterInfo[i].character.ToString()
                              + " UNICODE: " + ArabicAlphabetHelper.GetHexUnicodeFromChar(textInfo.characterInfo[i].character)
                             );
                }
                //textInfo.characterInfo[1].textElement.yOffset += yOffset;
            }


        }
    }
}
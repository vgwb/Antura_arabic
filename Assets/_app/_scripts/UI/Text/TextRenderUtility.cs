using UnityEngine;
using UnityEngine.UI;
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

            var ObjectStats = "Characters: " + characterCount + "   Words: " + textInfo.wordCount + "   Spaces: " + textInfo.spaceCount + "   Sprites: " + textInfo.spriteCount + "   Links: " + textInfo.linkCount
                      + "\nLines: " + textInfo.lineCount + "   Pages: " + textInfo.pageCount;

            Debug.Log("Text Info: " + ObjectStats);

            if (characterCount > 1) {
                for (int i = 0; i < characterCount; i++) {
                    // Debug.Log("CAHR " + characterCount + ": " + textInfo.characterInfo[characterCount].ToString());


                }
                textInfo.characterInfo[1].textElement.yOffset = yOffset;
            }


        }
    }
}
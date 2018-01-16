using Antura.Database;
using UnityEngine;

namespace Antura.UI
{
    public class ItemDiacriticSymbol : MonoBehaviour
    {
        public TextRender LetterText;
        public TextRender EnglishLetterText;

        public void Init(LetterData letterData = null)
        {
            if (letterData == null) {
                LetterText.SetTextUnfiltered("");
                EnglishLetterText.SetText("");
            } else {
                var isolatedChar = letterData.GetStringForDisplay(LetterForm.Isolated);

                LetterText.SetTextUnfiltered(isolatedChar);
                EnglishLetterText.SetText(letterData.Id);
            }
        }

    }
}
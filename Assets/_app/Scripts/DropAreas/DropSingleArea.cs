using UnityEngine;
using System.Collections;
using Google2u;
using TMPro;

namespace CGL.Antura {
    public class DropSingleArea : MonoBehaviour {

        public TMP_Text LetterLable;
        public LetterData Data;

        public void Init(LetterData _letterData) {
            Data = _letterData;
            LetterLable.text = ArabicAlphabetHelper.GetLetterFromUnicode(Data.Isolated_Unicode);
        }

    }
}

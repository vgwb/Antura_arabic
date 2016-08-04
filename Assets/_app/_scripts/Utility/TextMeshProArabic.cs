using UnityEngine;
using System.Collections;
using TMPro;
using ArabicSupport;

namespace EA4S
{
    public class TextMeshProArabic : MonoBehaviour
    {

        public string text;
        public bool isUI;

        void Start()
        {
            if (isUI) {
                gameObject.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(text, false, false);
            } else {
//                gameObject.GetComponent<TextMeshPro>().text = ArabicFixer.Fix(text, false, false);
                gameObject.GetComponent<TextMeshPro>().text = GenericUtilites.ReverseText(ArabicFixer.Fix(text, false, false));
            }
        }
    }
}
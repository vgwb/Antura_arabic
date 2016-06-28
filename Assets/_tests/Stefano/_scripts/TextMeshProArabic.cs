using UnityEngine;
using System.Collections;
using TMPro;
using ArabicSupport;


// font range: 
// basic latin: 20-7F
// arabic: 600-603,60B-615,61B,61E-61F,621-63A,640-65E,660-6FF,750-76D,FB50-FBB1,FBD3-FBE9,FBFC-FBFF,FC5E-FC62,FD3E-FD3F,FDF2,FDFC,FE80-FEFC


public class TextMeshProArabic : MonoBehaviour
{

    public string text;

    void Start()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = ArabicFixer.Fix(text, false, false);
    }
}

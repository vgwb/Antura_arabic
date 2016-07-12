using UnityEngine;
using System.Collections;
using TMPro;

public class PopupWindowController : MonoBehaviour
{

    public static PopupWindowController I;
    public GameObject TitleGO;
    public GameObject DrawingImageGO;
    public GameObject WordTextGO;
    public GameObject ButtonGO;

    void Start() {
        I = this;
    }
	

}

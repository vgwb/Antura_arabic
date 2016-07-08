using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CGL.Antura;
using TMPro;

public class LetterController : MonoBehaviour
{

    public BalloonController parentBalloon;
    public bool drop;
    public LetterData letter;
    public int associatedPromptIndex;
    public bool isRequired;
    public LetterObject LetterModel;
    public TMP_Text LetterView;

    private Vector3 mousePosition = new Vector3();
    private float cameraDistance;


    void Start()
    {
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        var result = BalloonsGameManager.instance.inputPlane.Raycast(ray, out cameraDistance);
//
//        Debug.Log(result + " " + cameraDistance);
    }

    void Update()
    {
        if (drop)
        {
            transform.Translate(Vector3.down * Time.deltaTime * 50f);
        }

        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }

    public void Init(LetterData _data)
    {
        LetterModel = new LetterObject(_data);
        LetterView.text = ArabicAlphabetHelper.GetLetterFromUnicode(LetterModel.Data.Isolated_Unicode);
    }

    void OnMouseDrag()
    {
//        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//        BalloonsGameManager.instance.inputPlane.Raycast(ray, out cameraDistance);
//        var point = ray.GetPoint(cameraDistance);
//
////        mousePosition = Input.mousePosition;
////        mousePosition.z = point.z;

        mousePosition = Input.mousePosition;
        mousePosition.z = 30f;

        parentBalloon.MoveHorizontally(Camera.main.ScreenToWorldPoint(mousePosition).x);
    }
}

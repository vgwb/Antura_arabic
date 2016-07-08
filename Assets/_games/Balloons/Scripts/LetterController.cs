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
        cameraDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
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
        mousePosition = Input.mousePosition;
        mousePosition.z = cameraDistance;

        parentBalloon.MoveHorizontally(Camera.main.ScreenToWorldPoint(mousePosition).x);
    }
}

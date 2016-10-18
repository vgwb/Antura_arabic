using UnityEngine;
using System.Collections;
using EA4S;
using EA4S.FastCrowd;

[RequireComponent(typeof(FastCrowdLivingLetter))]
[RequireComponent(typeof(LetterObjectView))]
public class FastCrowdDraggableLetter : MonoBehaviour
{
    FastCrowdLivingLetter letter;
    DropAreaWidget currentDropAreaWidget;
    DropSingleArea currentDropArea;

    void Awake()
    {
        letter = GetComponent<FastCrowdLivingLetter>();
    }

    void OnDestroy()
    {

    }

    // Just test
    void OnMouseDown()
    {
        letter.SetCurrentState(letter.HangingState);
        var oldPos = letter.transform.position;
        oldPos.y = 2;

        letter.transform.position = oldPos;
    }

    void OnMouseUp()
    {
        letter.SetCurrentState(letter.FallingState);

        if (currentDropArea != null)
            letter.DropOnArea(currentDropAreaWidget);
    }

    void OnMouseDrag()
    {

    }


    void OnTriggerEnter(Collider other)
    {
        if (letter.GetCurrentState() == letter.HangingState)
        {
            DropSingleArea singleArea = other.GetComponent<DropSingleArea>();
            if (singleArea)
            {
                var dropArea = singleArea.transform.parent.GetComponent<DropAreaWidget>(); // dirty hack

                currentDropAreaWidget = dropArea;
                currentDropArea = singleArea;

                bool matching = dropArea.GetActiveData().Key == GetComponent<LetterObjectView>().Model.Data.Key;

                dropArea.SetMatchingOutline(true, matching);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        DropSingleArea dropArea = other.GetComponent<DropSingleArea>();
        if (dropArea && (dropArea == currentDropArea))
        {
            currentDropAreaWidget.SetMatchingOutline(false, false);

            currentDropAreaWidget = null;
            currentDropArea = null;
        }
    }
}

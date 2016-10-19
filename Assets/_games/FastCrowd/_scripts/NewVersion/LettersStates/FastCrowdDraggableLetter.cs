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

    Vector3 rayOffset;
    bool isDragging = false;
    float currentY = 0;

    void Awake()
    {
        letter = GetComponent<FastCrowdLivingLetter>();
    }

    void OnDestroy()
    {

    }

    public void StartDragging(Vector3 dragOffset)
    {
        letter.SetCurrentState(letter.HangingState);
        var oldPos = letter.transform.position;
        currentY = oldPos.y;
        letter.transform.position = oldPos;
        
        rayOffset = dragOffset;

        isDragging = true;
    }

    public void EndDragging()
    {
        letter.SetCurrentState(letter.FallingState);

        if (currentDropArea != null)
            letter.DropOnArea(currentDropAreaWidget);

        isDragging = false;
    }

    void OnDrag()
    {
        if (letter.GetCurrentState() != letter.HangingState)
            letter.SetCurrentState(letter.HangingState);

        var oldPos = ComputePointedPosition(rayOffset);
        oldPos.y = currentY;

        currentY = Mathf.Lerp(currentY, 2, Time.deltaTime * 10.0f);
        
        letter.transform.position = oldPos;
    }

    Vector3 ComputePointedPosition(Vector3 rayOffset)
    {
        var screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        var o = screenRay.origin;
        o -= rayOffset;
        screenRay.origin = o;

        if (screenRay.direction.y != 0)
        {
            float t = -screenRay.origin.y / screenRay.direction.y;

            Vector3 pos = screenRay.origin + t * screenRay.direction;
            pos.y = 0;

            return pos;
        }

        return Vector3.zero;
    }


    void Update()
    {
        rayOffset.x = Mathf.Lerp(rayOffset.x, 0, Time.deltaTime);
        rayOffset.z = Mathf.Lerp(rayOffset.z, 0, Time.deltaTime);

        if (isDragging)
            OnDrag();
    }

    void OnTriggerEnter(Collider other)
    {
        if (letter.GetCurrentState() == letter.HangingState)
        {
            DropSingleArea singleArea = other.GetComponent<DropSingleArea>();
            if (singleArea)
            {
                var dropArea = singleArea.transform.parent.GetComponent<DropAreaWidget>(); // dirty hack

                if (dropArea == null)
                    return;

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

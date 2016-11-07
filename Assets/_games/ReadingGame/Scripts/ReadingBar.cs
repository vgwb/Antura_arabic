using UnityEngine;
using System.Collections;

public class ReadingBar : MonoBehaviour
{
    public TMPro.TextMeshPro text;

    public RectTransform start;
    public RectTransform end;

    [Range(0, 1)]
    public float currentReading = 0;

    public float endOffset = 2;

    public MagnifingGlass glass;

    void Start()
    {
        Update();

        glass.transform.position = GetGlassWorldPosition();
    }

    void Update ()
    {
        var size = text.GetPreferredValues();

        var oldStartPos = start.localPosition;
        oldStartPos.x = size.x * 0.5f;
        start.localPosition = oldStartPos;

        var oldEndPos = end.localPosition;
        oldEndPos.x = - size.x * 0.5f - endOffset;
        end.localPosition = oldEndPos;

        glass.transform.position = Vector3.Lerp(glass.transform.position, GetGlassWorldPosition(), Time.deltaTime*20);
    }

    public void SetGlassScreenPosition(Vector2 position)
    {
        var startScreen = Camera.main.WorldToScreenPoint(start.position);
        var endScreen = Camera.main.WorldToScreenPoint(end.position);

        var glassScreenSize = Camera.main.WorldToScreenPoint(glass.transform.position + glass.GetSize())
            - Camera.main.WorldToScreenPoint(glass.transform.position);

        if (Mathf.Abs(endScreen.x - position.x) < Mathf.Abs(glassScreenSize.x) / 2)
        {
            position = endScreen;
        }

        currentReading = 1.0f - Mathf.Clamp01((position.x - endScreen.x) / (startScreen.x - endScreen.x));
    }

    public Vector2 GetGlassScreenPosition()
    {
        var startScreen = Camera.main.WorldToScreenPoint(start.position);
        var endScreen = Camera.main.WorldToScreenPoint(end.position);

        return Vector3.Lerp(startScreen, endScreen, currentReading);
    }

    public Vector3 GetGlassWorldPosition()
    {
        return Vector3.Lerp(start.position, end.position, currentReading);
    }
}

using UnityEngine;
using System.Collections;

public class ReadingBar : MonoBehaviour
{
    public TMPro.TextMeshPro text;

    public RectTransform start;
    public RectTransform end;
    public RectTransform endCompleted;

    [Range(0, 1)]
    public float currentReading = 0;

    public float endOffset = 2;

    public MagnifingGlass glass;

    bool active;
    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            active = value;

            glass.gameObject.SetActive(active);
        }

    }

    void Awake()
    {
        Active = false;
    }

    void Start()
    {
        Update();

        glass.transform.position = GetGlassWorldPosition();
        end.gameObject.SetActive(true);
        endCompleted.gameObject.SetActive(false);
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
        endCompleted.localPosition = oldEndPos;

        glass.transform.position = Vector3.Lerp(glass.transform.position, GetGlassWorldPosition(), Time.deltaTime*20);
    }

    public void Complete()
    {
        end.gameObject.SetActive(false);
        endCompleted.gameObject.SetActive(true);
        currentReading = 1;
    }

    public bool SetGlassScreenPosition(Vector2 position)
    {
        var startScreen = Camera.main.WorldToScreenPoint(start.position);
        var endScreen = Camera.main.WorldToScreenPoint(end.position);

        var glassScreenSize = Camera.main.WorldToScreenPoint(glass.transform.position + glass.GetSize())
            - Camera.main.WorldToScreenPoint(glass.transform.position);

        bool completed = false;
        if (Mathf.Abs(endScreen.x - position.x) < Mathf.Abs(glassScreenSize.x) / 2)
        {
            position = endScreen;
            completed = true;
        }

        currentReading = 1.0f - Mathf.Clamp01((position.x - endScreen.x) / (startScreen.x - endScreen.x));

        return completed;
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

using UnityEngine;
using System.Collections;
using System;

public class ReadingBar : MonoBehaviour
{
    public TMPro.TextMeshPro text;

    public RectTransform start;
    public RectTransform end;
    public RectTransform endCompleted;

    public Color clearColor;
    public Color doneColor;

    [Range(0, 1)]
    public float currentReading = 0;

    public float startOffset = 2;
    public float endOffset = 4;

    public MagnifingGlass glass;
    public ThreeSlicesSprite backSprite;

    [Range(0, 1)]
    public float alpha = 0;

    SpriteRenderer[] spriteRenderers;
    Color[] startColors;
    Color startTextColor;

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
            start.GetComponent<SpriteRenderer>().color = doneColor;
        }

    }

    void Awake()
    {
        alpha = 0;
        Active = false;

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        startColors = new Color[spriteRenderers.Length];

        for (int i = 0; i < spriteRenderers.Length; ++i)
            startColors[i] = spriteRenderers[i].color;
        startTextColor = text.color;

        for (int i = 0; i < spriteRenderers.Length; ++i)
        {
            var color = spriteRenderers[i].color;
            color.a = 0;
            spriteRenderers[i].color = color;
        }

        var textColor = text.color;
        textColor.a = 0;
        text.color = textColor;
    }

    void Start()
    {
        Update();

        glass.transform.position = GetGlassWorldPosition();
        end.gameObject.SetActive(true);
        endCompleted.gameObject.SetActive(false);
        start.GetComponent<SpriteRenderer>().color = active ? doneColor : clearColor;
    }

    void Update()
    {
        var size = text.GetPreferredValues();

        var oldStartPos = start.localPosition;
        oldStartPos.x = size.x * 0.5f + startOffset;
        start.localPosition = oldStartPos;

        var oldEndPos = end.localPosition;
        oldEndPos.x = -size.x * 0.5f - endOffset;
        end.localPosition = oldEndPos;
        endCompleted.localPosition = oldEndPos;

        glass.transform.position = Vector3.Lerp(glass.transform.position, GetGlassWorldPosition(), Time.deltaTime * 20);
        float glassPercPos = Vector3.Distance(glass.transform.position, start.position)/Vector3.Distance(start.position, end.position);


        // Set Back Sprite
        var oldPos = backSprite.transform.localPosition;

        oldPos.x = (start.localPosition.x + end.localPosition.x) * 0.5f;
        backSprite.transform.localPosition = oldPos;
        backSprite.donePercentage = 1 - glassPercPos;
        var oldScale = backSprite.transform.localScale;
        oldScale.x = (start.localPosition.x - end.localPosition.x) * 0.25f;
        backSprite.transform.localScale = oldScale;

        const float ALPHA_LERP_SPEED = 5.0f;

        for (int i = 0; i < spriteRenderers.Length; ++i)
        {
            var color = spriteRenderers[i].color;
            color.a = Mathf.Lerp(color.a, startColors[i].a * alpha, ALPHA_LERP_SPEED * Time.deltaTime);
            spriteRenderers[i].color = color;
        }

        var textColor = text.color;
        textColor.a = Mathf.Lerp(textColor.a, startTextColor.a * alpha, ALPHA_LERP_SPEED * Time.deltaTime);
        text.color = textColor;
    }

    public void Complete()
    {
        end.gameObject.SetActive(false);
        endCompleted.gameObject.SetActive(true);
        endCompleted.GetComponent<SpriteRenderer>().color = doneColor;
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

        if (currentReading >= 0.99f)
            completed = true;

        return completed;
    }

    public void Show(bool show)
    {
        alpha = show ? 1 : 0;
       // text.gameObject.SetActive(show);
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

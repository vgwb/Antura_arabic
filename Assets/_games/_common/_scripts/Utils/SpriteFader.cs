using System;
using UnityEngine;

public class SpriteFader : MonoBehaviour
{
    public bool show = true;
    public float fadeSpeed = 2.0f;

    SpriteRenderer sprite;
    float startAlpha;
    float currentAlpha;

    bool overwrittenAlpha = false;
    bool awaken = false;

    void Awake()
    {
        awaken = true;
        sprite = GetComponent<SpriteRenderer>();

        startAlpha = sprite.color.a;
        if (overwrittenAlpha)
        {
            var old = sprite.color;
            old.a = currentAlpha;
            sprite.color = old;
        }
        else
        {
            currentAlpha = startAlpha;
        }
    }

    void Update()
    {
        var old = sprite.color;

        var target = startAlpha * (show ? 1 : 0);

        var distance = target - currentAlpha;

        float delta = fadeSpeed * Time.deltaTime;

        if (target <= 0.001f)
        {
            currentAlpha -= delta;

            if (currentAlpha < 0)
                currentAlpha = 0;
        }
        else
        {
            if (Mathf.Abs(distance) > 0.05f)
                currentAlpha += Mathf.Sign(distance) * delta;
        }

        old.a = currentAlpha;
        sprite.color = old;

        var enabled = currentAlpha > 0.001f;
        if (sprite.enabled != enabled)
            sprite.enabled = enabled;
    }

    public void SetAlphaImmediate(float alpha)
    {
        currentAlpha = alpha;
        overwrittenAlpha = true;
    }
}
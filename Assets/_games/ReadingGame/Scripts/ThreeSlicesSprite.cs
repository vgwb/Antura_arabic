using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ThreeSlicesSprite : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Material material;

    [Range(0, 1)]
    public float donePercentage;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        material = spriteRenderer.material;
    }

    void Update()
    {
        material.SetFloat("_ScreenLeftOffset", Camera.main.WorldToScreenPoint(spriteRenderer.bounds.min).x);
        material.SetFloat("_ScreenRightOffset", Camera.main.WorldToScreenPoint(spriteRenderer.bounds.max).x);
        material.SetFloat("_Done", donePercentage);
    }
}


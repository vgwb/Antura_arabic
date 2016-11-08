using UnityEngine;
using System.Collections;
using System;

public class MagnifingGlass : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public Vector3 GetSize()
    {
        return spriteRenderer.bounds.extents;
    }
}

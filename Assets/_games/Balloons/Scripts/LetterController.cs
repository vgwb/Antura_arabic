using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CGL.Antura;

public class LetterController : MonoBehaviour
{
    public bool drop;
    public LetterData letter;
    public int associatedPromptIndex; 
    public bool isRequired;

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
}

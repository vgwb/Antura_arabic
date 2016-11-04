using UnityEngine;
using System.Collections;

public class GroundController : MonoBehaviour {

    public static GroundController instance;

    void Awake()
    {
        instance = this;
    }
}

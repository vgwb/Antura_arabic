using UnityEngine;
using System;

// refactor: this can be moved to the utilities.
public class OnActiveBehaviour : MonoBehaviour {

    void OnEnable()
    {
        if(OnEnableAction != null)
        {
            OnEnableAction();
        }
    }

    void OnDisable()
    {
        if(OnDisableAction != null)
        {
            OnDisableAction();
        }
    }

    [HideInInspector]
    public Action OnEnableAction;
    public Action OnDisableAction;
}

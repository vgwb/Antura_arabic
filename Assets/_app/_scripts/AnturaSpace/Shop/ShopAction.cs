using Antura.Core;
using UnityEngine;

public class ShopAction : MonoBehaviour
{
    public Sprite iconSprite;
    public int bonesCost;

    public bool IsLocked { get { return locked; } }

    private bool locked = false;

    public virtual void PerformAction()
    {
        // nothing to do here
    }

    public void SetLocked(bool _locked)
    {
        locked = _locked;
    }

    public virtual void InitialiseLockedState()
    {
        if (AppManager.I.Player.GetTotalNumberOfBones() > bonesCost) {
            SetLocked(false);
        } else {
            SetLocked(true);
        }
    }
}
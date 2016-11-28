using UnityEngine;
using System.Collections;

public class MakeFriendsAnturaController : MonoBehaviour
{
    public AnturaAnimationController animationController;
    public Vector3 runDirection;
    public float runSpeed;

    private bool run;


    public void ReactToEndGame()
    {
        animationController.DoCharge(null);
        run = true;
    }

    void FixedUpdate()
    {
        if (run)
        {
            transform.Translate(runDirection * runSpeed);
        }
    }

    public void ReactNegatively()
    {
        animationController.DoShout();
    }

    public void ReactPositively()
    {
        animationController.DoSniff();
    }
}

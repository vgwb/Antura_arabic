using UnityEngine;
using System.Collections;

public class LivingLetterRagdoll : MonoBehaviour
{
    public Animator animator;

    // For Test purposes
    public bool doRagdoll = false;

    bool isRagdoll = false;
    Rigidbody[] rigidBodies;

    void Awake()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>(true);

        for (int i = 0; i < rigidBodies.Length; ++i)
            rigidBodies[i].isKinematic = true;
    }

    void Update()
    {
        if (doRagdoll)
        {
            doRagdoll = false;

            SetRagdoll(true, 10 * Vector3.one);
        }
    }

    public void SetRagdoll(bool active, Vector3 initialVelocity)
    {
        animator.enabled = !active;

        if (active && !isRagdoll)
        {
            for (int i = 0; i < rigidBodies.Length; ++i)
            {
                rigidBodies[i].isKinematic = false;
                rigidBodies[i].velocity = initialVelocity;
            }
        }

        isRagdoll = active;
    }
}

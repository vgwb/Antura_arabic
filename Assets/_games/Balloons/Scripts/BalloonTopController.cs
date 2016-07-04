using UnityEngine;
using System.Collections;

public class BalloonTopController : MonoBehaviour
{
    public BalloonController parent;
    public Collider balloonCollider;

    private Rigidbody body;
    private Animator animator;
    private AudioSource popAudio;
    private int taps = 0;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        popAudio = GetComponent<AudioSource>();
    }

    public void OnMouseDown()
    {
        TapAction();
    }

    void TapAction()
    {
        taps++;
        if (taps >= parent.tapsNeeded)
        {
            Pop();
        }
    }

    public void Pop()
    {
        balloonCollider.enabled = false;
        parent.Pop();
        popAudio.Play();
        animator.SetBool("Pop", true);
    }

    //    void Shrink()
    //    {
    //        transform.localScale = new Vector3(transform.localScale.x - 0.25f, transform.localScale.y - 0.25f, transform.localScale.z - 0.25f);
    //    }
}

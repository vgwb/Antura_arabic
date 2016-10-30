using UnityEngine;
using System.Collections;

public class DropZoneController : MonoBehaviour
{
    public static DropZoneController chosenDropZone;
    public SpriteRenderer spriteRenderer;

    private float THROB_INIT_SCALE;
    private float THROB_SCALE_MULTIPLIER = 1.2f;
    private float THROB_PERIOD = 0.33f;

    private IEnumerator throbAnimation;
    private bool isChosen = false;

    // Use this for initialization
    void Start()
    {
        THROB_INIT_SCALE = transform.localScale.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isChosen && chosenDropZone != this)
        {
            isChosen = false;
            Unhighlight();
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Highlight()
    {
        spriteRenderer.color = Color.yellow;
    }

    public void Unhighlight()
    {
        spriteRenderer.color = Color.white;
    }

    public void OnTriggerEnter(Collider collider)
    {
        Throb();
        isChosen = true;
        chosenDropZone = this;
        Highlight();
    }

    public void OnTriggerExit(Collider collider)
    {
        isChosen = false;
        Unhighlight();
    }

    private void Throb()
    {
        if (throbAnimation != null)
        {
            StopCoroutine(throbAnimation);
        }

        throbAnimation = ThrobCoroutine();
        StartCoroutine(throbAnimation);
    }

    private IEnumerator ThrobCoroutine()
    {
        float throbFinalScale = THROB_INIT_SCALE * THROB_SCALE_MULTIPLIER;
        float throbScaleIncrementPerFixedFrame = ((throbFinalScale - THROB_INIT_SCALE) * Time.fixedDeltaTime) / (THROB_PERIOD * 0.5f);

        Vector3 scale = new Vector3(THROB_INIT_SCALE, THROB_INIT_SCALE, 1);

        transform.localScale = scale;

        while (true)
        {
            scale.x += throbScaleIncrementPerFixedFrame;
            scale.y += throbScaleIncrementPerFixedFrame;
            if (scale.x > throbFinalScale)
            {
                throbScaleIncrementPerFixedFrame *= -1;
            }
            else if (scale.x < THROB_INIT_SCALE)
            {
                transform.localScale = new Vector3(THROB_INIT_SCALE, THROB_INIT_SCALE, 1);
                break;
            }
            transform.localScale = scale;
            yield return new WaitForFixedUpdate();
        }
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}

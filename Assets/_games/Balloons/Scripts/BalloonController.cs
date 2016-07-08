using UnityEngine;
using System.Collections;

public class BalloonController : MonoBehaviour
{
    [Range(0, 10)]
    public float floatSpeed; //e.g. 1f
    [Range(0, 10)]
    public float floatDistance; //e.g. 1.5f
    [Range(0, 1)]
    public float floatRandomness; // e.g. 0.25f
    [Range(0, 10)]
    public float dragSpeed; //e.g. 2f
    [Range(1, 10)]
    public int tapsNeeded; // e.g. 3

    public BalloonTopController balloonTop;
    public GameObject rope;
    public LetterController letter;

    private int floatDirection = 1;
    private Vector3 basePosition;


    void Start()
    {
        basePosition = transform.position;
        RandomizeFloating();
    }

    void Update()
    {
        Float();
    }

    void RandomizeFloating()
    {
        floatSpeed += Random.Range(-floatRandomness * floatSpeed, floatRandomness * floatSpeed);
        floatDistance += Random.Range(-floatRandomness * floatDistance, floatRandomness * floatDistance);
    }

    void Float()
    {
        transform.position = basePosition + floatDistance * Mathf.Sin(floatSpeed * Time.time) * Vector3.up;
        floatDirection *= -1;
    }

    public void MoveHorizontally(float x)
    {
        basePosition.x = Mathf.Lerp(basePosition.x, x, dragSpeed * Time.deltaTime);
    }

    public void Pop()
    {
        rope.SetActive(false);
        if (letter.isRequired)
        {
            BalloonsGameManager.instance.OnPoppedRequired(letter.associatedPromptIndex);
        }
        letter.transform.SetParent(null);
        letter.drop = true;
        BalloonsGameManager.instance.balloons.Remove(this);
        BalloonsGameManager.instance.CheckRemainingBalloons();
        Destroy(gameObject, 3f);
    }

}

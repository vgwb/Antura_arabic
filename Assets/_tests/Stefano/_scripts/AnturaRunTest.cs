using UnityEngine;
using System.Collections;

/// <summary>
/// Creates wandering behaviour for a CharacterController.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class AnturaRunTest : MonoBehaviour
{
    public float speed = 5;
    public float directionChangeInterval = 1;
    public float maxHeadingChange = 30;

    int pointIndex;
    public GameObject[] viewPoint;

    CharacterController controller;
    float heading;
    Vector3 targetRotation;

    void Awake() {
        controller = GetComponent<CharacterController>();

        // Set random initial rotation
        heading = Random.Range(0, 360);
        transform.eulerAngles = new Vector3(0, heading, 0);

        StartCoroutine(NewHeading());
    }

    void Update() {
        transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, targetRotation, Time.deltaTime * directionChangeInterval);
        var forward = transform.TransformDirection(Vector3.forward);
        controller.SimpleMove(forward * speed);
    }

    /// <summary>
    /// Repeatedly calculates a new direction to move towards.
    /// Use this instead of MonoBehaviour.InvokeRepeating so that the interval can be changed at runtime.
    /// </summary>
    IEnumerator NewHeading() {
        while (true) {
            NewHeadingRoutine();
            yield return new WaitForSeconds(directionChangeInterval);
        }
    }

    /// <summary>
    /// Calculates a new direction to move towards.
    /// </summary>
    void NewHeadingRoutine() {
//        var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
//        var ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
//        heading = Random.Range(floor, ceil);
        heading = Mathf.Clamp(heading + 20, 0, 360);
        targetRotation = new Vector3(0, heading, 0);
    }
}
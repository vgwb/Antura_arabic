using UnityEngine;

public class FastCrowdLetterMovement : MonoBehaviour
{
    CharacterController controller;

    bool sweep = true;
    public bool EnableSweep
    {
        get { return sweep; }
        set
        {
            sweep = value;
            controller.enabled = value;
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();

    }
    public void MoveAmount(Vector3 deltaPosition)
    {
        if (EnableSweep)
            controller.Move(deltaPosition);
        else
            transform.position += deltaPosition;
    }

    public void MoveTo(Vector3 position)
    {
        MoveAmount(position - transform.position);
    }

    public void LookAt(Vector3 position)
    {
        LerpLookAt(position, 1);
    }

    public void LerpLookAt(Vector3 position, float t)
    {
        Vector3 targetDir3D = (transform.position - position);
        if (targetDir3D.sqrMagnitude < 0.001f)
            return;

        Vector2 targetDir = new Vector2(targetDir3D.x, targetDir3D.z);
        Vector2 letterDir = new Vector2(transform.forward.x, transform.forward.z);

        targetDir.Normalize();
        letterDir.Normalize();

        var desiredAngle = AngleCounterClockwise(targetDir, Vector2.down);
        var currentAngle = AngleCounterClockwise(letterDir, Vector2.up);

        currentAngle = Mathf.LerpAngle(currentAngle, desiredAngle, t);

        transform.rotation = Quaternion.AngleAxis(currentAngle * Mathf.Rad2Deg, Vector3.up);
    }

    static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    static float AngleCounterClockwise(Vector2 a, Vector2 b)
    {
        float dot = Vector2.Dot(a.normalized, b.normalized);
        dot = Mathf.Clamp(dot, -1.0f, 1.0f);

        if (Cross(a, b) >= 0)
            return Mathf.Acos(dot);
        return Mathf.PI * 2 - Mathf.Acos(dot);
    }
}


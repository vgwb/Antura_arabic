using EA4S.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringController : MonoBehaviour
{
    public Animation springAnimation;

    public Transform innerSpring;

    [Range(-1, 1)]
    public float t;
    public bool Released = false;

    public float Elasticity = 400.0f;
    public float Friction = 1.5f;

    float velocity = 0;

    void Start()
    {
        springAnimation["Base"].normalizedTime = 0.5f;
        springAnimation["Base"].speed = 0.0f;
        springAnimation.Play("Base");

    }

    void Update()
    {
        if (Released)
        {
            //// Simulate elastic movement
            // F = -K * dx
            float elasticForce = -Elasticity * t;

            // F = m * a; we approximate mass to 1 and work on Elasticity to model the graphics feedback
            // F -> 1 * a
            float acceleration = elasticForce - velocity * Friction;

            // V = a * t
            velocity += acceleration * Time.deltaTime;

            // Integrates changes between last timestep
            // h += V * t + 0.5 * a * t^2
            t += velocity * Time.deltaTime + 0.5f * acceleration * Time.deltaTime * Time.deltaTime;
        }
        else
        {
            velocity = 0;

            Transform followElement = null;
            if (followElement != null)
            {
                var direction = followElement.transform.position - transform.position;

                Vector2 planarDirection = new Vector2(direction.x, direction.z);

                var forward = transform.forward;
                Vector2 planarForward = new Vector2(forward.x, forward.z);

                var desiredAngle = MathHelper.AngleCounterClockwise(planarDirection, planarForward) * Mathf.Rad2Deg;
                if (desiredAngle > 180)
                    desiredAngle -= 180; // -180 to 180

                if (desiredAngle > 90 || desiredAngle < -90)
                    desiredAngle += 180;

                innerSpring.transform.localRotation = Quaternion.AngleAxis(180 + desiredAngle, Vector3.up);
            }
        }

        t = Mathf.Clamp(t, -1, 1);

        springAnimation["Base"].normalizedTime = (t + 1) * 0.5f;
    }
}

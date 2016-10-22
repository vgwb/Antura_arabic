using UnityEngine;
using System.Collections;

namespace EA4S.ThrowBalls
{
    public class SlingshotController : MonoBehaviour
    {
        public static SlingshotController instance;

        public GameObject ball;
        public GameObject greenCross;
        public GameObject arc;

        private PokeballController ballController;

        // One of the fixed pivots of the slingshot:
        private Vector3 pivot1;

        // The other fixed pivot:
        private Vector3 pivot2;

        // The center of the slingshot.
        // For normal behavior, it should be the average of both pivots:
        private Vector3 center = new Vector3(0, 6, -20);

        // The range of the projectile's point of impact's Z coordinate after it has been launched:
        private float zLaunchRange = 24;

        // The minimum point of impact's Z coordinate:
        private float minLaunchZ = -4;

        // The range of the projectile's point of impact's Z coordinate after it has been launched:
        private float xLaunchRange = 46;

        // The minimum point of impact's Z coordinate:
        private float minLaunchX = -23;

        // The point of impact, calculated in real-time:
        private Vector3 pointOfImpact;

        // The elasticity. The higher the value, the more the ball travels for a fixed tug at the slingshot.
        private float elasticity = SROptions.Current.Elasticity;

        // The Y velocity of the ball when it is launched at minimum Z:
        private float minYLaunchVelocity = 60;

        // The Y velocity of the ball when it is launched at maximum Z:
        private float maxYLaunchVelocity = 80;

        // The Y coordinate of the point of impact:
        private float pointOfImpactY = 1.1f;

        float peakY = 0;

        void Awake()
        {
            instance = this;
            ballController = ball.GetComponent<PokeballController>();
        }

        void Update()
        {
            if (!ballController.IsLaunched)
            {
                UpdatePointOfImpact();
                UpdateArc();
            }

            else
            {
                if (ballController.transform.position.y > peakY)
                {
                    peakY = ballController.transform.position.y;
                }

                else
                {
                    Debug.Log("peak y is " + peakY);
                }
            }
        }

        private void UpdatePointOfImpact()
        {
            Vector3 pokeballPosition = ball.transform.position;

            float zChargeUp = (center.y - pokeballPosition.y) * elasticity;
            float xChargeUp = (center.x - pokeballPosition.x) * elasticity;

            float pointOfImpactZ = Mathf.Clamp(minLaunchZ + zChargeUp, minLaunchZ, minLaunchZ + zLaunchRange);
            float pointOfImpactX = Mathf.Clamp(center.x + xChargeUp, minLaunchX, minLaunchX + xLaunchRange);

            pointOfImpact = new Vector3(pointOfImpactX, pointOfImpactY, pointOfImpactZ);

            greenCross.transform.position = pointOfImpact;
        }

        private void UpdateArc()
        {
            Vector3 ballPosition = ball.transform.position;
            Vector3 distanceToImpactPoint = pointOfImpact - ballPosition;
            distanceToImpactPoint.y = 0;
            //ballPosition.z = ballPosition.z;

            float hypotheticalYVelocity = (((pointOfImpact.z - minLaunchZ) / (zLaunchRange)) * (maxYLaunchVelocity - minYLaunchVelocity)) + minYLaunchVelocity;
            float hypotheticalYPeak = -0.5f * Mathf.Pow(hypotheticalYVelocity, 2) * Constants.GRAVITY_INVERSE.y + ballPosition.y;

            float xScale = distanceToImpactPoint.magnitude / 2;
            float yScale = 15;
            float zScale = hypotheticalYPeak;

            arc.transform.localScale = new Vector3(xScale, yScale, zScale);

            float yAngle = Vector3.Angle(ballPosition - pointOfImpact, Vector3.right);

            arc.transform.rotation = Quaternion.Euler(90, yAngle, 0);

            Vector3 arcPosition = arc.transform.position;
            arcPosition.x = (pointOfImpact.x + ballPosition.x) / 2;
            arcPosition.z = (pointOfImpact.z + ballPosition.z) / 2;
            arcPosition.z += 2f;
            arc.transform.position = arcPosition;
        }

        public Vector3 GetLaunchForce()
        {
            Vector3 ballPosition = ball.transform.position;

            float yVelocity = (((pointOfImpact.z - minLaunchZ) / (zLaunchRange)) * (maxYLaunchVelocity - minYLaunchVelocity)) + minYLaunchVelocity;

            float velocityFactor = (-1 * yVelocity) - Mathf.Sqrt(Mathf.Pow(yVelocity, 2) - (2 * (ballPosition.y - pointOfImpact.y) * Constants.GRAVITY.y));
            velocityFactor = Mathf.Pow(velocityFactor, -1);
            velocityFactor *= Constants.GRAVITY.y;

            float zVelocity = (pointOfImpact.z - ballPosition.z) * velocityFactor;
            float xVelocity = (pointOfImpact.x - ballPosition.x) * velocityFactor;

            Debug.Log("Launching with a y velocity of " + yVelocity + "at y = " + ballPosition.y);

            return new Vector3(xVelocity, yVelocity, zVelocity);
        }
    }
}
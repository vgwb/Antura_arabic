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

        // The force exerted on the ball at launch, calculated in real-time:
        private Vector3 launchForce;

        // The elasticity. The higher the value, the more the ball travels for a fixed tug at the slingshot.
        private float elasticity = SROptions.Current.Elasticity;

        // The Y velocity of the ball when it is launched at minimum Z:
        private float minYLaunchVelocity = 60;

        // The Y velocity of the ball when it is launched at maximum Z:
        private float maxYLaunchVelocity = 80;

        // The Y coordinate of the point of impact:
        private float pointOfImpactY = 1.1f;

        void Awake()
        {
            instance = this;
            ballController = ball.GetComponent<PokeballController>();
        }

        void FixedUpdate()
        {
            if (!ballController.IsLaunched)
            {
                UpdatePointOfImpact();
                UpdateLaunchForce();
                UpdateArc();
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

        private void UpdateLaunchForce()
        {
            Vector3 ballPosition = ball.transform.position;

            float yVelocity = (((pointOfImpact.z - minLaunchZ) / (zLaunchRange)) * (maxYLaunchVelocity - minYLaunchVelocity)) + minYLaunchVelocity;

            float velocityFactor = (-1 * yVelocity) - Mathf.Sqrt(Mathf.Pow(yVelocity, 2) - (2 * (ballPosition.y - pointOfImpact.y) * Constants.GRAVITY.y));
            velocityFactor = Mathf.Pow(velocityFactor, -1);
            velocityFactor *= Constants.GRAVITY.y;

            float zVelocity = (pointOfImpact.z - ballPosition.z) * velocityFactor;
            float xVelocity = (pointOfImpact.x - ballPosition.x) * velocityFactor;

            launchForce = new Vector3(xVelocity, yVelocity, zVelocity);
        }

        private void UpdateArc()
        {
            Vector3 ballPosition = ball.transform.position;

            Vector3 hypotheticalPeakPosition = new Vector3();
            hypotheticalPeakPosition.y = -0.5f * Mathf.Pow(launchForce.y, 2) * Constants.GRAVITY_INVERSE.y + ballPosition.y;
            hypotheticalPeakPosition.z = (-launchForce.y * Constants.GRAVITY_INVERSE.y) * launchForce.z + ballPosition.z;
            hypotheticalPeakPosition.x = (-launchForce.y * Constants.GRAVITY_INVERSE.y) * launchForce.x + ballPosition.x;

            Vector3 hypotheticalArcStart = new Vector3();
            hypotheticalArcStart.y = pointOfImpact.y;
            hypotheticalArcStart.z = -pointOfImpact.z + 2 * hypotheticalPeakPosition.z;

            // To find the start X of the arc, we need to find the equation of the circle
            // passing through the 3 points: Ball position, peak, and point of impact.

            // We begin by determining the center of the arc. The center is the intersection
            // point of three planes: the bisector plane of a segment formed by two points,
            // the bisector plane of a segment formed by a different pair of points, and
            // the plane formed by the three points themselves.

            Vector3 plane1Normal = hypotheticalPeakPosition - ballPosition;
            float plane1CstFactor = plane1Normal.x * (hypotheticalPeakPosition.x + ballPosition.x) / 2
                                        + plane1Normal.y * (hypotheticalPeakPosition.y + ballPosition.y) / 2
                                              + plane1Normal.z * (hypotheticalPeakPosition.z + ballPosition.z) / 2;

            Vector3 plane2Normal = hypotheticalPeakPosition - pointOfImpact;
            float plane2CstFactor = plane2Normal.x * (hypotheticalPeakPosition.x + pointOfImpact.x) / 2
                                        + plane2Normal.y * (hypotheticalPeakPosition.y + pointOfImpact.y) / 2
                                              + plane2Normal.z * (hypotheticalPeakPosition.z + pointOfImpact.z) / 2;

            Vector3 plane3Normal = Vector3.Cross(hypotheticalPeakPosition - ballPosition, ballPosition - pointOfImpact);
            float plane3CstFactor = plane3Normal.x * (pointOfImpact.x)
                                        + plane3Normal.y * (pointOfImpact.y)
                                              + plane3Normal.z * (pointOfImpact.z);

            Vector3 arcCenter = (plane1CstFactor * Vector3.Cross(plane2Normal, plane3Normal)
                                    + plane2CstFactor * Vector3.Cross(plane3Normal, plane1Normal)
                                        + plane3CstFactor * Vector3.Cross(plane1Normal, plane2Normal))
                / (Vector3.Dot(plane1Normal, Vector3.Cross(plane2Normal, plane3Normal)));

            float radius = (arcCenter - pointOfImpact).magnitude;

            hypotheticalArcStart.x = -pointOfImpact.x + 2 * arcCenter.x;

            Vector3 arcDistance = new Vector3(hypotheticalArcStart.x - pointOfImpact.x, 0, hypotheticalArcStart.z - pointOfImpact.z);

            float xScale = arcDistance.magnitude * 0.5f;
            float yScale = 15;
            float zScale = hypotheticalPeakPosition.y;

            arc.transform.localScale = new Vector3(xScale, yScale, zScale);

            Vector3 ballToPointOfImpactDistance = new Vector3(ballPosition.x - pointOfImpact.x, 0, ballPosition.z - pointOfImpact.z);
            float yAngle = Vector3.Angle(ballToPointOfImpactDistance, Vector3.right);

            arc.transform.rotation = Quaternion.Euler(90, yAngle, 0);

            Vector3 arcPosition = arc.transform.position;
            arcPosition.x = (ballPosition.x + pointOfImpact.x) / 2;
            arcPosition.z = (ballPosition.z + pointOfImpact.z) / 2;
            arcPosition.y = pointOfImpact.y;
            arc.transform.position = arcPosition;
        }

        public Vector3 GetLaunchForce()
        {
            return launchForce;
        }
    }
}
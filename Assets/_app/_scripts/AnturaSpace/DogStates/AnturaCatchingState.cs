using UnityEngine;

namespace Antura.AnturaSpace
{
    public class AnturaCatchingState : AnturaState
    {
        private ThrowableObject objectToCatch;
        private bool objectEaten;
        private Rigidbody boneRigidBody;

        public AnturaCatchingState(AnturaSpaceScene controller) : base(controller)
        {
        }

        public override void EnterState()
        {
            base.EnterState();

            if (controller.NextObjectToCatch == null) {
                controller.CurrentState = controller.Idle;
                return;
            }

            objectToCatch = controller.NextObjectToCatch;
            objectEaten = false;

            controller.Antura.SetTarget(objectToCatch.transform, false);
            boneRigidBody = controller.NextObjectToCatch.GetComponent<Rigidbody>();
            controller.Antura.Excited = true;
            controller.MustShowBonesButton = true;
        }

        public override void ExitState()
        {
            base.ExitState();

            controller.Antura.Excited = false;
            controller.LastTimeCatching = Time.realtimeSinceStartup;
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (!objectEaten && !controller.Antura.IsJumping &&
                (controller.Antura.HasReachedTarget || controller.Antura.PlanarDistanceFromTarget < 5)) {
                if ((controller.Antura.TargetHeight >= 2 && boneRigidBody != null && boneRigidBody.velocity.y > 10)) {
                    objectEaten = true;

                    // Jump!
                    controller.Antura.AnimationController.DoSmallJumpAndGrab(() =>
                    {
                        controller.EatObject(objectToCatch);
                        controller.CurrentState = controller.Idle;
                        objectToCatch = null;
                        objectEaten = false;
                    });

                } else if (controller.Antura.TargetHeight <= 4.5f) {
                    objectEaten = true;
                    controller.Antura.AnimationController.DoBite(() =>
                    {
                        controller.EatObject(objectToCatch);
                        controller.CurrentState = controller.Idle;
                        objectToCatch = null;
                        objectEaten = false;
                    });
                }
            }
        }
    }
}
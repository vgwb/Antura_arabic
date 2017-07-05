using UnityEngine;

namespace Antura.AnturaSpace
{
    public class AnturaCatchingState : AnturaState
    {
        private GameObject bone;
        private bool boneEaten;
        private Rigidbody boneRigidBody;

        public AnturaCatchingState(AnturaSpaceScene controller) : base(controller)
        {
        }

        public override void EnterState()
        {
            base.EnterState();

            if (controller.NextBoneToCatch == null)
            {
                controller.CurrentState = controller.Idle;
                return;
            }

            bone = controller.NextBoneToCatch.gameObject;
            boneEaten = false;

            controller.Antura.SetTarget(controller.NextBoneToCatch, false);
            boneRigidBody = controller.NextBoneToCatch.GetComponent<Rigidbody>();
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

            if (!boneEaten && !controller.Antura.IsJumping &&
                (controller.Antura.HasReachedTarget || controller.Antura.PlanarDistanceFromTarget < 5))
            {
                if ((controller.Antura.TargetHeight >= 2 && boneRigidBody != null && boneRigidBody.velocity.y > 10))
                {
                    boneEaten = true;
                    // Jump!
                    controller.Antura.AnimationController.DoSmallJumpAndGrab(() =>
                    {
                        controller.EatBone(bone);
                        controller.CurrentState = controller.Idle;
                        bone = null;
                        boneEaten = false;
                    });
                }
                else if (controller.Antura.TargetHeight <= 4.5f)
                {
                    boneEaten = true;
                    controller.Antura.AnimationController.DoBite(() =>
                    {
                        controller.EatBone(bone);
                        controller.CurrentState = controller.Idle;
                        bone = null;
                        boneEaten = false;
                    });
                }
            }
        }
    }
}
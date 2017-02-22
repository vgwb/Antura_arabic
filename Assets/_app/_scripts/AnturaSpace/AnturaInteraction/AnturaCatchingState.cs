using System;
using UnityEngine;

namespace EA4S.AnturaSpace
{
    public class AnturaCatchingState : AnturaState
    {
        GameObject bone;

        bool boneEaten = false;
        Rigidbody boneRigidBody;

        public AnturaCatchingState(AnturaSpaceManager controller) : base(controller)
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
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            
            if (!boneEaten && !controller.Antura.IsJumping && (controller.Antura.HasReachedTarget || controller.Antura.PlanarDistanceFromTarget < 5))
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

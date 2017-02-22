using System;
using UnityEngine;

namespace EA4S.AnturaSpace
{
    public class AnturaCatchingState : AnturaState
    {
        GameObject bone;
        bool boneEaten = false;
        bool isRunningJump = false;

        public AnturaCatchingState(AnturaSpaceManager controller) : base(controller)
        {
        }

        public override void EnterState()
        {
            base.EnterState();

            if (controller.LaunchedBone == null)
            {
                controller.CurrentState = controller.Idle;
                return;
            }

            bone = controller.LaunchedBone.gameObject;
            boneEaten = false;

            controller.Antura.SetTarget(controller.LaunchedBone, false);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (!boneEaten && !isRunningJump && (controller.Antura.HasReachedTarget || controller.Antura.PlanarDistanceFromTarget < 4))
            {
                if (controller.Antura.TargetHeight < 4 || controller.Antura.DistanceFromTarget < 3)
                {
                    boneEaten = true;

                    if (controller.Antura.IsJumping)
                    {
                        isRunningJump = true;
                        controller.EatBone(bone);
                        controller.Antura.AnimationController.OnJumpGrab();
                        controller.Antura.AnimationController.OnJumpMaximumHeightReached();
                        controller.Antura.AnimationController.OnJumpEnded();
                    }
                    else
                    {
                        controller.Antura.AnimationController.DoShout(() =>
                        {
                            controller.EatBone(bone);
                            controller.CurrentState = controller.Idle;
                            bone = null;
                        });
                    }
                }
                else
                {
                    controller.Antura.AnimationController.OnJumpStart();
                }
            }

            if (isRunningJump && !controller.Antura.IsJumping)
            {
                isRunningJump = false;
                controller.CurrentState = controller.Idle;
            }
        }
    }
}

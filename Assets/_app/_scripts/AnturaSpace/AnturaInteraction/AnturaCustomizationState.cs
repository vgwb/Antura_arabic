using System;
using UnityEngine;

namespace EA4S.AnturaSpace
{
    public class AnturaCustomizationState : AnturaState
    {
        float rotatingTimer;

        public AnturaCustomizationState(AnturaSpaceManager controller) : base(controller)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            controller.UI.ShowBonesButton(false);
            controller.Antura.SetTarget(controller.SceneCenter, true);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            rotatingTimer -= delta;

            if (rotatingTimer > 0)
                controller.Antura.AnimationController.State = AnturaAnimationStates.bitingTail;
            else
                controller.Antura.AnimationController.State = AnturaAnimationStates.idle;
        }

        public override void ExitState()
        {
            controller.Antura.AnimationController.State = AnturaAnimationStates.idle;
            controller.Antura.SetTarget(null, false);
            base.ExitState();
        }

        public override void OnTouched()
        {
            if (rotatingTimer > 0)
                return;

            rotatingTimer = 3;
        }
    }
}

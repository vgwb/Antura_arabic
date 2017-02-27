using System;
using UnityEngine;

namespace EA4S.AnturaSpace
{
    public class AnturaCustomizationState : AnturaState
    {
        public AnturaCustomizationState(AnturaSpaceManager controller) : base(controller)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            controller.UI.ShowBonesButton(false);
            controller.Antura.SetTarget(controller.SceneCenter, true, controller.RotatingBase.transform);
            controller.RotatingBase.Activated = true;
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            controller.Antura.AnimationController.State = AnturaAnimationStates.sitting;
        }

        public override void ExitState()
        {
            controller.RotatingBase.Activated = false;
            controller.Antura.AnimationController.State = AnturaAnimationStates.idle;
            controller.Antura.SetTarget(null, false);
            base.ExitState();
        }
    }
}

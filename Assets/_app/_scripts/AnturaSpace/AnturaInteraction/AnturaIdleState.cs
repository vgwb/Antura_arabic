using System;

namespace EA4S.AnturaSpace
{
    public class AnturaIdleState : AnturaState
    {
        float sitTimer;

        float timeToStayInThisState;

        public AnturaIdleState(AnturaSpaceManager controller) : base(controller)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            sitTimer = 0.5f;
            timeToStayInThisState = 4 + UnityEngine.Random.value * 2;
            controller.UI.ShowBonesButton(true);
            controller.Antura.SetTarget(controller.SceneCenter, true);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (controller.DraggingBone != null)
            {

            }
            else if (controller.LaunchedBone != null)
            {
                controller.CurrentState = controller.Catching;
                return;
            }

            if (controller.Antura.HasReachedTarget)
            {
                timeToStayInThisState -= delta;

                sitTimer -= delta;

                if (sitTimer <= 0)
                    controller.Antura.AnimationController.State = AnturaAnimationStates.sitting;
            }

            if (timeToStayInThisState <= 0 && controller.HasPlayerBones)
            {
                controller.CurrentState = controller.DrawingAttention;
            }
        }

        public override void ExitState()
        {
            controller.Antura.SetTarget(null, false);
            controller.UI.ShowBonesButton(false);
            base.ExitState();
        }
    }
}

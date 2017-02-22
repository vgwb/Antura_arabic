using System;

namespace EA4S.AnturaSpace
{
    public class AnturaDrawingAttentionState : AnturaState
    {
        float shoutTimer;
        float timeInThisState;

        public AnturaDrawingAttentionState(AnturaSpaceManager controller) : base(controller)
        {
        }

        public override void EnterState()
        {
            base.EnterState();
            shoutTimer = UnityEngine.Random.Range(1, 3);
            timeInThisState = 0;
            controller.UI.ShowBonesButton(true);
            controller.Antura.AnimationController.State = AnturaAnimationStates.idle;
            controller.Antura.SetTarget(controller.AttentionPosition, true);
        }

        public override void ExitState()
        {
            base.ExitState();
            controller.UI.ShowBonesButton(false);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (controller.DraggingBone != null || controller.LaunchedBone != null)
            {
                controller.CurrentState = controller.Idle;
                return;
            }

            if (shoutTimer > 0 & controller.Antura.HasReachedTarget)
            {
                timeInThisState += delta;
                shoutTimer -= delta;

                if (shoutTimer <= 0)
                {
                    shoutTimer = UnityEngine.Random.Range(1.5f, 4);
                    controller.Antura.AnimationController.DoShout(() => { Audio.AudioManager.I.PlaySound(Sfx.DogBarking); });
                }
            }

            if (timeInThisState > 10)
            {
                controller.CurrentState = controller.Idle;
            }
        }
    }
}

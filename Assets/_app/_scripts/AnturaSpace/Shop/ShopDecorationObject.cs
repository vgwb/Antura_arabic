using DG.DeExtensions;
using DG.Tweening;
using UnityEngine;

namespace Antura.AnturaSpace
{
    public class ShopDecorationObject : MonoBehaviour
    {
        public ShopDecorationSlotType slotType;
        public string id;
        public Sprite iconSprite;

        public void OnMouseDown()
        {
            ShopDecorationsManager.I.StartDragPlacement(this, false);
        }

        #region Feedback

        private ShopSlotFeedback feedback;

        public void Initialise(GameObject slotFeedbackPrefabGo)
        {
            var feedbackGo = Instantiate(slotFeedbackPrefabGo);
            feedbackGo.transform.SetParent(transform);
            feedbackGo.transform.SetLocalScale(1);
            feedbackGo.transform.localPosition = Vector3.zero;
            feedbackGo.transform.localEulerAngles = Vector3.zero;
            feedback = feedbackGo.GetComponent<ShopSlotFeedback>();
        }

        private Tween pulseTween;

        private void SetAsPreview()
        {

        }

        private void SetAsReal()
        {
        }

        public void FocusHighlight(bool choice)
        {
            if (choice) SetAsPreview();
            else SetAsReal();

            feedback.FocusHighlight(choice);
        }

        public void Spawn()
        {
            feedback.Spawn();
        }

        public void Despawn()
        {
            feedback.Despawn();
        }
        #endregion

    }
}
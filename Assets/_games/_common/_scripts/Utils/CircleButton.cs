using System;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S
{
    public class CircleButton : MonoBehaviour
    {
        public UnityEngine.UI.Image image;
        public TMPro.TextMeshProUGUI text;

        public System.Action<CircleButton> onClicked;

        ILivingLetterData answer;
        public ILivingLetterData Answer
        {
            get
            {
                return answer;
            }
            set
            {
                answer = value;
                text.text = value.TextForLivingLetter;
                image.sprite = value.DrawForLivingLetter;
                text.gameObject.SetActive(!ImageMode || image.sprite == null);
                image.gameObject.SetActive(ImageMode && image.sprite != null);
            }

        }

        bool imageMode;
        public bool ImageMode
        {
            get
            {
                return imageMode;
            }
            set
            {
                imageMode = value;
                text.gameObject.SetActive(!value || image.sprite == null);
                image.gameObject.SetActive(value && image.sprite != null);
            }

        }


        public void ScaleTo(float v1, float v2, float v3, System.Action p)
        {
        }

        public void OnClicked()
        {
            if (onClicked != null)
                onClicked(this);
        }
    }
}
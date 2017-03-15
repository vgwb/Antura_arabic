using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using EA4S.Database;
using EA4S.UI;

namespace EA4S.Book
{
    /// <summary>
    /// Displays the completion state of a variable
    /// </summary>
    public class CompletionSlider : MonoBehaviour
    {
        public TextRender Percent;
        public Slider Slider;

        /*void Update()
        {
            SetValue(Mathf.Repeat(Time.time, 1f), 1);
        }*/

        public void SetValue(float current, float max)
        {
            float ratio = current / max;
            Slider.value = ratio;
            Percent.text = string.Format("{0:P0}", ratio);
        }

    }
}
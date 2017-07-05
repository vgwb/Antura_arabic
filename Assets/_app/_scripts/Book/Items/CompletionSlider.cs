using Antura.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Antura.Book
{
    /// <summary>
    /// Displays the completion state of a variable
    /// </summary>
    public class CompletionSlider : MonoBehaviour
    {
        public TextRender Percent;
        public TextRender Current;
        public TextRender Total;
        public Slider Slider;

        public void SetValue(float current, float max)
        {
            float ratio = current / max;
            Slider.value = ratio;
            Percent.text = string.Format("{0:P0}", ratio);
            Current.text = Mathf.RoundToInt(current).ToString();
            Total.text = Mathf.RoundToInt(max).ToString();
        }

    }
}
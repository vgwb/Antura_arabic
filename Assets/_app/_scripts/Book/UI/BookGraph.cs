using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EA4S
{
    public class BookGraph : MonoBehaviour
    {
        public GameObject barPrefabGo;
        public float barWidth = 10f;
        public float barMaxHeight = 100f;

        public void SetValues(int nValues, float maxValue, float[] values, string[] moodLabels = null)
        {
            for (int value_i = 0; value_i < values.Length; value_i++)
            {
                var barGo = Instantiate(barPrefabGo);
                var barImage = barGo.GetComponentInChildren<Image>();
                var barText = barGo.GetComponentInChildren<Text>();
                barGo.transform.SetParent(this.transform);
                barImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barWidth);
                barImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, barMaxHeight * values[value_i] / maxValue);
                barGo.transform.position = (Vector3.right) * (value_i*(barWidth));

                if (moodLabels != null)
                {
                    barText.text = moodLabels[value_i];
                }
                else
                {
                    barText.text = "";
                }
            }
        }

    }
}

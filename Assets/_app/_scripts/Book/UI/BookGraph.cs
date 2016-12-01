using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EA4S
{
    public class BookGraph : MonoBehaviour
    {
        public GameObject barPrefabGo;

        public void SetValues(int nValues, float maxValue, float[] values, bool autoMaxValue = true, string[] labels = null)
        {
            if (autoMaxValue)
            {
                maxValue = Mathf.Max(values);
            }

            // Cleanup
            foreach(Transform tr in this.transform)
            {
                if (tr != this.transform) Destroy(tr.gameObject);
            }

            for (int i = 0; i < values.Length; i++)
            {
                var barGo = Instantiate(barPrefabGo);
                var barImage = barGo.GetComponentInChildren<Image>();
                var barText = barGo.GetComponentInChildren<Text>();
                barGo.transform.SetParent(this.transform);
                barImage.rectTransform.anchorMax = new Vector2(1, values[i] / maxValue);

                if (labels != null)
                {
                    barText.text = labels[i];
                }
                else
                {
                    barText.text = "";
                }
            }
        }

    }
}

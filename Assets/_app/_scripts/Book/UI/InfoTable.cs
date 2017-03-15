using UnityEngine;

namespace EA4S.Book
{
    public class InfoTable : MonoBehaviour
    {
        public GameObject RowPrefab;
        public GameObject RowSliderPrefab;

        GameObject rowGO;

        public void Reset()
        {
            emptyListContainers();
        }

        public void AddRow(string _titleEn, string _title, string _value)
        {
            rowGO = Instantiate(RowPrefab);
            rowGO.transform.SetParent(transform, false);
            rowGO.GetComponent<TableRow>().Init(_titleEn, _title, _value);
        }

        public void AddSliderRow(string _titleEn, string _title, float _value, float _valueMax)
        {
            rowGO = Instantiate(RowSliderPrefab);
            rowGO.transform.SetParent(transform, false);
            rowGO.GetComponent<TableRow>().InitSlider(_titleEn, _title, _value, _valueMax);
        }

        void emptyListContainers()
        {
            foreach (Transform t in transform) {
                Destroy(t.gameObject);
            }
        }
    }
}
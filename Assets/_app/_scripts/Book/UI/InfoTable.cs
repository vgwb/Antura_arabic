using UnityEngine;

namespace EA4S
{
    public class InfoTable : MonoBehaviour
    {
        public GameObject RowPrefab;

        GameObject rowGO;

        public void Reset()
        {
            emptyListContainers();
        }

        public void AddRow(string _title, string _value, string _subtitle = "")
        {
            rowGO = Instantiate(RowPrefab);
            rowGO.transform.SetParent(transform, false);
            rowGO.GetComponent<TableRow>().Init(_title, _value, _subtitle);
        }

        void emptyListContainers()
        {
            foreach (Transform t in transform) {
                Destroy(t.gameObject);
            }
        }
    }
}
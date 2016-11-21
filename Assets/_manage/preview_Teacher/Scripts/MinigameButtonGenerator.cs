using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace EA4S.Teacher.Test
{

    public class MinigameButtonGenerator : MonoBehaviour
    {
        public GameObject buttonPrefab;
        public TeacherTester tester;

        void Start()
        {
            foreach (var enumValue in GenericUtilities.SortEnums<MiniGameCode>()) {
                MiniGameCode code = enumValue;
                var btnGO = Instantiate(buttonPrefab);
                btnGO.transform.SetParent(this.transform);
                btnGO.GetComponentInChildren<Text>().text = (enumValue.ToString()).Replace("_", "\n");
                btnGO.GetComponent<Button>().onClick.AddListener(() => { tester.SimulateMiniGame(code);});
            }
            Destroy(buttonPrefab);
        }

    }

}

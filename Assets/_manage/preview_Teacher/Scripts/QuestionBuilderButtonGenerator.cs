using EA4S.Helpers;
using EA4S.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S.Teacher.Test
{
    /// <summary>
    /// Helper class that generates a button for each QuestionBuilder in the Teacher Management scene.
    /// </summary>
    public class QuestionBuilderButtonGenerator : MonoBehaviour
    {
        public GameObject buttonPrefab;
        public TeacherTester tester;

        void Start()
        {
            foreach (var enumValue in GenericHelper.SortEnums<QuestionBuilderType>())
            {
                QuestionBuilderType type = enumValue;
                var btnGO = Instantiate(buttonPrefab);
                btnGO.transform.SetParent(this.transform);
                btnGO.GetComponentInChildren<Text>().text = (enumValue.ToString()).Replace("_", "\n");
                btnGO.GetComponent<Button>().onClick.AddListener(() => { tester.TestQuestionBuilder(type);});
                tester.qbButtonsDict[enumValue] = btnGO.GetComponent<Button>();
            }
            Destroy(buttonPrefab);
        }

    }

}

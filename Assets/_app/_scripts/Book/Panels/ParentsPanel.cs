using UnityEngine;
using UnityEngine.UI;
using EA4S.Db;
using System.Collections.Generic;

namespace EA4S
{
    public class ParentsPanel : MonoBehaviour
    {
        [Header("Prefabs")]
        //public GameObject LearningBlockItemPrefab;

        [Header("References")]
        //public GameObject ElementsContainer;
        public TextRender ScoreText;

        void OnEnable()
        {
            InitUI();
        }

        void InitUI()
        {


        }

        public void OnSuperDogMode()
        {
            GlobalUI.ShowPrompt(true, "go super dog?", GoSuperDogMode);
        }

        void GoSuperDogMode()
        {
            Debug.Log("YEAH!");
        }

        public void OnDeleteProfile()
        {
            GlobalUI.ShowPrompt(true, "delete this profile?", GoDeleteProfile);
        }

        void GoDeleteProfile()
        {
            Debug.Log("YEAH!");
        }

        public void OnExportData()
        {
            GlobalUI.ShowPrompt(true, "Export alla Database?", GoExportData);
        }

        void GoExportData()
        {
            Debug.Log("YEAH!");
        }
    }
}
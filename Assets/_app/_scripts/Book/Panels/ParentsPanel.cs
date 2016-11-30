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

    }
}
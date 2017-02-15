using EA4S.Core;
using EA4S.Rewards;
using EA4S.UI;
using UnityEngine;
using UnityEngine.UI;

namespace EA4S.PlayerBook
{

    /// <summary>
    /// Displays information relevant to the parent. 
    /// Also enables cheat/developer commands.
    /// </summary>
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
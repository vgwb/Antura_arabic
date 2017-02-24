using UnityEngine;
using System.Collections.Generic;
using EA4S.Audio;
using EA4S.Core;
using EA4S.Database;
using EA4S.Helpers;
using EA4S.LivingLetters;
using EA4S.MinigamesAPI;
using EA4S.UI;
using EA4S.Utilities;

namespace EA4S.Book
{

    /// <summary>
    /// Displays information on all learning items the player has unlocked.
    /// </summary>
    public class JourneyPanel : MonoBehaviour, IBookPanel
    {
        [Header("Prefabs")]
        public GameObject CategoryItemPrefab;
        public GameObject LearningBlockItemPrefab;

        [Header("References")]
        public GameObject DetailPanel;
        public GameObject Submenu;
        public GameObject SubmenuContainer;
        public GameObject ListPanel;
        public GameObject ElementsContainer;

        public GameObject MoreInfoPanel;
        public TextRender ArabicText;
        public TextRender ScoreText;
        public TextRender DescriptionText;

        int currentStage;
        GameObject btnGO;

        void Start()
        {
        }

        void OnEnable()
        {
            OpenArea();
        }

        void OpenArea()
        {
            AudioManager.I.PlayDialogue(LocalizationDataId.UI_LearningBlock);
            DetailPanel.SetActive(false);
            LearningBlockPanel(1);
        }

        void LearningBlockPanel(int _stage = 1)
        {
            currentStage = _stage;

            Submenu.SetActive(true);
            ListPanel.SetActive(true);
            emptyListContainers();

            List<LearningBlockData> list = AppManager.I.DB.FindLearningBlockData((x) => (x.Stage == currentStage));

            List<LearningBlockInfo> info_list = AppManager.I.ScoreHelper.GetAllLearningBlockInfo();
            foreach (var info_item in info_list) {
                if (list.Contains(info_item.data)) {
                    btnGO = Instantiate(LearningBlockItemPrefab);
                    btnGO.transform.SetParent(ElementsContainer.transform, false);
                    btnGO.GetComponent<ItemLearningBlock>().Init(this, info_item);
                }
            }

            var listStages = AppManager.I.DB.GetAllStageData();

            foreach (var stage in listStages) {
                btnGO = Instantiate(CategoryItemPrefab);
                btnGO.transform.SetParent(SubmenuContainer.transform, false);
                btnGO.GetComponent<MenuItemCategory>().Init(
                    this,
                    new GenericCategoryData {
                        area = VocabularyChapter.LearningBlock,
                        Id = stage.Id,
                        Title = "Stage " + stage.Id
                    },
                    int.Parse(stage.Id) == currentStage
                );
            }

        }

        public void SelectSubCategory(GenericCategoryData _stage)
        {
            LearningBlockPanel(int.Parse(_stage.Id));
        }

        public void DetailLearningBlock(LearningBlockInfo info)
        {
            DetailPanel.SetActive(true);
            AudioManager.I.PlayDialogue(info.data.GetTitleSoundFilename());
            ScoreText.text = "Score: " + info.score;
            MoreInfoPanel.SetActive(true);

            ArabicText.text = info.data.Title_Ar;
            DescriptionText.text = info.data.Description;
        }

        void emptyListContainers()
        {
            foreach (Transform t in ElementsContainer.transform) {
                Destroy(t.gameObject);
            }
            foreach (Transform t in SubmenuContainer.transform) {
                Destroy(t.gameObject);
            }
        }


        void ResetLL()
        {

        }
    }
}
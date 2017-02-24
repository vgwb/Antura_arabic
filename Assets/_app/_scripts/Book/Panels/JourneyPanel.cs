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


        int currentChapter = 0;
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
            LearningBlockPanel();
        }

        void LearningBlockPanel()
        {
            ListPanel.SetActive(false);
            Submenu.SetActive(false);
            ListPanel.SetActive(true);
            emptyListContainers();

            List<LearningBlockInfo> info_list = AppManager.I.ScoreHelper.GetAllLearningBlockInfo();
            foreach (var item_info in info_list) {
                btnGO = Instantiate(LearningBlockItemPrefab);
                btnGO.transform.SetParent(ElementsContainer.transform, false);
                btnGO.GetComponent<ItemLearningBlock>().Init(this, item_info);
            }

            //foreach (WordDataCategory cat in GenericHelper.SortEnums<WordDataCategory>()) {
            //    btnGO = Instantiate(CategoryItemPrefab);
            //    btnGO.transform.SetParent(SubmenuContainer.transform, false);
            //    btnGO.GetComponent<MenuItemCategory>().Init(
            //        this,
            //        new GenericCategoryData {
            //            area = VocabularyChapter.Words,
            //            wordCategory = cat,
            //            Id = cat.ToString(),
            //            Title = LocalizationManager.GetWordCategoryTitle(cat)
            //        });
            //}
        }

        public void SelectSubCategory(GenericCategoryData _category)
        {

        }

        public void DetailLearningBlock(LearningBlockInfo info)
        {
            DetailPanel.SetActive(true);
            AudioManager.I.PlayDialogue(info.data.GetTitleSoundFilename());
            ScoreText.text = "Score: " + info.score;
            MoreInfoPanel.SetActive(false);

            ArabicText.text = info.data.Title_Ar;

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
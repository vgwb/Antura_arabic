using System.Collections.Generic;

namespace EA4S.MiniGameConfiguration
{
    /// <summary>
    /// Given a minigame, handles the generation of question packs
    /// </summary>
    public class MiniGameConfigurationGenerator
    {
        IMiniGameConfigurationRules currentConfigurationRules;

        public void SetCurrentMiniGame(MiniGameCode _miniGameCode)
        {
            currentConfigurationRules = GetConfigurationRules(_miniGameCode);
        }

        /// <summary>
        /// Select what configuration rules to use based on the selected mini game
        /// </summary>
        private IMiniGameConfigurationRules GetConfigurationRules(MiniGameCode _miniGameCode)
        {
            IMiniGameConfigurationRules selectedRules = null;
            switch (_miniGameCode)
            {
                default:
                    selectedRules = new All_MiniGameConfigurationRules(_miniGameCode);
                    break;
            }
            return selectedRules;
        }

        public List<IQuestionPack> GenerateQuestionPacks()
        {
            int questionPacksNumber = currentConfigurationRules.GetQuestionPackCount();
            List<IQuestionPack> questionPackList = new List<IQuestionPack>();
            for (int i = 0; i < questionPacksNumber; i++)
            {
                questionPackList.Add(currentConfigurationRules.CreateQuestionPack());
            }

            /*// TODO: separate LL_LetterData and such in a FINAL step
            List<QuestionPackData> questionPackDataList = new List<QuestionPackData>();
            for (int i = 0; i < questionPacksNumber; i++)
            {
                questionPackDataList.Add(currentConfigurationRules.CreateQuestionPackData());
            }
            List<IQuestionPack> questionPackList = ConvertToQuestionPacks(questionPacksData);*/

            return questionPackList;
        }

        private List<IQuestionPack> ConvertToQuestionPacks(List<QuestionPackData> questionPackDataList)
        {
            // TODO: implement
            return new List<IQuestionPack>();
        }

        public class QuestionPackData
        {
            // TODO: implement
        }

    }
}

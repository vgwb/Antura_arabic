using EA4S.API;
using System.Collections.Generic;

namespace EA4S
{

    /// <summary>
    /// Given a minigame, handles the generation of question packs
    /// </summary>
    public class MiniGameConfigurationGenerator
    {
        public IQuestionBuilder currentConfigurationRules;

        public void SetCurrentMiniGame(MiniGameCode _miniGameCode)
        {
            currentConfigurationRules = GetConfigurationRules(_miniGameCode);
        }

        /// <summary>
        /// Select what configuration rules to use based on the selected mini game
        /// </summary>
        private IQuestionBuilder GetConfigurationRules(MiniGameCode _miniGameCode)
        {
            IQuestionBuilder selectedRules = null;
            switch (_miniGameCode)
            {
                case MiniGameCode.Egg:
                    selectedRules = new Egg_MiniGameConfigurationRules();
                    break;
                default:
                    selectedRules = new All_MiniGameConfigurationRules(_miniGameCode);
                    break;
            }
            return selectedRules;
        }

        public List<IQuestionPack> GenerateQuestionPacks()
        {
            int questionPacksNumber = currentConfigurationRules.GetQuestionPackCount();

            List<IQuestionPack> questionPackList;
            try
            {
                // @note: this is the new logic to handle question pack, which does not depend on LivingLetterData anymore
                List<QuestionPackData> questionPackDataList = new List<QuestionPackData>();
                for (int i = 0; i < questionPacksNumber; i++)
                {
                    var newData = currentConfigurationRules.CreateQuestionPackData();
                    questionPackDataList.Add(newData);

                    UnityEngine.Debug.Log(newData.ToString());

                }
                questionPackList = ConvertToQuestionPacks(questionPackDataList);
            }
            catch
            {
                // @note: this is a fallback needed for now to make old minigame rules work
                questionPackList = new List<IQuestionPack>();
                for (int i = 0; i < questionPacksNumber; i++)
                {
                    questionPackList.Add(currentConfigurationRules.CreateQuestionPack());
                }
            }

            return questionPackList;
        }

        private List<IQuestionPack> ConvertToQuestionPacks(List<QuestionPackData> questionPackDataList)
        {
            List<IQuestionPack> questionPackList = new List<IQuestionPack>();
            foreach(var questionPackData in questionPackDataList)
            {
                ILivingLetterData ll_question = questionPackData.question.ConvertToLivingLetterData();
                List<ILivingLetterData> ll_wrongAnswers = questionPackData.wrongAnswers.ConvertAll(x => x.ConvertToLivingLetterData());
                List<ILivingLetterData> ll_correctAnswers = questionPackData.correctAnswers.ConvertAll(x => x.ConvertToLivingLetterData());
                IQuestionPack questionPack = new FindRightDataQuestionPack(ll_question, ll_wrongAnswers, ll_correctAnswers);
                questionPackList.Add(questionPack);
            }
            return questionPackList;
        }
    }


}

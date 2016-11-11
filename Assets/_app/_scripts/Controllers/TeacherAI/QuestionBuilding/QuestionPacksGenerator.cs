using EA4S.API;
using System.Collections.Generic;

namespace EA4S
{

    /// <summary>
    /// Given a minigame, handles the generation of question packs
    /// </summary>
    public class QuestionPacksGenerator
    {
        public List<IQuestionPack> GenerateQuestionPacks(IQuestionBuilder currentConfigurationRules)
        {
            int questionPacksNumber = currentConfigurationRules.GetQuestionPackCount();

            // @todo: add this new version instead List<QuestionPackData> questionPackDataList = currentConfigurationRules.CreateAllQuestionPacks();

            List<QuestionPackData> questionPackDataList = new List<QuestionPackData>();
            for (int i = 0; i < questionPacksNumber; i++)
            {
                var newData = currentConfigurationRules.CreateQuestionPackData();
                questionPackDataList.Add(newData);
                //UnityEngine.Debug.Log(newData.ToString());
            }
            List<IQuestionPack> questionPackList = ConvertToQuestionPacks(questionPackDataList);

            return questionPackList;
        }

        private List<IQuestionPack> ConvertToQuestionPacks(List<QuestionPackData> questionPackDataList)
        {
            List<IQuestionPack> questionPackList = new List<IQuestionPack>();
            foreach(var questionPackData in questionPackDataList)
            {
                ILivingLetterData ll_question = questionPackData.question != null ? questionPackData.question.ConvertToLivingLetterData() : null;
                List<ILivingLetterData> ll_questions = questionPackData.questions != null ? questionPackData.questions.ConvertAll(x => x.ConvertToLivingLetterData()) : null;
                List<ILivingLetterData> ll_wrongAnswers = questionPackData.wrongAnswers != null ? questionPackData.wrongAnswers.ConvertAll(x => x.ConvertToLivingLetterData()) : null;
                List<ILivingLetterData> ll_correctAnswers = questionPackData.correctAnswers != null ? questionPackData.correctAnswers.ConvertAll(x => x.ConvertToLivingLetterData()) : null;
                IQuestionPack pack;

                // Conversion based on what kind of question we have
                // @todo: at least on the teacher's side, this could be simplified by always using a LIST of questions
                if (ll_questions != null && ll_questions.Count > 0)
                {
                    pack = new FindRightDataQuestionPack(ll_questions, ll_wrongAnswers, ll_correctAnswers);
                } else
                {
                    pack = new FindRightDataQuestionPack(ll_question, ll_wrongAnswers, ll_correctAnswers);
                }
                questionPackList.Add(pack);
            }
            return questionPackList;
        }
    }


}

using EA4S.API;
using System.Collections.Generic;

namespace EA4S
{
    /// <summary>
    /// Handles the logic to launch minigames with the correct configuration.
    /// </summary>
    public class MiniGameLauncher
    {
        private QuestionPacksGenerator questionPacksGenerator;
        private TeacherAI teacher;

        public MiniGameLauncher(TeacherAI _teacher)
        {
            teacher = _teacher;
            questionPacksGenerator = new QuestionPacksGenerator();
        }

        public void LaunchGame(MiniGameCode miniGameCode)
        {
            float difficulty = teacher.GetCurrentDifficulty(miniGameCode);
            GameConfiguration configuration = new GameConfiguration(difficulty);
            MiniGameAPI.Instance.StartGame(miniGameCode, configuration);
        } 

        public List<IQuestionPack> RetrieveQuestionPacks(IQuestionBuilder builder)
        {
            return questionPacksGenerator.GenerateQuestionPacks(builder);
        }

    }

}


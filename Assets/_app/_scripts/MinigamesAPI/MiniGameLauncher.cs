using System.Collections.Generic;
using EA4S.MinigamesCommon;
using EA4S.Teacher;

namespace EA4S.MinigamesAPI
{
    /// <summary>
    /// Handles the logic to launch minigames with the correct configuration.
    /// </summary>
    // refactor: MiniGameLauncher can probably be merged with MiniGameAPI (actually, it's better to move MiniGameAPI here!)
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


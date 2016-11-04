using EA4S.API;
using EA4S.MiniGameConfiguration;
using System.Collections.Generic;

namespace EA4S
{
    /// <summary>
    /// Handles the logic to launch minigames with the correct configuration.
    /// </summary>
    public class MiniGameLauncher
    {
        private MiniGameConfigurationGenerator configurationGenerator;
        private TeacherAI teacher;

        public MiniGameLauncher(TeacherAI _teacher)
        {
            this.teacher = _teacher;
            configurationGenerator = new MiniGameConfigurationGenerator();
        }

        public void LaunchGame(MiniGameCode miniGameCode)
        {
            this.configurationGenerator.SetCurrentMiniGame(miniGameCode);
            List<IQuestionPack> questionPacks = configurationGenerator.GenerateQuestionPacks();

            float difficulty = this.teacher.GetCurrentDifficulty();
            GameConfiguration configuration = new GameConfiguration(difficulty);

            MiniGameAPI.Instance.StartGame(miniGameCode, questionPacks, configuration);
        }
    }

}


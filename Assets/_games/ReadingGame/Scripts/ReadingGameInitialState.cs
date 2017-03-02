using EA4S.MinigamesCommon;

namespace EA4S.Minigames.ReadingGame
{
    public class ReadingGameInitialState : IState
    {
        ReadingGameGame game;

        float timer = 2;

        bool introCompleted = false;

        public ReadingGameInitialState(ReadingGameGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            timer = 2;

            if (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.ReadAndAnswer)
            {
                game.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.ReadingGame_Title, () => { introCompleted = true; });
            }
            else if (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.AlphabetSong)
            {
                game.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.AlphabetSong_Title, () => { introCompleted = true; });
            }
            else if (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.DiacriticSong)
            {
                game.Context.GetAudioManager().PlayDialogue(Database.LocalizationDataId.Diacritic_Song_Title, () => { introCompleted = true; });
            }
            else
            {
                introCompleted = true;
            }

            if (ReadingGameConfiguration.Instance.Variation == ReadingGameVariation.ReadAndAnswer)
                game.Context.GetAudioManager().PlayMusic(Music.Theme8);
            else
                game.Context.GetAudioManager().StopMusic();
        }


        public void ExitState()
        {
           
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0 && introCompleted)
                game.SetCurrentState(game.QuestionState);
        }

        public void UpdatePhysics(float delta)
        {

        }
    }
}
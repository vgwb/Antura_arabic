namespace EA4S.SickLetters
{
    public class ResultGameState : IGameState
    {
        SickLettersGame game;
        int stars, score;

        float timer = 0;
        public ResultGameState(SickLettersGame game)
        {
            this.game = game;
        }

        public void EnterState()
        {
            

            game.LLPrefab.jumpOut(0,true);

            if (game.scale.counter < game.targetScale)
            {
                game.manager.failure();
                timer = 6;
            }

            if (game.scale.counter >= game.targetScale)
                timer= 4;
        }

        public void ExitState()
        {
        }

        public void Update(float delta)
        {
            timer -= delta;

            if (timer < 0)
            {
                game.EndGame(game.currentStars, game.maxWieght);

                if (game.currentStars == 0)
                {
                    AudioManager.I.PlayDialog("Reward_0Star");
                    //WidgetSubtitles.I.DisplaySentence(Db.LocalizationDataId.Reward_0Star, 4, true);
                }
                else
                {
                    //string dia = "Reward_" + game.currentStars + "Star_" + UnityEngine.Random.Range(1, 4);
                    Db.LocalizationDataId data = randomRewardData();
                    //WidgetSubtitles.I.gameObject.SetActive(true);
                    AudioManager.I.PlayDialog(data);
                    //WidgetSubtitles.I.DisplaySentence(data , 2, true);
                }
                //game.Context.GetAudioManager().PlayDialogue(Db.LocalizationData)
                //WidgetSubtitles.I.DisplaySentence()
                //game.EndGame(game.scale.counter / (game.targetScale / 3), game.scale.counter);
            }
        }

        public void UpdatePhysics(float delta)
        {
        }

        Db.LocalizationDataId randomRewardData()
        {
            if (game.currentStars == 1)
                return (Db.LocalizationDataId)(UnityEngine.Random.Range(262,265));
            else if (game.currentStars == 2)
                return (Db.LocalizationDataId)(UnityEngine.Random.Range(265, 268));
            else
                return (Db.LocalizationDataId)(UnityEngine.Random.Range(268, 271));

        }
    }
}

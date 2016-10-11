namespace EA4S
{
    public enum Music
    {
        Silence = 0,
        MainTheme = 1,
        Relax = 2,
        Lullaby = 3,
        Theme3 = 4,
        Theme4 = 5
    }

    public enum Sfx
    {
        Hit,
        BaloonPop,
        DangerClock,
        DangerClockLong,
        GameTitle,
        Win,
        Lose,
        UIPopup,
        UIButtonClick,
        UIPauseIn,
        UIPauseOut,
        CameraMovement,
        AlarmClock,
        LetterAngry,
        LetterHappy,
        LetterSad,
        LetterHold,
        LetterFear,
        WalkieTalkie,
        WheelStart,
        WheelTick,
        DogBarking,
        DogSnoring,
        DogSnorting,
        OK,
        KO,
        Transition,
        StarFlower,
        StampOK
    }

    class AudioConfig
    {

        public static string GetMusicEventName(Music music)
        {
            var eventName = "";
            switch (music) {
                case Music.Silence:
                    eventName = "";
                    break;
                case Music.MainTheme:
                    eventName = "Music1";
                    break;
                case Music.Relax:
                    eventName = "Music2";
                    break;
                case Music.Lullaby:
                    eventName = "Music5";
                    break;
                case Music.Theme3:
                    eventName = "Music3";
                    break;
                case Music.Theme4:
                    eventName = "Music4";
                    break;
            }
            return eventName;
        }

        public static string GetSfxEventName(Sfx sfx)
        {
            var eventName = "";
            switch (sfx) {
                case Sfx.Hit:
                    eventName = "Sfx/Hit";
                    break;
                case Sfx.DangerClock:
                    eventName = "Sfx/DangerClock";
                    break;
                case Sfx.DangerClockLong:
                    eventName = "Sfx/DangerClockLong";
                    break;
                case Sfx.Win:
                    eventName = "Sfx/Win";
                    break;
                case Sfx.Lose:
                    eventName = "Sfx/Lose";
                    break;
                case Sfx.BaloonPop:
                    eventName = "Sfx/BalloonPop";
                    break;
                case Sfx.UIPopup:
                    eventName = "Sfx/UI/Popup";
                    break;
                case Sfx.UIButtonClick:
                    eventName = "Sfx/UI/Button";
                    break;
                case Sfx.UIPauseIn:
                    eventName = "Sfx/UI/PauseIn";
                    break;
                case Sfx.UIPauseOut:
                    eventName = "Sfx/UI/PauseOut";
                    break;
                case Sfx.CameraMovement:
                    eventName = "Sfx/CameraMovement";
                    break;
                case Sfx.AlarmClock:
                    eventName = "Sfx/AlarmClock";
                    break;
                case Sfx.LetterAngry:
                    eventName = "LivingLetter/Angry";
                    break;
                case Sfx.LetterHappy:
                    eventName = "LivingLetter/Happy";
                    break;
                case Sfx.LetterSad:
                    eventName = "LivingLetter/Sad";
                    break;
                case Sfx.LetterHold:
                    eventName = "LivingLetter/Hold";
                    break;
                case Sfx.LetterFear:
                    eventName = "LivingLetter/Fear";
                    break;
                case Sfx.GameTitle:
                    eventName = "VOX/GameTitle";
                    break;
                case Sfx.WalkieTalkie:
                    eventName = "Sfx/WalkieTalkie";
                    break;
                case Sfx.WheelStart:
                    eventName = "Sfx/WheelStart";
                    break;
                case Sfx.WheelTick:
                    eventName = "Sfx/WheelTick";
                    break;
                case Sfx.DogBarking:
                    eventName = "Dog/Barking";
                    break;
                case Sfx.DogSnoring:
                    eventName = "Dog/Snoring";
                    break;
                case Sfx.DogSnorting:
                    eventName = "Dog/Snorting";
                    break;
                case Sfx.OK:
                    eventName = "Sfx/OK";
                    break;
                case Sfx.KO:
                    eventName = "Sfx/KO";
                    break;
                case Sfx.Transition:
                    eventName = "Sfx/Transition";
                    break;
                case Sfx.StarFlower:
                    eventName = "Sfx/StarFlower";
                    break;
                case Sfx.StampOK:
                    eventName = "Sfx/StampOK";
                    break;
            }
            return eventName;
        }
    }
}
namespace EA4S
{
    public enum Music
    {
        Silence = 0,
        MainTheme = 1,
        Relax = 2,
        Lullaby = 3,
        Theme3 = 4,
        Theme4 = 5,
        Theme6 = 6,
        Theme7 = 7
    }

    // last is 45
    public enum Sfx
    {
        AlarmClock = 13,
        BallHit = 31,
        BaloonPop = 2,
        BushRustlingIn = 32,
        BushRustlingOut = 33,
        CameraMovement = 12,
        CrateLandOnground = 34,
        DangerClock = 3,
        DangerClockLong = 4,
        DogBarking = 22,
        DogSnoring = 23,
        DogSnorting = 24,
        EggBreak = 40,
        EggMove = 41,
        GameTitle = 5,
        Hit = 1,
        KO = 26,
        LetterAngry = 14,
        LetterFear = 18,
        LetterHappy = 15,
        LetterHold = 17,
        LetterSad = 16,
        Lose = 7,
        OK = 25,
        PipeBlowIn = 35,
        PipeBlowOut = 36,
        Poof = 37,
        ScaleDown = 43,
        ScaleUp = 44,
        ScreenHit = 38,
        Splat = 30,
        StampOK = 29,
        StarFlower = 28,
        ThrowArm = 42,
        ThrowObj = 39,
        TickAndWin = 45,
        Transition = 27,
        UIButtonClick = 9,
        UIPauseIn = 10,
        UIPauseOut = 11,
        UIPopup = 8,
        WalkieTalkie = 19,
        WheelStart = 20,
        WheelTick = 21,
        Win = 6
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
                case Music.Theme6:
                    eventName = "Music6";
                    break;
                case Music.Theme7:
                    eventName = "Music7";
                    break;
            }
            return eventName;
        }

        public static string GetSfxEventName(Sfx sfx)
        {
            var eventName = "";
            switch (sfx) {
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
                case Sfx.DogBarking:
                    eventName = "Dog/Barking";
                    break;
                case Sfx.DogSnoring:
                    eventName = "Dog/Snoring";
                    break;
                case Sfx.DogSnorting:
                    eventName = "Dog/Snorting";
                    break;
                default:
                    eventName = "Sfx/" + sfx.ToString();
                    break;
            }
            return eventName;
        }
    }
}
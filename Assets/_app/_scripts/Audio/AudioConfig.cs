namespace EA4S
{
    public enum Music
    {
        Silence = 0,
        MainTheme = 1,
        Relax = 2,
        Lullaby = 3,
        Theme6 = 6,
        Theme7 = 7,
        Theme8 = 8,
        Theme9 = 9,
        Theme10 = 10
    }

    // last is 69
    public enum Sfx
    {
        AlarmClock = 13,
        BallHit = 31,
        BalloonPop = 2,
        Blip = 62,
        BushRustlingIn = 32,
        BushRustlingOut = 33,
        CameraMovement = 12,
        CameraMovementShort = 63,
        ChoiceSwipe = 64,
        CrateLandOnground = 34,
        DangerClock = 3,
        DangerClockLong = 4,
        DogBarking = 22,
        DogSnoring = 23,
        DogSnorting = 24,
        Dog_Exhale = 60,
        Dog_Inhale = 61,
        Dog_Noize = 69,
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
        LL_Afraid = 46,
        LL_Annoyed = 47,
        LL_Giggle = 48,
        LL_Jump = 50,
        LL_La = 51,
        LL_Laugh = 52,
        LL_No1 = 53,
        LL_No2 = 54,
        LL_No3 = 55,
        LL_Sleep = 56,
        LL_Surprise = 57,
        LL_Suspicious = 58,
        LL_Tada = 59,
        LL_Yeah = 49,
        Lose = 7,
        OK = 25,
        PipeBlowIn = 35,
        PipeBlowOut = 36,
        Poof = 37,
        RocketMove = 66,
        ScaleDown = 43,
        ScaleUp = 44,
        ScreenHit = 38,
        ScreenGlassHit = 65,
        Splat = 30,
        StampOK = 29,
        StarFlower = 28,
        ThrowArm = 42,
        ThrowObj = 39,
        TickAndWin = 45,
        Transition = 27,
        TrapdoorClose = 67,
        TrapdoorOpen = 68,
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
                case Music.Theme6:
                    eventName = "Music6";
                    break;
                case Music.Theme7:
                    eventName = "Music7";
                    break;
                case Music.Theme8:
                    eventName = "Music8";
                    break;
                case Music.Theme9:
                    eventName = "Music9";
                    break;
                case Music.Theme10:
                    eventName = "Music10";
                    break;
            }
            return eventName;
        }

        public static string GetSfxEventName(Sfx sfx)
        {
            var eventName = "";
            switch (sfx) {
                case Sfx.UIPopup:
                    eventName = "Sfx/UI/Popup"; break;
                case Sfx.UIButtonClick:
                    eventName = "Sfx/UI/Button"; break;
                case Sfx.UIPauseIn:
                    eventName = "Sfx/UI/PauseIn"; break;
                case Sfx.UIPauseOut:
                    eventName = "Sfx/UI/PauseOut"; break;
                case Sfx.LetterAngry:
                    eventName = "LL/Angry"; break;
                case Sfx.LetterHappy:
                    eventName = "LL/Happy"; break;
                case Sfx.LetterSad:
                    eventName = "LL/Sad"; break;
                case Sfx.LetterHold:
                    eventName = "LL/Hold"; break;
                case Sfx.LetterFear:
                    eventName = "LL/Fear"; break;
                case Sfx.LL_Afraid:
                    eventName = "LL/Afraid"; break;
                case Sfx.LL_Annoyed:
                    eventName = "LL/Annoyed"; break;
                case Sfx.LL_Giggle:
                    eventName = "LL/Giggle"; break;
                case Sfx.LL_Jump:
                    eventName = "LL/Jump"; break;
                case Sfx.LL_La:
                    eventName = "LL/La"; break;
                case Sfx.LL_Laugh:
                    eventName = "LL/Laugh"; break;
                case Sfx.LL_No1:
                    eventName = "LL/No1"; break;
                case Sfx.LL_No2:
                    eventName = "LL/No2"; break;
                case Sfx.LL_No3:
                    eventName = "LL/No3"; break;
                case Sfx.LL_Sleep:
                    eventName = "LL/Sleep"; break;
                case Sfx.LL_Surprise:
                    eventName = "LL/Surprise"; break;
                case Sfx.LL_Suspicious:
                    eventName = "LL/Suspicious"; break;
                case Sfx.LL_Tada:
                    eventName = "LL/Tada"; break;
                case Sfx.LL_Yeah:
                    eventName = "LL/Yeah"; break;
                case Sfx.GameTitle:
                    eventName = "VOX/GameTitle"; break;
                case Sfx.DogBarking:
                    eventName = "Dog/Barking"; break;
                case Sfx.DogSnoring:
                    eventName = "Dog/Snoring"; break;
                case Sfx.DogSnorting:
                    eventName = "Dog/Snorting"; break;
                case Sfx.Dog_Exhale:
                    eventName = "Dog/Exhale"; break;
                case Sfx.Dog_Inhale:
                    eventName = "Dog/Inhale"; break;
                case Sfx.Dog_Noize:
                    eventName = "Dog/Noize"; break;
                default:
                    eventName = "Sfx/" + sfx.ToString();
                    break;
            }
            return eventName;
        }
    }
}
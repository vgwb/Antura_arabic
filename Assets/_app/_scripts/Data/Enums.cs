using System.Collections;

namespace EA4S
{

    public enum MinigamesCode {
        FastCrowd = 1,
        DontWakeUp = 2,
        Balloons = 3
    }

    public enum MinigameState {
        Initializing,
        GameIntro,
        RoundIntro,
        Playing,
        Paused,
        RoundEnd,
        GameEnd,
        Result,
        Award
    }

}

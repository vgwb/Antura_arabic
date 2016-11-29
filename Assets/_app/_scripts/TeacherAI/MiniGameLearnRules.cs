
namespace EA4S
{
    /// <summary>
    /// Specific rules per mini game
    /// </summary>
    public class MiniGameLearnRules
    {
        public enum VoteLogic
        {
            Threshold,
            SuccessRatio
        }

        public VoteLogic voteLogic;
        public float logicParameter;                // for example, success threshold 
        public float minigameVoteSkewOffset;        // takes into account that some minigames are skewed
        public float minigameImportanceWeight;      // takes into account that some minigames are more important on learning in respect to others

        public MiniGameLearnRules()
        {
            // default learn rules
            voteLogic = VoteLogic.SuccessRatio;
            logicParameter = 0f;
            minigameVoteSkewOffset = 0f;
            minigameImportanceWeight = 1f;
        }
    }

}
using UnityEngine;

namespace EA4S.Assessment
{
    public interface IDialogueManager
    {
        /// <summary>
        /// Play audio and show subtitles for a dialogue. You can "yield return it"
        /// </summary>
        /// <param name="ID">Dialogue ID</param>
        /// <returns>Yield instruction to wait it ends</returns>
        YieldInstruction Dialogue( Db.LocalizationDataId ID, bool showWalkieTalkie = false);

        /// <summary>
        /// Play audio for a dialogue. You can "yield return it"
        /// </summary>
        /// <param name="ID">Dialogue ID</param>
        /// <returns>Yield instruction to wait it ends</returns>
        YieldInstruction Speak( Db.LocalizationDataId ID);
    }
}

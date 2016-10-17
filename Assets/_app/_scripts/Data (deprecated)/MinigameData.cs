using UnityEngine;
using System.Collections;

namespace EA4S.deprecated
{
    public class MinigameData
    {
        public string Code;
        public string Title;
        public string EnglishTitle;
        public bool Available;
        public string SceneName;

        public MinigameData(string code, string title, string englishTitle, string sceneName, bool available)
        {
            Code = code;
            Title = title;
            EnglishTitle = englishTitle;
            SceneName = sceneName;
            Available = available;
        }

        public string GetIconResourcePath()
        {
            return "Images/GameIcons/minigame_icon_" + Code;
        }

    }
}

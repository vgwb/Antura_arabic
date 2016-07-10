using UnityEngine;
using System.Collections;

namespace EA4S
{
    public class MinigameData
    {
        public string Code;
        public string Title;
        public string EnglishTitle;
        public bool Available;

        public MinigameData(string code, string title, string englishTitle, bool available) {
            Code = code;
            Title = title;
            EnglishTitle = englishTitle;
            Available = available;
        }

        public string GetIconResourcePath() {
            return "Images/GameIcons/minigame_icon_" + Code;
        }

    }
}
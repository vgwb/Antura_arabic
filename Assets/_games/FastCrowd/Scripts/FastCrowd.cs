using UnityEngine;
using System.Collections.Generic;
using CGL.Antura;
using ModularFramework.Modules;

namespace CGL.Antura.FastCrowd {

    public class FastCrowd : AnturaMiniGame {

        public LetterObjectView LetterPrefab;
        //List<LetterData> letters = LetterDataListFromWord(_word, _vocabulary);

        protected override void ReadyForGameplay() {
            base.ReadyForGameplay();
            // put here start logic

            for (int i = 0; i < 3; i++) {
                //ArabicAlphabetHelper
                //LetterData newLetter = new LetterData() {
                //};
            }
        }



    }
}
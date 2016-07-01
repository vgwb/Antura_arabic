using UnityEngine;
using System.Collections.Generic;
using CGL.Antura;
using ModularFramework.Helpers;
using Google2u;

namespace CGL.Antura.FastCrowd {

    public class FastCrowd : AnturaMiniGame {

        public LetterObjectView LetterPrefab;
        public Transform Terrain;

        public int MinLettersOnField = 10;
        //List<LetterData> letters = LetterDataListFromWord(_word, _vocabulary);

        protected override void ReadyForGameplay() {
            base.ReadyForGameplay();
            // put here start logic

            // Get letters and word
            // TODO: Only for pre-alpha. This logic must be in Antura app logic.
            string word = words.Instance.Rows.GetRandomElement()._word;
            List<LetterData> gameLetters = ArabicAlphabetHelper.LetterDataListFromWord(word, AnturaGameManager.Instance.Letters);

            foreach (LetterData letterData in gameLetters) {
                LetterObjectView letterObjectView = Instantiate(LetterPrefab);
                letterObjectView.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); // ???
                letterObjectView.transform.SetParent(Terrain, true);
                letterObjectView.Init(letterData);
            }

            /// Add other random letters
            int OtherLettersCount = MinLettersOnField - gameLetters.Count;
            for (int i = 0; i < OtherLettersCount; i++) {
                LetterObjectView letterObjectView = Instantiate(LetterPrefab);
                letterObjectView.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); // ???
                letterObjectView.transform.SetParent(Terrain, true);
                letterObjectView.Init(AnturaGameManager.Instance.Letters.GetRandomElement());
            }
        }



    }
}
using UnityEngine;
using System.Collections.Generic;
using CGL.Antura;
using ModularFramework.Helpers;
using Google2u;

namespace CGL.Antura.FastCrowd {

    public class FastCrowd : AnturaMiniGame {

        [Header("Letters Env")]
        public LetterObjectView LetterPref;
        public Transform TerrainTrans;

        [Header("Drop Area")] 
        public DropSingleArea DropSingleAreaPref;
        public Transform DropAreaContainerTrans;

        [Header("Gameplay")]
        public int MinLettersOnField = 10;
        //List<LetterData> letters = LetterDataListFromWord(_word, _vocabulary);

        protected override void ReadyForGameplay() {
            base.ReadyForGameplay();
            // put here start logic

            // Get letters and word
            // TODO: Only for pre-alpha. This logic must be in Antura app logic.
            string word = words.Instance.Rows.GetRandomElement()._word;
            List<LetterData> gameLetters = ArabicAlphabetHelper.LetterDataListFromWord(word, AnturaGameManager.Instance.Letters);

            int count = 0;
            // Letter from db filtered by some parameters
            foreach (LetterData letterData in gameLetters) {
                LetterObjectView letterObjectView = Instantiate(LetterPref);
                letterObjectView.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); // TODO: check for alternative solution!
                letterObjectView.transform.SetParent(TerrainTrans, true);
                letterObjectView.Init(letterData);
                PlaceDropAreaElement(letterData, count);
                count++;
            }

            // Add other random letters
            int OtherLettersCount = MinLettersOnField - gameLetters.Count;
            for (int i = 0; i < OtherLettersCount; i++) {
                LetterObjectView letterObjectView = Instantiate(LetterPref);
                letterObjectView.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); // TODO: check for alternative solution!
                letterObjectView.transform.SetParent(TerrainTrans, true);
                // TODO: the selection is curiously only between the letters of the word... to be checked.
                letterObjectView.Init(AnturaGameManager.Instance.Letters.GetRandomElement());
            }

            

        }

        /// <summary>
        /// Place drop object area in drop object area container.
        /// </summary>
        /// <param name="_letterData"></param>
        void PlaceDropAreaElement(LetterData _letterData, int position) {
            DropSingleArea dropSingleArea = Instantiate(DropSingleAreaPref);
            dropSingleArea.transform.SetParent(DropAreaContainerTrans, false);
            dropSingleArea.transform.position = dropSingleArea.transform.position + new Vector3(-1.8f * position, 0, 0);
            dropSingleArea.Init(_letterData);
        }


    }
}
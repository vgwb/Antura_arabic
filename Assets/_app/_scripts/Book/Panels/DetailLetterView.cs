using Antura.Audio;
using Antura.Core;
using Antura.Database;
using Antura.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Antura.Book
{
    public class DetailLetterView : MonoBehaviour
    {
        [Header("References")]
        public LetterAllForms MainLetterDisplay;
        public GameObject BtnPlayLetterName;
        public GameObject BtnPlayLetterPhoneme;
        public TextRender LetterScoreText;
        public GameObject VariationsContainer;

        [Header("Prefabs")]
        public GameObject DiacriticSymbolItemPrefab;

        public void Init(LetterInfo letterInfo)
        {
            string positionsString = "";
            foreach (var p in letterInfo.data.GetAvailableForms()) {
                positionsString = positionsString + " " + p;
            }
            //Debug.Log("Detail Letter :" + currentLetter.data.Id + " [" + positionsString + " ]");
            AudioManager.I.PlayLetter(letterInfo.data);

            MainLetterDisplay.Init(letterInfo.data);

            BtnPlayLetterName.SetActive(AudioManager.I.GetAudioClip(letterInfo.data, LetterDataSoundType.Name) != null);
            BtnPlayLetterPhoneme.SetActive(AudioManager.I.GetAudioClip(letterInfo.data, LetterDataSoundType.Phoneme) != null);

            LetterScoreText.text = "Score: " + letterInfo.score;

            foreach (Transform t in VariationsContainer.transform) {
                Destroy(t.gameObject);
            }
            var letterbase = letterInfo.data.Id;
            var variationsletters = AppManager.I.DB.FindLetterData(
                (x) => (x.BaseLetter == letterbase && (x.Kind == LetterDataKind.DiacriticCombo || x.Kind == LetterDataKind.LetterVariation))
            );

            var letterGO = Instantiate(DiacriticSymbolItemPrefab);
            letterGO.transform.SetParent(VariationsContainer.transform, false);
            letterGO.GetComponent<ItemDiacriticSymbol>().Init(null);

            foreach (var letter in variationsletters) {
                letterGO = Instantiate(DiacriticSymbolItemPrefab);
                letterGO.transform.SetParent(VariationsContainer.transform, false);
                letterGO.GetComponent<ItemDiacriticSymbol>().Init(letter);
            }
        }
    }
}
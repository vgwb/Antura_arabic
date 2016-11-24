using UnityEngine;
using System.Collections;
using TMPro;

namespace EA4S.DancingDots
{

	public enum LivingLetterAnim {
		Nothing = 0,
		idle = 1,
		hold = 2,
		run = 3,
		walk = 4,
		ninja = 5
	}

	public class DancingDotsLivingLetter : MonoBehaviour
	{

		[Header("References")]
		public LetterObjectView letterObjectView;
		public TextMeshPro hintText;
		public TextMeshPro dotlessText;
		public GameObject fullTextGO, contentGO;

		public GameObject rainbow;
		public DancingDotsGame game;

		TextMeshPro fullText;

		ILivingLetterData letterData;

		void Start()
		{

			fullText = fullTextGO.GetComponent<TextMeshPro>();
			HideRainbow();
			PlayAnimation();
		}

		public void Reset()
		{
			RandomLetter();
			SpeakLetter();
		}

		void PlayAnimation()
		{
			letterObjectView.SetState(LLAnimationStates.LL_dancing);
		}
			
		void RandomLetter()
		{
			letterData = game.questionsManager.getNewLetter();
			SetupLetter();
		}

		void SetupLetter()
		{

            DancingDotsGame.instance.currentLetter = letterData.TextForLivingLetter;

			string lettersWithOneDot = "ج خ غ ف ض ب ن ز ذ ظ";
			string lettersWithTwoDots = "ة ق ي ت";
			string lettersWithThreeDots = "ث ش";

			if (lettersWithThreeDots.Contains(letterData.TextForLivingLetter))
			{
                DancingDotsGame.instance.dotsCount = 3;
			}
			else if (lettersWithTwoDots.Contains(letterData.TextForLivingLetter))
			{
                DancingDotsGame.instance.dotsCount = 2;
			}
			else if (lettersWithOneDot.Contains(letterData.TextForLivingLetter))
			{
                DancingDotsGame.instance.dotsCount = 1;
			}
			else
			{
                DancingDotsGame.instance.dotsCount = 0;
			}
					
			hintText.text = letterData.TextForLivingLetter;
			ShowText(hintText, DancingDotsGame.instance.dotHintAlpha);
			dotlessText.text = letterData.TextForLivingLetter;
			fullText.text = letterData.TextForLivingLetter;
			fullTextGO.SetActive(false);

		}

		public void HideText(TextMeshPro tmp)
		{
			tmp.color = DancingDotsGame.instance.SetAlpha(tmp.color,0);
		}

		public void ShowText(TextMeshPro tmp, byte alpha)
		{
			tmp.color = DancingDotsGame.instance.SetAlpha(tmp.color, alpha);
		}

		public void ShowRainbow()
		{
			rainbow.SetActive(true);
		}

		public void HideRainbow()
		{
			rainbow.SetActive(false);
		}

		private void SpeakLetter()
		{
			if (letterData != null && !game.isTutRound) {
				AudioManager.I.PlayLetter(letterData.Id);
			}
		}

//		public void HideAllText()
//		{
//			hintText.color = SetAlpha(hintText.color,0);
//			dotlessText.color = SetAlpha(dotlessText.color,0);
//			fullText.color = SetAlpha(fullText.color,0);
//		}
//
//		public void ShowAllText()
//		{
//			hintText.color = SetAlpha(hintText.color,DancingDotsGameManager.instance.dotHintAlpha);
//			dotlessText.color = SetAlpha(dotlessText.color,255);
//			fullText.color = SetAlpha(fullText.color,255);
//		}

	}
}
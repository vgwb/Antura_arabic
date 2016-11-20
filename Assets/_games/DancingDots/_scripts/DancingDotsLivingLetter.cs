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
		[Header("Behavior")]
		public bool ClickToChangeAnimation = false;
		public bool ClickToChangeLetter = false;
		public bool ClickToSpeakLetter = false;

		[Header("Starting State")]
		public LetterObjectView toDelete;
		public bool ShowLetter = false;

		[Header("References")]
		public LetterObjectView letterObjectView;
		public TextMeshPro hintText;
		public TextMeshPro dotlessText;
		public GameObject fullTextGO;

		public GameObject rainbow;
		public DancingDotsGameManager game;

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

			if (ShowLetter) {
				RandomLetter();
				SpeakLetter();
			} else {
				hintText.text = "";
				dotlessText.text = "";
				fullText.text = "";
				fullTextGO.SetActive(false);
			}
		}

		void PlayAnimation()
		{
			letterObjectView.SetState(LLAnimationStates.LL_dancing);
		}

		public void Clicked()
		{
			if (ClickToChangeAnimation)
				RandomAnimation();

			if (ClickToChangeLetter)
				RandomLetter();

			if (ClickToSpeakLetter)
				SpeakLetter();
		}
			
		void RandomLetter()
		{

//			string lettersWithDots = "ض ث ق ف غ خ ج ة ن ت ب ي ش ظ ذ ز";
//			do
//			{
//				letterData = AppManager.Instance.Teacher.GetRandomTestLetterLL();
//			} 
//			while (!lettersWithDots.Contains(letterData.TextForLivingLetter) || 
//				letterData.TextForLivingLetter == DancingDotsGameManager.instance.currentLetter);

			letterData = game.questionsManager.getNewLetter();

			SetupLetter();
		}

		void SetupLetter()
		{

			DancingDotsGameManager.instance.currentLetter = letterData.TextForLivingLetter;

			string lettersWithOneDot = "ج خ غ ف ض ب ن ز ذ ظ";
			string lettersWithTwoDots = "ة ق ي ت";
			string lettersWithThreeDots = "ث ش";

			if (lettersWithThreeDots.Contains(letterData.TextForLivingLetter))
			{
				DancingDotsGameManager.instance.dotsCount = 3;
			}
			else if (lettersWithTwoDots.Contains(letterData.TextForLivingLetter))
			{
				DancingDotsGameManager.instance.dotsCount = 2;
			}
			else if (lettersWithOneDot.Contains(letterData.TextForLivingLetter))
			{
				DancingDotsGameManager.instance.dotsCount = 1;
			}
			else
			{
				DancingDotsGameManager.instance.dotsCount = 0;
			}
					
			hintText.text = letterData.TextForLivingLetter;
			ShowText(hintText, DancingDotsGameManager.instance.dotHintAlpha);
			dotlessText.text = letterData.TextForLivingLetter;
			fullText.text = letterData.TextForLivingLetter;
			fullTextGO.SetActive(false);

		}

		public void HideText(TextMeshPro tmp)
		{
			tmp.color = DancingDotsGameManager.instance.SetAlpha(tmp.color,0);
		}

		public void ShowText(TextMeshPro tmp, byte alpha)
		{
			tmp.color = DancingDotsGameManager.instance.SetAlpha(tmp.color, alpha);
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
			if (letterData != null) {
				AudioManager.I.PlayLetter(letterData.Id);
			}
		}

		void RandomAnimation()
		{
			// TODO delte
		}

		string GetStateName(LivingLetterAnim state)
		{
			var stateName = "";
			switch (state) {
			case LivingLetterAnim.Nothing:
				stateName = "";
				break;
			case LivingLetterAnim.idle:
				stateName = "idle";
				break;
			case LivingLetterAnim.hold:
				stateName = "hold";
				break;
			case LivingLetterAnim.run:
				stateName = "run";
				break;
			case LivingLetterAnim.walk:
				stateName = "walk";
				break;
			case LivingLetterAnim.ninja:
				stateName = "ninja";
				break;
			}
			return stateName;
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
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
		public bool DisableAnimator = false;

		[Header("Starting State")]
		public LivingLetterAnim AnimationState;
		public bool ShowLetter = false;

		[Header("References")]
		public Animator LivingLetterAnimator;
		public TextMeshPro hintText;
		public TextMeshPro dotlessText;
		public GameObject fullTextGO;

		public GameObject rainbow;

		TextMeshPro fullText;

		LetterData letterData;

		void Start()
		{

			fullText = fullTextGO.GetComponent<TextMeshPro>();

			if (DisableAnimator) {
				LivingLetterAnimator.enabled = false;
			}

			PlayAnimation();
//			Reset();
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
			if (!DisableAnimator) {
				if (AnimationState != LivingLetterAnim.Nothing) {
					LivingLetterAnimator.Play(GetStateName(AnimationState));
				} else {
					LivingLetterAnimator.StopPlayback();
				}
			}
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

			string lettersWithDots = "ض ث ق ف غ خ ج ة ن ت ب ي ش ظ ذ ز";

			do
			{
				letterData = AppManager.Instance.Teacher.GimmeARandomLetter();

			} 
			// Check if letter in dotted letters and  
			// Check DancingDotsGameManager.instance.currentLetter so that previous letter is not repeated
			while (!lettersWithDots.Contains(letterData.TextForLivingLetter) || 
				letterData.TextForLivingLetter == DancingDotsGameManager.instance.currentLetter);

			SetupLetter();
		}

		void SetupLetter()
		{
			//			string[] diacritics = {"َ","ِ","ُ","ْ"};
			//			int random = UnityEngine.Random.Range(0, diacritics.Length);
			//			hintDotText.text = ArabicSupport.ArabicFixer.Fix(letterData.TextForLivingLetter+ diacritics[random], true, true);

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

		private Color32 SetAlpha(Color32 color, byte alpha)
		{
			if (alpha >= 0 && alpha <= 255)
			{
				return new Color32(color.r, color.g, color.b, alpha);
			}
			else
			{
				return color;
			}
		}

		public void HideText(TextMeshPro tmp)
		{
			tmp.color = SetAlpha(tmp.color,0);
		}

		public void ShowText(TextMeshPro tmp, byte alpha)
		{
			tmp.color = SetAlpha(tmp.color, alpha);
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
				AudioManager.I.PlayLetter(letterData.Key);
			}
		}

		void RandomAnimation()
		{
			LivingLetterAnim newAnimationState = LivingLetterAnim.Nothing;

			while ((newAnimationState == LivingLetterAnim.Nothing) || (newAnimationState == AnimationState)) {
				newAnimationState = GenericUtilites.GetRandomEnum<LivingLetterAnim>();
			}

			AnimationState = newAnimationState;
			PlayAnimation();
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
	}
}
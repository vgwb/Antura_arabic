using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using TMPro;

using System;
using EA4S;

namespace EA4S.DancingDots
{
	public enum Diacritic { Sokoun, Fatha, Dameh, Kasrah };

	public class DancingDotsGameManager : MiniGameBase {

		public static DancingDotsGameManager instance;

		public DancingDotsGamePlayInfo GameplayInfo;


		public Animator dotsMenu;
		public Animator diacriticMenu;

		public DancingDotsLivingLetter livingLetter;
		public GameObject[] diacritics;
		public DancingDotsDiacriticPosition activeDiacritic;

		public string currentLetter = "";
		private int _dotsCount;
		public int dotsCount
		{
			get 
			{
				return _dotsCount;
			}
			set
			{
				_dotsCount = value;
				foreach (DancingDotsDraggableDot dd in dragableDots)
				{
					dd.isNeeded = dd.dots == _dotsCount;
				}
			}
		}


		[Range(0, 255)] public byte dotHintAlpha = 60;
		[Range(0, 255)] public byte diacriticHintAlpha = 60;


		public DancingDotsDraggableDot[] dragableDots;
		public DancingDotsDraggableDot[] dragableDiacritics;

		private bool isCorrectDot = false;
		private bool isCorrectDiacritic = false;

		protected override void Awake()
		{
			base.Awake();
			instance = this;


		}

		protected override void Start()
		{

			base.Start();


			AppManager.Instance.InitDataAI();
			AppManager.Instance.CurrentGameManagerGO = gameObject;
			SceneTransitioner.Close();

//			StartCoroutine(ShowMenu(dotsMenu));

		}

		public void StartRound()
		{
			foreach (DancingDotsDraggableDot dDots in dragableDots) dDots.Reset();

			foreach (DancingDotsDraggableDot dDiacritic in dragableDiacritics) dDiacritic.Reset();

			livingLetter.Reset();

			isCorrectDot = false;

			isCorrectDiacritic = false;

			RandomDiacritic();
		}

		IEnumerator ShowMenu(Animator animator)
		{
			yield return new WaitForSeconds(0.5f);
			animator.SetTrigger("Show");


//			yield return new WaitForSeconds(3f);
//			StartCoroutine(SwitchMenus(dotsMenu, diacriticMenu));

		}

		IEnumerator SwitchMenus(Animator menu1, Animator menu2)
		{
			yield return new WaitForSeconds(0.5f);
			menu1.SetTrigger("Hide");
			menu2.SetTrigger("Show");
		}

		protected override void ReadyForGameplay()
		{
			base.ReadyForGameplay();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void OnMinigameQuit()
		{
			base.OnMinigameQuit();
		}

		public IEnumerator RandomDiacritic() {


			foreach (GameObject go in diacritics)
			{
				go.SetActive(false);
				Color32 currentColor = go.GetComponent<TextMeshPro>().color;
				go.GetComponent<TextMeshPro>().color = new Color32(currentColor.r,currentColor.g,currentColor.b,0);
			}


			int random = UnityEngine.Random.Range(0, diacritics.Length);
			activeDiacritic = diacritics[random].GetComponent<DancingDotsDiacriticPosition>();
		
			diacritics[random].SetActive(true);

			foreach (DancingDotsDraggableDot dd in dragableDiacritics)
			{
				dd.isNeeded = activeDiacritic.diacritic == dd.diacritic;
			}

			// wait for end of frame to ge correct values for meshes
			yield return new WaitForEndOfFrame();
			activeDiacritic.CheckPosition();

			Color32 oldColor = activeDiacritic.GetComponent<TextMeshPro>().color;
			activeDiacritic.GetComponent<TextMeshPro>().color = new Color32(oldColor.r,oldColor.g,oldColor.b,diacriticHintAlpha);

		}

		IEnumerator CorrectMove(bool isWon)
		{
			yield return new WaitForSeconds(0.25f);
			livingLetter.LivingLetterAnimator.Play("run");
			yield return new WaitForSeconds(0.5f);
			livingLetter.LivingLetterAnimator.Play("walk");

			if (isWon) StartCoroutine(RoundWon());
		}


		public void CorrectDot()
		{
			// Change font or show correct character
			isCorrectDot = true;

			livingLetter.fullTextGO.SetActive(true);

			StartCoroutine(CorrectMove(isCorrectDiacritic));
		}

		public void CorrectDiacritic()
		{

			isCorrectDiacritic = true;

			activeDiacritic.GetComponent<TextMeshPro>().color = new Color32(0,0,0,255);

			StartCoroutine(CorrectMove(isCorrectDot));

		}

		IEnumerator RoundWon()
		{
			livingLetter.LivingLetterAnimator.Play("ninja");
			yield return new WaitForSeconds(0.5f);
			livingLetter.GetComponent<Animator>().Play("Pirouette");
			yield return new WaitForSeconds(0.5f);
			StartRound();
			yield return new WaitForSeconds(1f);
			livingLetter.LivingLetterAnimator.Play("walk");


		}

	}



	[Serializable]
	public class DancingDotsGamePlayInfo: AnturaGameplayInfo
	{
		[Tooltip("Play session duration in seconds.")]
		public float PlayTime = 0f;
	}
}

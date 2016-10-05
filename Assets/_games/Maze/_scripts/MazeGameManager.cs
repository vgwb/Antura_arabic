using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ModularFramework.Core;
using ModularFramework.Helpers;
using EA4S;


namespace EA4S.Maze
{
	public class MazeGameManager : MiniGameBase
	{
		
		public static MazeGameManager Instance;
		public MazeGameplayInfo GameplayInfo;


		public bool tutorialForLetterisComplete = false;

		protected override void Awake()
		{
			base.Awake();
			Instance = this;
		}

		protected override void Start()
		{
			base.Start();

			AppManager.Instance.InitDataAI();
			AppManager.Instance.CurrentGameManagerGO = gameObject;
			SceneTransitioner.Close();


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

	}

	[Serializable]
	public class MazeGameplayInfo : AnturaGameplayInfo
	{
		[Tooltip("Play session duration in seconds.")]
		public float PlayTime = 0f;
	}
}

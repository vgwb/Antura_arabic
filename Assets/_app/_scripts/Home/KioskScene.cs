using Antura.Audio;
using Antura.Core;
using Antura.Database;
using Antura.Dog;
using Antura.LivingLetters;
using Antura.UI;
using UnityEngine;

namespace Antura.Scenes
{
    /// <summary>
    /// Controls the _Start scene, providing an entry point for all users prior to having selected a player profile. 
    /// </summary>
    public class KioskScene : SceneBase
    {
        [Header("Setup")]
        public AnturaAnimationStates AnturaAnimation = AnturaAnimationStates.sitting;
        public LLAnimationStates LLAnimation = LLAnimationStates.LL_dancing;

        [Header("References")]
        public AnturaAnimationController AnturaAnimController;
        public LivingLetterController LLAnimController;

        protected override void Start()
        {
            base.Start();
            GlobalUI.ShowPauseMenu(true, PauseMenuType.StartScreen);
            AudioManager.I.PlaySound(Sfx.GameTitle);

            AnturaAnimController.State = AnturaAnimation;
            LLAnimController.State = LLAnimation;

        }

        public void OnBtnPlay()
        {

        }

        public void OnBtnDonate()
        {

        }
    }
}
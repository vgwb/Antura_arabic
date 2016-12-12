using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace EA4S.Assessment
{
    public class AssessmentResultAntura: MonoBehaviour
    {
        [Header("Prefabs")]
        public AnturaAnimationController anturaPrefab;
        public GameObject poof;

        [Header("Positions")]
        public Transform AnturaPosition;

        #region Instance
        /////////////////
        // Singleton Pattern: Ok as long as factory has not other singletons as dependencies
        static AssessmentResultAntura instance;
        public static AssessmentResultAntura Instance
        {
            get
            {
                return instance;
            }
        }


        void Awake()
        {
            instance = this;
        }
        /////////////////
        #endregion

        public void StartAnimation( Action callback)
        {
            Coroutine.Start( AnimationCoroutine( callback));
        }

        private IEnumerator AnimationCoroutine( Action callback)
        {

            var particles =
            Instantiate( poof, AnturaPosition.position, AnturaPosition.rotation)
                as GameObject;

            var particles2 =
           Instantiate(poof, AnturaPosition.position, AnturaPosition.rotation)
               as GameObject;

            particles2.transform.localScale = new Vector3( 2, 2, 2);
            particles2.SetLayerRecursive( AnturaLayers.ModelsOverUI);
            particles.transform.localScale = new Vector3( 2, 2, 2);
            particles.SetLayerRecursive( AnturaLayers.ModelsOverUI);

            AssessmentConfiguration.Instance.Context.GetAudioManager().PlaySound( Sfx.Poof);

            yield return TimeEngine.Wait( 0.15f);

            AnturaAnimationController antura =
            Instantiate( anturaPrefab, AnturaPosition.position, AnturaPosition.rotation)
                as AnturaAnimationController;

            antura.gameObject.AddComponent<BringAnturaInFlagSpace>();

            antura.transform.localScale = Vector3.zero;
            antura.transform.DOScale( Vector3.one, 0.2f);

            yield return TimeEngine.Wait( 0.9f);
            antura.DoShout(
                () => AssessmentConfiguration.Instance.Context.GetAudioManager()
                            .PlaySound( Sfx.DogBarking)
                );

            yield return TimeEngine.Wait(1.2f);
            particles2 =
           Instantiate(poof, AnturaPosition.position, AnturaPosition.rotation)
               as GameObject;

            particles2.transform.localScale = new Vector3(2, 2, 2);
            particles2.SetLayerRecursive( AnturaLayers.ModelsOverUI);
            yield return TimeEngine.Wait( 1f);
            antura.transform.DOScale( Vector3.zero, 0.2f).SetEase(Ease.InExpo);
            callback();
        }
    }
}

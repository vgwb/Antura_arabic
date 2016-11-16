using System;
using UnityEngine;

namespace EA4S.Assessment
{
    public class LivingLetterFactory: MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject LivingLetterPrefab = null;
        public GameObject DropZonePrefab = null;

        [Header("Folders")]
        public GameObject Questions = null;
        public GameObject Answers = null;
        public GameObject Placeholders = null;

        public LetterObjectView SpawnQuestion( ILivingLetterData data)
        {
            // Organize LLs in inspector (just aestetical change)
            var go = SpawnLivingLetter( data);
            go.transform.SetParent( Questions.transform);
            return go;
        }

        public LetterObjectView SpawnAnswer( ILivingLetterData data)
        {
            // Organize LLs in inspector (just aestetical change)
            var go = SpawnLivingLetter( data);
            go.transform.SetParent( Answers.transform);
            return go;
        }

        private LetterObjectView SpawnLivingLetter( ILivingLetterData data)
        {
            var letter = (Instantiate( LivingLetterPrefab) as GameObject)
                    .GetComponent< LetterObjectView>();

            letter.Init(data);

            // Correct rotation but scale = 0 to hide
            letter.transform.localRotation = Quaternion.Euler( 0, 180f, 0);
            letter.transform.localScale = Vector3.zero;
            
            // We want to center on body, not on legs (that are hided anyway)
            FixShiftInLetter( letter.gameObject);
            letter.SetState( LLAnimationStates.LL_limbless);
            letter.GetComponent< Animator>().enabled = false;

            return letter;
        }

        // LL are not centered on Glyph center, but on legs, but we do not have legs so..
        private void FixShiftInLetter( GameObject go)
        {
            var child = go.transform.GetChild( 0);
            child.localPosition = new Vector3( 0, -3.5f, 0);
        }

        void Awake()
        {
            instance = this;
        }

        static LivingLetterFactory instance;
        public static LivingLetterFactory Instance
        {
            get
            {
                return instance;
            }
        }

        public GameObject SpawnCustomElement( CustomElement element)
        {
            switch (element)
            {
                case CustomElement.Placeholder:
                    return SpawnPlaceHolder();

                default:
                    throw new NotImplementedException( "Not implemented yet!");
            }
        }

        private GameObject SpawnPlaceHolder()
        {
            return ( Instantiate(DropZonePrefab) as GameObject);    
        }
    }
}

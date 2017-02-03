using EA4S.MinigamesAPI;
using System;
using UnityEngine;

namespace EA4S.Assessment
{
    public class LivingLetterFactory: MonoBehaviour
    {
        [Header( "Prefabs")]
        public GameObject StillLetterBox = null;
        public GameObject DropZonePrefab = null;

        [Header( "Folders")]
        public GameObject Questions = null;
        public GameObject Answers = null;
        public GameObject Placeholders = null;

        private int counter = 0;

        public StillLetterBox SpawnQuestion( ILivingLetterData data)
        {
            // Organize LLs in inspector's hierarchy view
            var letter = SpawnStillLetter( Questions);
            letter.Init( data, false);

            return letter;
        }

        public Answer SpawnAnswer( ILivingLetterData data, bool correct, AssessmentDialogues dialogues)
        {
            // Organize LLs in inspector's hierarchy view
            var letter = SpawnStillLetter( Answers);

            // Link LL to answer
            var answ = letter.gameObject.AddComponent< Answer>();
            letter.Init( data, true);
            answ.Init( correct, dialogues, data);
            return answ;
        }

        public StillLetterBox SpawnPlaceholder( LivingLetterDataType type)
        {
            // Organize LLs in inspector's hierarchy view
            var letter = SpawnStillLetter( Placeholders);
            letter.InitAsSlot( type);
            letter.gameObject.AddComponent< PlaceholderBehaviour>();
            return letter;
        }

        private StillLetterBox SpawnStillLetter( GameObject parent_Folder)
        {
            counter++;
            var letter = (Instantiate( StillLetterBox) as GameObject)
                    .GetComponent< StillLetterBox>();

            letter.gameObject.name= "instance_" + counter;
            letter.InstaShrink();
            letter.transform.SetParent( parent_Folder.transform);
            
            return letter;
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
            return ( Instantiate( DropZonePrefab) as GameObject);    
        }
    }
}

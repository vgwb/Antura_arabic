using EA4S.MinigamesAPI;
using System.Collections.Generic;
using UnityEngine;

namespace EA4S.Assessment
{
    public class LivingLetterFactory: MonoBehaviour
    {
        [Header( "Prefabs")]
        public GameObject StillLetterBox = null;
        public GameObject QuestionBoxPrefab = null;

        [Header( "Folders")]
        public GameObject Questions = null;
        public GameObject Answers = null;
        public GameObject Placeholders = null;
        public GameObject QuestionBoxes = null;

        private int counter = 0;

        public StillLetterBox SpawnQuestion( ILivingLetterData data)
        {
            // Organize LLs in inspector's hierarchy view
            var letter = SpawnStillLetter( Questions);
            letter.Init( data, false);

            return letter;
        }

        public Answer SpawnAnswer( ILivingLetterData data, bool correct, AssessmentAudioManager dialogues)
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

        public QuestionBox SpawnQuestionBox( IEnumerable< StillLetterBox> letterBoxes)
        {
            counter++;
            var qbox = (Instantiate( QuestionBoxPrefab) as GameObject)
                    .GetComponent< QuestionBox>();

            qbox.gameObject.name = "instance_" + counter;
            qbox.HideInstant();
            qbox.transform.SetParent( QuestionBoxes.transform);
            qbox.WrapBoxAroundWords( letterBoxes);
            return qbox;
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
    }
}

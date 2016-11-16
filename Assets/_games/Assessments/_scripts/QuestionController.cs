using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace EA4S.Assessment
{
    // TODO: move eastetic stuff to QuestionView
    public class QuestionController: MonoBehaviour
    {
        internal enum QuestionControllerStatus
        {
            CleaningUp,
            Cleaned,
            SpawningLetters,
            Idle
        }

        public IPositionsProvider positions;

        [Header("Prefabs")]
        public GameObject limblessLetter;
        public GameObject dropZone;

        private Transform letters = null;
        private readonly string lettersName = "Letters";

        private List< IQuestionPack> questionPacks;
        private List< LetterObjectView> livingObjects;

        /// <summary>
        /// Remove All Living Letters  from scene
        /// </summary>
        public void Cleanup()
        {
            state = QuestionControllerStatus.CleaningUp;
            StartCoroutine( DestroyOldLetters() );           
        }

        /// <summary>
        /// True if Living Letters are animating
        /// </summary>
        public bool IsAnimating()
        {
            return state == QuestionControllerStatus.CleaningUp ||
                state == QuestionControllerStatus.SpawningLetters;
        }

        QuestionControllerStatus state;

        public IAudioManager audioManager { get; internal set; }
        public DragManager dragManager { get; internal set; }

        IEnumerator DestroyOldLetters()
        {
            // TODO: detach from Drag Manager
            foreach (var zone in dropZones)
                Destroy(zone.gameObject);

            foreach ( var ll in livingObjects)
            {
                yield return TimeEngine.Wait( Random.Range( 0.07f, 0.14f));

                // Say goodbye..
                audioManager.PlaySound( Sfx.Poof);
                ll.Poof( ElementsSize.PoofOffset);
                ll.transform.DOScale( 0, 0.4f);
            }

            yield return TimeEngine.Wait( 1.0f);

            Destroy( letters.gameObject);

            Reset();
            state = QuestionControllerStatus.Cleaned;
        }

        public void Reset()
        {
            positions.Reset();
            questionPacks = new List< IQuestionPack>();
            livingObjects = new List< LetterObjectView>();
            dropZones = new List< GameObject>();
        }

        public void AddQuestion( IQuestionPack questionPack )
        {
            questionPacks
                .Add( questionPack);
        }

        public void SpawnAllLivingLetters( ScoreCounter score)
        {
            state = QuestionControllerStatus.SpawningLetters;
            StartCoroutine( SpawnLettersCoroutine(score));
        }

        private IEnumerator SpawnLettersCoroutine( ScoreCounter score)
        {
            score.ResetRound();

            letters = (new GameObject( lettersName)).transform;

            foreach (var question in questionPacks)
                positions.AddQuestionPack( question);

            var lettersPositions = positions.GetPositions().Positions;

            int index = 0;
            int groupID = 1;
            foreach (var question in questionPacks)
            {
                var pos = lettersPositions[index++];
                yield return SpawnQuestion( question.GetQuestion(), pos.QuestionPivots[0]);

                int i = 0;
                
                foreach (var p in question.GetCorrectAnswers())
                {
                    //TODO: for now we spawn same letter for debugging purpose. Instead
                    // of a living Letter a placeholder should be something else (unless LL
                    // can be configured to become a placeholder).
                    SpawnDropZone( pos.PlaceholderPivots[i], groupID);
                    yield return SpawnLetter( p, pos.CorrectPivots[i++], groupID);
                }

                score.AddGroup( groupID, i);

                groupID++;
                int j = 0;
                foreach (var p in question.GetWrongAnswers())
                {
                    yield return SpawnLetter( p, pos.WrongPivots[j++], 0);
                }
            }
            state = QuestionControllerStatus.Idle;
        }


       

        private CustomYieldInstruction SpawnQuestion( ILivingLetterData data, Position position)
        {
            var letter = SpawnLivingLetter( data, position);

            return TimeEngine.Wait( Random.Range( 0.07f, 0.14f));
        }


        private List<GameObject> dropZones;

        /// <summary>
        /// Create a place where to drop correct answers
        /// </summary>
        /// <param name="position"></param>
        /// <param name="groupID"></param>
        private void SpawnDropZone(Position position, int groupID)
        {
            var zone = (Instantiate(dropZone) as GameObject)
                .GetComponent<DropZone>();

            zone.transform.LocalFrom(position);
            zone.transform.localScale = new Vector3(2f, 2f, 2f);
            dropZones.Add( zone.gameObject);

            dragManager.DecorateDropZone( zone, groupID);
        }

        /// <summary>
        /// Spawn a limbless letter at given position
        /// </summary>
        /// <param name="position">Position (World space)</param>
        /// <returns>Letter View</returns>
        private CustomYieldInstruction SpawnLetter( ILivingLetterData data, Position position, int groupID)
        {
            var letter = SpawnLivingLetter( data, position);
            dragManager.DecorateLivingLetter( letter, groupID);

            return TimeEngine.Wait( Random.Range( 0.07f, 0.14f));
        }

        private LetterObjectView SpawnLivingLetter( ILivingLetterData data, Position position)
        {
            var letter = (Instantiate(limblessLetter) as GameObject)
                    .GetComponent< LetterObjectView>();

            letter.Init(data);
            letter.transform.LocalFrom( position);
            FixShiftInLetter( letter.gameObject);
            letter.SetState( LLAnimationStates.LL_limbless);
            letter.transform.SetParent( letters, true);

            letter.transform.localScale = Vector3.zero;
            letter.transform.DOScale(1, 0.4f);
            letter.GetComponent< Animator>().enabled = false;
            letter.Poof( ElementsSize.PoofOffset);
            audioManager.PlaySound( Sfx.Poof);
            livingObjects.Add( letter);

            return letter;
        }

        private void FixShiftInLetter( GameObject go)
        {
            var child = go.transform.GetChild(0);
            child.localPosition = new Vector3(0, -3.5f, 0);
        }
    }
}

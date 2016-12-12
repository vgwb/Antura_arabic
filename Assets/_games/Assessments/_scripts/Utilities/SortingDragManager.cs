using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EA4S.Assessment
{
    internal class SortingDragManager : IDragManager, ITickable
    {
        private IAudioManager audioManager;
        private ICheckmarkWidget widget;

        public SortingDragManager( IAudioManager audioManager, ICheckmarkWidget widget)
        {
            this.audioManager = audioManager;
            this.widget = widget;
            ResetRound();
        }

        private bool dragOnly = false;
        public void EnableDragOnly()
        {
            dragOnly = true;
            ticking = true;
            TimeEngine.AddTickable(this);
            foreach (var a in answers)
                a.Enable();
        }

        List< SortableBehaviour> answers = null;

        // This should be called onlye once
        public void AddElements(
                                    List< PlaceholderBehaviour> placeholders,
                                    List< AnswerBehaviour> answers,
                                    List< IQuestion> questions)
        {
            this.answers = BehaviourFromAnswers( answers);
        }

        private bool searchForBuckets = false;
        private bool objectFlying = false;
        private bool returnedAllAnswered = false;

        private void FindBuckets()
        {
            // Sorted array by position
            // Ascending =>     X--->
            var positions = UnityEngine.Object.FindObjectsOfType< SortingTicket>()
                .OrderByDescending( x => x.transform.position.x).ToArray();

            this.positions = new Vector3[positions.Length];

            for (int i = 0; i < positions.Length; i++)
                this.positions[i] = positions[i].transform.localPosition;

            sortables = new SortableBehaviour[ positions.Length];
            for (int i = 0; i < sortables.Length; i++)
                sortables[i] = null;

            searchForBuckets = false;
        }

        private List< SortableBehaviour> BehaviourFromAnswers( List< AnswerBehaviour> answers)
        {
            var list = new List< SortableBehaviour>();

            foreach (var a in answers)
            {
                var droppable = a.gameObject.AddComponent< SortableBehaviour>();
                droppable.SetDragManager( this);
                list.Add( droppable);
            }

            return list;
        }

        IEnumerator AllCorrectCoroutine()
        {
            audioManager.PlaySound( Sfx.StampOK);
            yield return TimeEngine.Wait(0.4f);
            widget.Show(true);
            yield return TimeEngine.Wait(1.0f);
        }

        public bool AllAnswered()
        {
            if (dragOnly || objectFlying || returnedAllAnswered)
                return false; // When antura is animating we should not complete the assessment

            if (sortables == null)
                return false;

            IAnswer[] answer = new IAnswer[ answers.Count];
            IAnswer[] answerSorted = new IAnswer[ answers.Count];

            //Answers like are actually sorted
            var sorted = sortables.OrderByDescending( x => x.transform.position.x).ToArray();

            int index = 0;
            foreach( var a in answers)
            {
                answerSorted[ index] = sorted[ index].gameObject.GetComponent< AnswerBehaviour>().GetAnswer();
                index++;
            }

            // Answers sorted by ticket
            answer = answerSorted.OrderBy( a => a.GetTicket()).ToArray();

            for(int i=0; i<answer.Length; i++)
            {
                Debug.Log(  "  answer:" + answer[i].Data().Id +
                            "  sorted:" + answerSorted[i].Data().Id);
            }

            for(int i=0; i < answer.Length; i++)
            {
                //If sorted version has letter shapes in wrong positions we know we didn't find solution
                if (answer[i].Equals( answerSorted[i]) == false)
                    return false;
            }

            //Debug.Log("Return TRUE");
            // two words identical!
            returnedAllAnswered = true;
            Coroutine.Start( AllCorrectCoroutine());
            return true;
        }

        public void Enable()
        {
            dragOnly = false;
        }

        public void ResetRound()
        {
            answers = null;
        }

        IDroppable droppable = null;

        // ALL NEEDED EVENTS ARE HERE
        public void StartDragging( IDroppable droppable)
        {
            if (this.droppable != null)
                return;

            objectFlying = true;
            audioManager.PlaySound( Sfx.UIPopup);
            this.droppable = droppable;
            droppable.StartDrag ( x => RemoveFromUpdate());
        }

        void RemoveFromUpdate()
        {
            droppable.StopDrag();
            droppable = null;
        }

        public void StopDragging( IDroppable droppable)
        {
            objectFlying = false;
            if (this.droppable == droppable && droppable != null)
            {
                audioManager.PlaySound( Sfx.UIPopup);
                RemoveFromUpdate();
                MoveStuffToPosition();
            }
        }

        bool ticking = false;

        Vector3[] positions;
        SortableBehaviour[] sortables;

        public bool Update(float deltaTime)
        {
            if (searchForBuckets)
                FindBuckets();

            if (droppable != null)
            {
                var pos = Camera.main.ScreenToWorldPoint( Input.mousePosition);
                pos.z = 5;
                droppable.GetTransform().localPosition = pos;
            }

            MoveStuffToPosition();

            return !ticking;
        }

        private void MoveStuffToPosition()
        {
            if (sortables == null)
                return;

            if (returnedAllAnswered)
                return;

            sortables = answers.OrderByDescending(x => x.transform.position.x).ToArray();
            for (int i = 0; i < sortables.Length; i++)
            {
                var s = sortables[i];
                //DO NOT TWEEN THE OBJECT WE ARE DRAGGIN
                if ((s as IDroppable) != droppable /*&& s.SetSortIndex(i)*/)
                {
                    s.SetSortIndex(i);
                    s.Move( positions[i], 0.3f);
                }
            }
        }

        public void DisableInput()
        {
            foreach (var a in answers)
                a.Disable();
        }

        public void EnableInput()
        {
            foreach (var a in answers)
                a.Enable();
        }

        public void RemoveDraggables()
        {
            dragOnly = true;
            if (droppable != null)
            {
                droppable.StopDrag();
                droppable = null;
            }
        }

        public void OnAnswerAdded()
        {
            searchForBuckets = true;
            returnedAllAnswered = false;
            sortables = null;
        }
    }
}

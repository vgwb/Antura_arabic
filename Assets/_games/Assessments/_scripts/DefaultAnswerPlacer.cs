using System.Collections;

namespace EA4S.Assessment
{
    public class DefaultAnswerPlacer : IAnswerPlacer
    {
        private bool isAnimating = false;
        public bool IsAnimating()
        {
            return isAnimating;
        }

        public void Place( IAnswer[] answer)
        {
            isAnimating = true;
            Coroutine.Start( PlaceCoroutine( answer));
        }

        public void RemoveAnswers()
        {
            isAnimating = true;
            Coroutine.Start( RemoveCoroutine());
        }

        private IEnumerator PlaceCoroutine( IAnswer[] answers)
        {

            yield return TimeEngine.Wait( 0.65f);
            isAnimating = false;
        }

        private IEnumerator RemoveCoroutine()
        {

            yield return TimeEngine.Wait( 0.65f);
            isAnimating = false;
        }
    }
}

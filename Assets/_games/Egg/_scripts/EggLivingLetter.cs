using UnityEngine;

namespace EA4S.Egg
{
    public class EggLivingLetter : MonoBehaviour
    {
        GameObject letterObjectViewPrefab;

        LetterObjectView livingLetter;

        public void Initialize(GameObject letterObjectViewPrefab)
        {
            this.letterObjectViewPrefab = letterObjectViewPrefab;
        }

        public void PlayIdleAnimation()
        {
            livingLetter.Model.State = LLAnimationStates.LL_idle_1;
        }

        public void PlayWalkAnimation()
        {
            livingLetter.Model.State = LLAnimationStates.LL_walk;
        }

        public void PlayHorrayAnimation()
        {
            livingLetter.Model.State = LLAnimationStates.LL_horray;
        }

        public void SetLetter(ILivingLetterData livingLetterData)
        {
            if(livingLetter != null)
            {
                Destroy(livingLetter.gameObject);
            }

            livingLetter = GameObject.Instantiate(letterObjectViewPrefab).GetComponent<LetterObjectView>();

            livingLetter.transform.SetParent(transform);
            livingLetter.transform.localPosition = Vector3.zero;
            livingLetter.Init(livingLetterData);
        }
    }
}

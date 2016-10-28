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
            //livingLetter.SetState(LLAnimationStates.LL_normal);
            //livingLetter.SetIdle();
        }

        public void PlayWalkAnimation()
        {
            //livingLetter.SetState.State = LLAnimationStates.LL_walk;
            //livingLetter.SetWalking();
        }

        public void PlayHorrayAnimation()
        {
            livingLetter.DoHorray();
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

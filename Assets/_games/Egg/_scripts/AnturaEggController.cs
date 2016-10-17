using UnityEngine;

namespace EA4S.Egg
{
    [RequireComponent(typeof(Antura))]
    public class AnturaEggController : MonoBehaviour
    {
        float barkingTimer = 0.0f;
        bool IsWaken { get { return barkingTimer > 0; } }

        public void Bark()
        {
            //GetComponent<Antura>().IsBarking = true;
            GetComponent<Antura>().SetAnimation(AnturaAnim.StandExcitedBreath);
            barkingTimer = 3f;
        }


        void Update()
        {
            if (IsWaken)
            {
                barkingTimer -= Time.deltaTime;

                if (barkingTimer <= 0)
                {
                    GetComponent<Antura>().SetAnimation(AnturaAnim.SitBreath);
                }
            }
        }
    }
}
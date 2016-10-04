using UnityEngine;

namespace EA4S.Tobogan
{
    [RequireComponent(typeof(Antura))]
    public class AnturaAngerController : MonoBehaviour
    {
        float barkingTimer = 0.0f;
        bool IsWaken { get { return barkingTimer > 0; } }

        public void Bark()
        {
            //GetComponent<Antura>().IsBarking = true;
            GetComponent<Antura>().SetAnimation(AnturaAnim.StandExcitedLookR);
            barkingTimer = 1.4f;
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
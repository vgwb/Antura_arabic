using UnityEngine;

namespace EA4S.Assessment
{
    public class TutorialHelper : MonoBehaviour
    {

        static TutorialHelper instance;
        public static TutorialHelper Instance
        {
            get
            {
                return instance;
            }
        }

        void Awake()
        {
            instance = this;
        }

        public static Vector3 GetWorldPosition()
        {
            return instance.transform.position;
        }

        
    }
}

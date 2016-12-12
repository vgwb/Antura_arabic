using UnityEngine;
using System.Collections;

namespace EA4S.Assessment
{
    /// <summary>
    /// Commodity for calling coroutines from non-MonoBheaviours (that
    /// are guaranteed to be runned after scene start)
    /// </summary>
    public class Coroutine : MonoBehaviour
    {
        public static YieldInstruction Start( IEnumerator coroutine)
        {
            return instance.StartCoroutine( coroutine);
        }

        public static void Stop( IEnumerator coroutine)
        {
            instance.StopCoroutine( coroutine);
        }

        void Awake()
        {
            instance = this;
        }

        static Coroutine instance;
        public static Coroutine Instance
        {
            get
            {
                return instance;
            }
        }
    }

}

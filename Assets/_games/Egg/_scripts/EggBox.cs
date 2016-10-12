using UnityEngine;

namespace EA4S.Egg
{
    public class EggBox : MonoBehaviour
    {
        public Transform[] eggPositions;

        public Vector3[] GetEggLocalPositions()
        {
            Vector3[] eggLocalPositions = new Vector3[eggPositions.Length];

            for(int i=0; i<eggPositions.Length; i++)
            {
                eggLocalPositions[i] = eggPositions[i].localPosition;
            }

            return eggLocalPositions;
        }
    }
}
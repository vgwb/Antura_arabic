using UnityEngine;

namespace EA4S.Assessment
{
    public class DropZone : MonoBehaviour
    {
        private int groupId;
        /// <summary>
        /// If group match, answer is correct
        /// </summary>
        /// <param name="i">number of group (question 1 = group 1, and so on)</param>
        public void SetGroup( int i)
        {
            groupId = i;
        }

        public int GetGroup()
        {
            return groupId;
        }

        public void PlacedOnCorrectPlace()
        {
            Destroy(this);
        }
    }
}

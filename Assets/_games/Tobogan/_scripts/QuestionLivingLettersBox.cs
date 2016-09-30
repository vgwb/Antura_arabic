using UnityEngine;

namespace EA4S.Tobogan
{
    public class QuestionLivingLettersBox : MonoBehaviour
    {
        Vector3 position = new Vector3(-95f, 52.5f, -67.8f);
        Vector3 rotation = new Vector3(167.054f, -136.083f, -180.018f);

        public Transform letterStartPosition;
        public Transform[] lettersPosition;
        public Transform letterEndPosition;

        public Transform upRightMaxPosition;
        public Transform downLeftMaxPosition;
    }
}

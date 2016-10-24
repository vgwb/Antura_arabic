using UnityEngine;
using System.Collections;
using EA4S;

namespace EA4S.TestE
{
    public class MiniMap : MonoBehaviour
    {
        [Header("Letter")]
        public GameObject letter;

        [Header("Cameras")]
        public GameObject[] cameraM;

        [Header("Pines")]
        public Transform[] posPines;
        public int numStepsBetweenPines;
        public GameObject dot;
        public Vector3[] posDots;
        public Vector3 pinLeft, pinRight;
        Quaternion rot;

        void Awake()
        {

            pinLeft = posPines[0].position;
            pinRight = posPines[1].position;

            CalculateStepsBetweenPines(pinLeft, pinRight);

            CameraGameplayController.I.MoveToPosition(cameraM[0].transform.position, cameraM[0].transform.rotation);
        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if(hit.collider.name == "pin-11")
                    {
                        pinLeft = posPines[1].position;
                        pinRight = posPines[2].position;

                        CalculateStepsBetweenPines(pinLeft, pinRight);

                        CameraGameplayController.I.MoveToPosition(cameraM[1].transform.position, cameraM[1].transform.rotation);

                        letter.GetComponent<LetterMovement>().ResetPosLetter(2, pinRight);
                    }

                }
            }
        }
        void CalculateStepsBetweenPines(Vector3 p1, Vector3 p2)
        {
            int i = 0;
            posDots = new Vector3[numStepsBetweenPines];
            float step = 1f / (numStepsBetweenPines + 1);
            for (float perc = step; perc < 1f; perc += step)
            {
                Vector3 v = Vector3.Lerp(p1, p2, perc);
                posDots[i] = v;
                i++;
                rot.eulerAngles = new Vector3(90, 0, 0);
                Instantiate(dot, v, rot);
            }
        }
    }
}

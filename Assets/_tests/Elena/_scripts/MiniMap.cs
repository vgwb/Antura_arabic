using UnityEngine;
using System.Collections;
using EA4S;

namespace EA4S.TestE
{
    public class MiniMap : MonoBehaviour
    {
        public GameObject letter;
        public GameObject[] cameraM;
        public Transform pos1;
        public Transform pos2;
        public Transform pos3;
        public int numSteps;
        public GameObject sphere;
        public Vector3[] posSteps;
        Quaternion rot;

        void Awake()
        {

            Vector3 p1 = pos1.position;
            Vector3 p2 = pos2.position;

            int i = 0;
            posSteps = new Vector3[numSteps];
            float step = 1f / (numSteps + 1);
            for (float perc = step; perc < 1f; perc += step)
            {
                Vector3 v = Vector3.Lerp(p1, p2, perc);
                posSteps[i] = v;
                i++;
                rot.eulerAngles = new Vector3(90, 0, 0);
                Instantiate(sphere, v, rot);
            }

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
                        Vector3 p1 = pos2.position;
                        Vector3 p2 = pos3.position;

                        int i = 0;
                        posSteps = new Vector3[numSteps];
                        float step = 1f / (numSteps + 1);
                        for (float perc = step; perc < 1f; perc += step)
                        {
                            Vector3 v = Vector3.Lerp(p1, p2, perc);
                            posSteps[i] = v;
                            i++;
                            rot.eulerAngles = new Vector3(90, 0, 0);
                            Instantiate(sphere, v, rot);
                        }

                        CameraGameplayController.I.MoveToPosition(cameraM[1].transform.position, cameraM[1].transform.rotation);
                        letter.GetComponent<AnturaMovement>().pos = 0;
                        letter.GetComponent<AnturaMovement>().v = posSteps[0];
                        letter.transform.LookAt(pos3);
                    }

                }
            }
        }
    }
}

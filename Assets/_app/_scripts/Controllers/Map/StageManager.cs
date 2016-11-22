using UnityEngine;
using System.Collections;
using EA4S;
namespace EA4S
{
    public class StageManager : MonoBehaviour
    {
        public GameObject[] stages;
        public GameObject[] cameras;
        public GameObject[] miniMaps;
        public GameObject letter;
        public void StageLeft()
        {
            int numberStage = AppManager.Instance.Player.CurrentJourneyPosition.Stage;
            if(numberStage < 6)
            {
                stages[numberStage].SetActive(false);
                stages[numberStage + 1].SetActive(true);
                ChangeCamera(cameras[numberStage+1]);

                AppManager.Instance.Player.CurrentJourneyPosition.Stage++;
                letter.GetComponent<LetterMovement>().miniMapScript = miniMaps[numberStage + 1].GetComponent<MiniMap>();
                letter.GetComponent<LetterMovement>().ResetPosLetter();
            }
        }
        public void StageRight()
        {
            int numberStage = AppManager.Instance.Player.CurrentJourneyPosition.Stage;
            if (numberStage > 1)
            {
                stages[numberStage].SetActive(false);
                stages[numberStage -1].SetActive(true);
                ChangeCamera(cameras[numberStage - 1]);

                AppManager.Instance.Player.CurrentJourneyPosition.Stage--;
                letter.GetComponent<LetterMovement>().miniMapScript = miniMaps[numberStage - 1].GetComponent<MiniMap>();
                letter.GetComponent<LetterMovement>().ResetPosLetter();
            }
        }
        public void ChangeCamera(GameObject ZoomCameraGO)
        {

            CameraGameplayController.I.MoveToPosition(ZoomCameraGO.transform.position, ZoomCameraGO.transform.rotation);

        }
    }
}

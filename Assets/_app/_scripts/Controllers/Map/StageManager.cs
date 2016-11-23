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
        public int s,i;
        void Awake()
        {
            s = AppManager.Instance.Player.MaxJourneyPosition.Stage;
            for (i=1;i<= (s-1);i++)
            {
                miniMaps[i].GetComponent<MiniMap>().isAvailableTheWholeMap = true;
                miniMaps[i].GetComponent<MiniMap>().CalculateSettingsStageMap();
            }
            miniMaps[i].GetComponent<MiniMap>().CalculateSettingsStageMap();
            ChangeCamera(cameras[s]);
            stages[s].SetActive(true);
            letter.GetComponent<LetterMovement>().miniMapScript = miniMaps[s].GetComponent<MiniMap>();
            StartCoroutine("ResetPosLetter");
        }
        public void StageLeft()
        {
            int numberStage = AppManager.Instance.Player.CurrentJourneyPosition.Stage;
            if(numberStage < s)
            {
                stages[numberStage].SetActive(false);
                stages[numberStage + 1].SetActive(true);
                ChangeCamera(cameras[numberStage+1]);

                AppManager.Instance.Player.CurrentJourneyPosition.Stage++;
                letter.GetComponent<LetterMovement>().miniMapScript = miniMaps[numberStage+1].GetComponent<MiniMap>();
                StartCoroutine("ResetPosLetter");
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
                letter.GetComponent<LetterMovement>().miniMapScript = miniMaps[numberStage-1].GetComponent<MiniMap>();
                StartCoroutine("ResetPosLetter");
            }
        }
        public void ChangeCamera(GameObject ZoomCameraGO)
        {

            CameraGameplayController.I.MoveToPosition(ZoomCameraGO.transform.position, ZoomCameraGO.transform.rotation);

        }
        IEnumerator ResetPosLetter()
        {
            yield return new WaitForSeconds(1);
            letter.GetComponent<LetterMovement>().ResetPosLetter();
        }
    }
}

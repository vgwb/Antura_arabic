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
            /*AppManager.Instance.Player.MaxJourneyPosition.Stage = 2;
            AppManager.Instance.Player.MaxJourneyPosition.LearningBlock = 3;
            AppManager.Instance.Player.MaxJourneyPosition.PlaySession = 1;*/

            s = AppManager.Instance.Player.MaxJourneyPosition.Stage;
            for (i=1;i<= (s-1);i++)
            {
                miniMaps[i].GetComponent<MiniMap>().isAvailableTheWholeMap = true;
                miniMaps[i].GetComponent<MiniMap>().CalculateSettingsStageMap();
            }
            miniMaps[i].GetComponent<MiniMap>().CalculateSettingsStageMap();

            
            //ChangeCamera(cameras[AppManager.Instance.Player.CurrentJourneyPosition.Stage]);
            stages[AppManager.Instance.Player.CurrentJourneyPosition.Stage].SetActive(true);
            letter.GetComponent<LetterMovement>().miniMapScript = miniMaps[AppManager.Instance.Player.CurrentJourneyPosition.Stage].GetComponent<MiniMap>();

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

                ChangePinDotToBlack();
                AppManager.Instance.Player.CurrentJourneyPosition.Stage++;
                letter.GetComponent<LetterMovement>().miniMapScript = miniMaps[numberStage+1].GetComponent<MiniMap>();
                letter.GetComponent<LetterMovement>().ResetPosLetterAfterChangeStage();
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

                ChangePinDotToBlack();
                AppManager.Instance.Player.CurrentJourneyPosition.Stage--;
                letter.GetComponent<LetterMovement>().miniMapScript = miniMaps[numberStage-1].GetComponent<MiniMap>();
                letter.GetComponent<LetterMovement>().ResetPosLetterAfterChangeStage();
            }
        }
        public void ChangeCamera(GameObject ZoomCameraGO)
        {

            CameraGameplayController.I.MoveToPosition(ZoomCameraGO.transform.position, ZoomCameraGO.transform.rotation);

        }
        IEnumerator ResetPosLetter()
        {
            yield return new WaitForSeconds(0.2f);
            letter.GetComponent<LetterMovement>().ResetPosLetter();
            letter.SetActive(true);
            CameraGameplayController.I.transform.position = cameras[AppManager.Instance.Player.CurrentJourneyPosition.Stage].transform.position;
        }
        void ChangePinDotToBlack()
        {
            if (AppManager.Instance.Player.CurrentJourneyPosition.PlaySession == 100)
                letter.GetComponent<LetterMovement>().ChangeMaterialPinToBlack(letter.GetComponent<LetterMovement>().miniMapScript.posPines[AppManager.Instance.Player.CurrentJourneyPosition.LearningBlock].GetComponent<MapPin>().Dot);
            else
                letter.GetComponent<LetterMovement>().ChangeMaterialDotToBlack(letter.GetComponent<LetterMovement>().miniMapScript.posDots[letter.GetComponent<LetterMovement>().pos]);
        }
    }
}

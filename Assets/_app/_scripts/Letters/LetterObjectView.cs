using UnityEngine;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;

namespace EA4S
{
    /// <summary>
    /// View object for letter puppets.
    /// </summary>
    public class LetterObjectView : MonoBehaviour
    {
        public float MergedElementsDistance = 1;

        // Obsolete
        public LetterObjectView RightLetter = null;
        // Obsolete
        public LetterObjectView LeftLetter = null;

        public LetterObject Model;

        public bool IsMerged;
        public DropSingleArea ActualDropArea;
        //DropState dropState = DropState.off;

        #region View

        public TMP_Text Lable;

        #endregion

        public void Init(ILivingLetterData _data, LetterBehaviour.BehaviourSettings _behaviourSettingsOverride)
        {
            Init(_data);
            GetComponent<LetterBehaviour>().Settings = _behaviourSettingsOverride;
        }

        public void Init(ILivingLetterData _data)
        {
            Model = new LetterObject(_data);
            Lable.text = Model.Data.TextForLivingLetter;
            IsMerged = false;
        }

        #region

        #endregion

        // check if is necessary. If all work correctly after refactoring delete it.
        /*
        void OnTriggerStay(Collider other) {
            ActualDropArea = other.GetComponent<DropSingleArea>();
            if (ActualDropArea) {
                if (Model.State == LetterObjectState.Grab_State) {
                    if (ActualDropArea.Data == Model.Data)
                        dropState = DropState.check_ok;
                    else
                        dropState = DropState.check_ko;
                } else if (Model.State == LetterObjectState.Run_State) {
                    Debug.Log("ReleaseLettera");
                } else {
                    
                }
            }
        }

        void OnTriggerExit(Collider other) {
            //if (ActualDropArea == other.GetComponent<DropSingleArea>()) {
            //    dropState = DropState.off;
            //}

        }

        void OnMouseUp() {
            //if (dropState == DropState.check_ok) {
            //    ActualDropArea.DropContain.NextArea();
            //    // Audio - quick and dirty
            //    AudioSource audio = FastCrowd.FastCrowd.Instance.GetComponent<AudioSource>();
            //    audio.clip = Instantiate<AudioClip>(Resources.Load("Audio/Vox/Letters/Names/VOX_Letters_" + Model.Data.Key) as AudioClip);
            //    audio.Play();

            //    GameObject.Destroy(gameObject);
            //} else {
            //    if (ActualDropArea)
            //        ActualDropArea.GetComponent<Renderer>().materials[0].color = Color.white;
            //}
        }

        public enum DropState {
            off,
            // Not checkable
            check_ok,
            // right matching preview
            check_ko,
            // wrong matching preview
        }
        */
    }
}

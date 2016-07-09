using UnityEngine;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;
using UniRx;
using Panda;

namespace EA4S
{
    public class LetterObjectView : MonoBehaviour
    {
        public float MergedElementsDistance = 1;

        public LetterObjectView RightLetter = null;
        public LetterObjectView LeftLetter = null;

        public LetterObject Model;
        
        public bool IsMerged;
        public DropSingleArea ActualDropArea;
        DropState dropState = DropState.off;
        NavMeshAgent agent;



        #region View

        public TMP_Text Lable;

        #endregion

        public void Init(LetterData _data) {
            // Navigation
            /*
            agent = GetComponent<NavMeshAgent>();
            if (!agent)
                gameObject.AddComponent<NavMeshAgent>();
            agent.enabled = false;
            */
            //GetComponent<Collider>().isTrigger = false;
            Model = new LetterObject(_data);
            Lable.text = ArabicAlphabetHelper.GetLetterFromUnicode(Model.Data.Isolated_Unicode);
            IsMerged = false;


            /// <summary>
            /// Monitoring Model property XXX value changes.
            /// </summary>
            this.transform.ObserveEveryValueChanged(x => dropState).Subscribe(_ =>
                {

                    switch (dropState) {
                        case DropState.off:
                            if (ActualDropArea) { 
                                ActualDropArea.GetComponent<Renderer>().materials[0].color = Color.white;
                                ActualDropArea = null;
                            }
                            break;
                        case DropState.check_ok:
                            if (ActualDropArea)
                                ActualDropArea.GetComponent<Renderer>().materials[0].color = Color.green;
                            break;
                        case DropState.check_ko:
                            if (ActualDropArea)
                                ActualDropArea.GetComponent<Renderer>().materials[0].color = Color.red;
                            break;
                        default:
                            break;
                    }

                });

        }

        #region

        //void OnMouseDrag() {
        //    if (IsMerged)
        //        return;
        //    Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        //    Vector3 objPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -Camera.main.transform.position.z));

        //    transform.localPosition = new Vector3(objPosition.x, objPosition.y, transform.localPosition.z);
        //    GetComponent<Collider>().isTrigger = true;
        //}

        //void OnMouseUp() {
        //    GetComponent<Collider>().isTrigger = false;
        //    if (RightLetter != null || LeftLetter != null) {
        //        IsMerged = true;
        //        SetLetterForPosition();
        //        propagateSetLetterForPosition();
        //    }

        //    if (RightLetter != null) {
        //        transform.position = RightLetter.transform.position + new Vector3(-MergedElementsDistance, 0, 0);
        //    } else if (LeftLetter != null) {
        //        transform.position = LeftLetter.transform.position + new Vector3(MergedElementsDistance, 0, 0);
        //    }
        //}

        public void propagateSetLetterForPosition() {
            LetterObjectView nextLeft = LeftLetter;
            while (nextLeft != null) {
                nextLeft.SetLetterForPosition();
                nextLeft = nextLeft.LeftLetter;
            }

            LetterObjectView nextRight = RightLetter;
            while (nextRight != null) {
                nextRight.SetLetterForPosition();
                nextRight = nextRight.LeftLetter;
            }
        }

        public void SetLetterForPosition() {
            IsMerged = true;
            if (Model.Data == null)
                // No data in db.
                return;
            if (RightLetter == null && LeftLetter == null) {
                // Isolate
                Lable.text = ArabicAlphabetHelper.GetLetterFromUnicode(Model.Data.Isolated_Unicode);
                return;
            } else if (RightLetter != null && LeftLetter == null) {
                // Initial
                Lable.text = ArabicAlphabetHelper.GetLetterFromUnicode(Model.Data.Initial_Unicode);
            } else if (RightLetter == null && LeftLetter != null) {
                // Final
                Lable.text = ArabicAlphabetHelper.GetLetterFromUnicode(Model.Data.Final_Unicode);
            } else if (RightLetter != null && LeftLetter != null) {
                // Median
                Lable.text = ArabicAlphabetHelper.GetLetterFromUnicode(Model.Data.Medial_Unicode);
            }
        }

        //#region Triggers
        //void OnTriggerEnter(Collider other) {
        //    LetterObjectView otherL = other.GetComponent<LetterObjectView>();
        //    if (otherL == null)
        //        return;

        //    if (transform.position.x < otherL.transform.position.x) {
        //        RightLetter = otherL;
        //        otherL.LeftLetter = this;
        //    } else {
        //        LeftLetter = otherL;
        //        otherL.RightLetter = this;
        //    }
        //}

        //void OnTriggerExit(Collider other) {
        //    LetterObjectView otherL = other.GetComponent<LetterObjectView>();
        //    if (otherL == null)
        //        return;

        //    if (otherL == RightLetter) {
        //        RightLetter = null;
        //        otherL.LeftLetter = null;
        //    } else if (otherL == LeftLetter) {
        //        LeftLetter = null;
        //        otherL.RightLetter = null;
        //    }
        //}
        //#endregion

        #endregion

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
            if (ActualDropArea == other.GetComponent<DropSingleArea>()) {
                dropState = DropState.off;
            }
            
        }

        void OnMouseUp() {
            if (dropState == DropState.check_ok) {
                ActualDropArea.DropContain.NextArea();
                // Audio - quick and dirty
                AudioSource audio = MiniGame.FastCrowd.FastCrowd.Instance.GetComponent<AudioSource>();
                audio.clip = Instantiate<AudioClip>(Resources.Load("Audio/Vox/Letters/Names/VOX_Letters_" + Model.Data.Key) as AudioClip);
                audio.Play();

                GameObject.Destroy(gameObject);
            } else {
                if (ActualDropArea)
                    ActualDropArea.GetComponent<Renderer>().materials[0].color = Color.white;
            }
        }

        public enum DropState {
            off,
            // Not checkable
            check_ok,
            // right matching preview
            check_ko,
            // wrong matching preview
        }
    }
}

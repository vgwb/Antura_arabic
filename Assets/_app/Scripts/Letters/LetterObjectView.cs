using UnityEngine;
using System.Collections;
using TMPro;
using System.Text.RegularExpressions;

namespace CGL.Antura {
    public class LetterObjectView : MonoBehaviour {
        public float MergedElementsDistance = 1;

        public LetterObjectView RightLetter = null;
        public LetterObjectView LeftLetter = null;

        public LetterData Data;
        public bool IsMerged;

        #region View
        public TMP_Text Lable;
        #endregion

        public void Init(LetterData _data) {
            GetComponent<Collider>().isTrigger = false;
            Data = _data;
            Lable.text = ArabicAlphabetHelper.GetLetterFromUnicode(_data.Isolated_Unicode);
            IsMerged = false;
        }

        void OnMouseDrag() {
            if (IsMerged)
                return;
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            Vector3 objPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -Camera.main.transform.position.z));

            transform.localPosition = new Vector3(objPosition.x, objPosition.y, transform.localPosition.z);
            GetComponent<Collider>().isTrigger = true;
        }

        void OnMouseUp() {
            GetComponent<Collider>().isTrigger = false;
            if (RightLetter != null || LeftLetter != null) {
                IsMerged = true;
                SetLetterForPosition();
                propagateSetLetterForPosition();
            }

            if (RightLetter != null) {
                transform.position = RightLetter.transform.position + new Vector3(-MergedElementsDistance, 0, 0);
            } else if (LeftLetter != null) {
                transform.position = LeftLetter.transform.position + new Vector3(MergedElementsDistance, 0, 0);
            }
        }

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
            if (Data == null)
                // No data in db.
                return;
            if (RightLetter == null && LeftLetter == null) {
                // Isolate
                Lable.text = ArabicAlphabetHelper.GetLetterFromUnicode(Data.Isolated_Unicode);
                return;
            }else
            if (RightLetter != null && LeftLetter == null) {
                // Initial
                Lable.text = ArabicAlphabetHelper.GetLetterFromUnicode(Data.Initial_Unicode);
            }else
            if (RightLetter == null && LeftLetter != null) {
                // Final
                Lable.text = ArabicAlphabetHelper.GetLetterFromUnicode(Data.Final_Unicode);
            } else
            if (RightLetter != null && LeftLetter != null) {
                // Median
                Lable.text = ArabicAlphabetHelper.GetLetterFromUnicode(Data.Medial_Unicode);
            }
        }

        #region Triggers
        void OnTriggerEnter(Collider other) {
            LetterObjectView otherL = other.GetComponent<LetterObjectView>();
            if (otherL == null)
                return;

            if (transform.position.x < otherL.transform.position.x) {
                RightLetter = otherL;
                otherL.LeftLetter = this;
            } else { 
                LeftLetter = otherL;
                otherL.RightLetter = this;
            }
        }

        void OnTriggerExit(Collider other) {
            LetterObjectView otherL = other.GetComponent<LetterObjectView>();
            if (otherL == null)
                return;

            if (otherL == RightLetter) { 
                RightLetter = null;
                otherL.LeftLetter = null;
            } else if (otherL == LeftLetter) { 
                LeftLetter = null;
                otherL.RightLetter = null;
            }
        }
        #endregion
    }
}

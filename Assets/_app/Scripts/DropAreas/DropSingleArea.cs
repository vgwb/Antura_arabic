using UnityEngine;
using System.Collections;
using Google2u;
using TMPro;

namespace CGL.Antura {
    public class DropSingleArea : MonoBehaviour {

        public TMP_Text LetterLable;
        public LetterData Data;
        public DropContainer DropContain;

        Vector3 enabledPos, disabledPos;

        public void Init(LetterData _letterData, DropContainer _dropContainer) {
            DropContain = _dropContainer;
            DropContain.Aree.Add(this);
            enabledPos = transform.position;
            disabledPos = enabledPos + new Vector3(0, -0.8f, 0);
            Data = _letterData;
            LetterLable.text = ArabicAlphabetHelper.GetLetterFromUnicode(Data.Isolated_Unicode);
        }

        private State areaState;

        public State AreaState {
            get { return areaState; }
            set {
                if (areaState != value) {
                    areaState = value;
                    areaStateChanged();
                } else {
                    areaState = value;
                }

            }
        }

        void areaStateChanged() {
            switch (AreaState) {
                case State.enabled:
                    GetComponent<Collider>().enabled = true;
                    transform.position = enabledPos;
                    break;
                case State.disabled:
                    GetComponent<Collider>().enabled = false;
                    transform.position = disabledPos;
                    break;
                default:
                    break;
            }
        }

        public enum State {
            enabled,
            disabled,
        }



    }
}

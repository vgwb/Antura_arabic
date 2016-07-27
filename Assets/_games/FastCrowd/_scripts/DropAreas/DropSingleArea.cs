using UnityEngine;
using System.Collections;
using Google2u;
using TMPro;

namespace EA4S {
    public class DropSingleArea : MonoBehaviour {

        public TMP_Text LetterLable;
        public SpriteRenderer DrawSprite;
        public ILivingLetterData Data;
        public DropContainer DropContain;

        Vector3 enabledPos, disabledPos;

        #region Api
        public void Init(ILivingLetterData _data, DropContainer _dropContainer) {
            DropContain = _dropContainer;
            DropContain.Aree.Add(this);
            Data = _data;
            if (_data.DataType == LivingLetterDataType.Letter) {
                LetterLable.text = Data.TextForLivingLetter;
                DrawSprite.gameObject.SetActive(false);
            } else {
                LetterLable.text = string.Empty;
                DrawSprite.gameObject.SetActive(true); 
                DrawSprite.sprite = Data.DrawForLivingLetter;
            }

            AreaState = State.disabled;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetDisbled() {
            AreaState = State.disabled;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetEnabled() {
            AreaState = State.enabled;
        }

        /// <summary>
        /// Set Matching state.
        /// </summary>
        public void SetMatching() {
            AreaState = State.matching;
        }

        /// <summary>
        /// Set Matching state.
        /// </summary>
        public void SetMatchingWrong() {
            AreaState = State.matching_wrong;
        }

        /// <summary>
        /// Automatically return to state pre matching.
        /// </summary>
        public void DeactivateMatching() {

            if (GetComponent<Collider>().enabled)
                AreaState = State.enabled;
            else
                AreaState = State.disabled;
        }
        #endregion

        /// <summary>
        /// Stete of drop Area.
        /// </summary>
        public State AreaState {
            get { return areaState; }
            protected set {
                if (areaState != value) {
                    areaState = value;
                    areaStateChanged();
                } else {
                    areaState = value;
                }

            }
        }
        private State areaState;


        /// <summary>
        /// Effects to state change.
        /// </summary>
        void areaStateChanged() {
            switch (AreaState) {
                case State.enabled:
                    GetComponent<Collider>().enabled = true;
                    GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", Color.yellow);
                    break;
                case State.disabled:
                    GetComponent<Collider>().enabled = false;
                    GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", Color.gray);
                    break;
                case State.matching:
                    // Matching preview right
                    GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", Color.green);
                    break;
                case State.matching_wrong:
                    // Matching preview wrong
                    GetComponent<MeshRenderer>().materials[0].SetColor("_EmissionColor", new Color(248,0,0));
                    break;
                default:
                    break;
            }
        }


        public enum State {
            isnull,
            disabled,
            enabled,
            matching,
            matching_wrong,
        }
    }
}

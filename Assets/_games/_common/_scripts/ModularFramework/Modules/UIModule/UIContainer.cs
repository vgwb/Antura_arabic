
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace EA4S.Core
{


    public class UIContainer : MonoBehaviour {

        #region Properties
        public string Key;
        public List<UIContainer> Breadcrumbs = new List<UIContainer>();
        public Button CloseContainerButton;
        #endregion

        #region Activation State
        public enum States {
            hide,
            hiding,
            show,
            showing,
        }

        private States state;
        /// <summary>
        /// State
        /// </summary>
        public States State {
            get { return state; }
            set {
                if (state != value)
                    OnStateChanged(value, state);
                state = value; }
        }
        #endregion

        // If needed...
        public UIContainerTypes ContainerType;
        public enum UIContainerTypes{
            FullScreen,
            Popup,
        }

        RectTransform rectTransform;

        void Awake() {
            rectTransform = GetComponent<RectTransform>();
            State = States.hiding;
        }

        public virtual void OnEnable() {
            // Remove UniRx refactoring request: any reactive interaction within this class must be called manually.
        }


        #region StateMachine
        /// <summary>
        /// Called at any state value change.
        /// </summary>
        /// <param name="_newState"></param>
        /// <param name="_oldStates"></param>
        protected virtual void OnStateChanged(States _newState, States _oldStates) {
            switch (_newState) {
                case States.hide:
                    break;
                case States.hiding:
                    rectTransform.anchoredPosition = new Vector2(0, Screen.height * 2);
                    State = States.hide;
                    break;
                case States.show:
                    break;
                case States.showing:
                    rectTransform.anchoredPosition = new Vector2(0, 0);
                    State = States.show;
                    break;
            }
        }
        #endregion

        #region API
        public void Show() {
            State = States.showing; 
        }

        public void Hide() {
            State = States.hiding;
        }
        #endregion
    }
}
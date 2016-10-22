using UnityEngine;
using System.Collections;

namespace EA4S {

    public class EmoticonsController : MonoBehaviour {

        Animator anim;

        // Use this for initialization
        void Start() {
            anim.GetComponentInChildren<Animator>();
        }

        public void Init(Emoticons _emoticons) {
            switch (_emoticons) {
                case Emoticons.vfx_emo_angry:
                    break;
                case Emoticons.vfx_emo_exclamative:
                    break;
                case Emoticons.vfx_emo_happy:
                    break;
                case Emoticons.vfx_emo_interrogative:
                    break;
                case Emoticons.vfx_emo_negative:
                    break;
                case Emoticons.vfx_emo_positive:
                    break;
                default:
                    Debug.LogWarningFormat("Emoticons {0} not found!", _emoticons.ToString());
                    break;
            }
        }


        public void Open(bool _isOpen) {
            anim.SetBool("IsOpen", _isOpen);
        }
    }

    public enum Emoticons {
        vfx_emo_angry,
        vfx_emo_exclamative,
        vfx_emo_happy,
        vfx_emo_interrogative,
        vfx_emo_negative,
        vfx_emo_positive,
    }

}
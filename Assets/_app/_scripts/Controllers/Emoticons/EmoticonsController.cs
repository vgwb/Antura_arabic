using UnityEngine;
using System.Collections;

namespace EA4S {

    public class EmoticonsController : MonoBehaviour {

        const string EMOTICON_PREFS_PATH = "Prefabs/Emoticons/";

        public Animator anim;
        public Transform EmoticonParentBone;

        // Use this for initialization
        void Start() {
            CleanEmoticonIcons();
        }

        void CleanEmoticonIcons() {
            foreach (Transform child in EmoticonParentBone) {
                GameObject.Destroy(child.gameObject);
            }
        }

        void scaleToOneAllChildren(Transform _parent) {

            foreach (Transform t in _parent.GetComponentsInChildren<Transform>()) {
                t.localScale = Vector3.one;
            }
        }

        public void SetEmoticon(Emoticons _emoticons, bool _open = false) {
            GameObject Et;
            CleanEmoticonIcons();
            switch (_emoticons) {
                case Emoticons.vfx_emo_angry:
                case Emoticons.vfx_emo_exclamative:
                case Emoticons.vfx_emo_happy:
                case Emoticons.vfx_emo_interrogative:
                case Emoticons.vfx_emo_negative:
                case Emoticons.vfx_emo_positive:
                    Et = Instantiate(Resources.Load(EMOTICON_PREFS_PATH + _emoticons.ToString()),EmoticonParentBone, false) as GameObject;
                    //scaleToOneAllChildren(Et.transform);
                    break;
                default:
                    Debug.LogWarningFormat("Emoticons {0} not found!", _emoticons.ToString());
                    break;
            }

            if (_open) {
                Open(true);
            }
        }

        void Update() {
            if (Input.anyKeyDown) {
                if (anim.GetBool("IsOpen")) {
                    Open(false);
                    return;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                Open(!anim.GetBool("IsOpen"));
            }
            if (Input.GetKeyDown(KeyCode.Z)) {
                SetEmoticon(Emoticons.vfx_emo_angry, true);
            }
            if (Input.GetKeyDown(KeyCode.X)) {
                SetEmoticon(Emoticons.vfx_emo_happy, true);
            }
            if (Input.GetKeyDown(KeyCode.C)) {
                SetEmoticon(Emoticons.vfx_emo_exclamative, true);
            }
            if (Input.GetKeyDown(KeyCode.V)) {
                SetEmoticon(Emoticons.vfx_emo_interrogative, true);
            }
            if (Input.GetKeyDown(KeyCode.B)) {
                SetEmoticon(Emoticons.vfx_emo_positive, true);
            }
            if (Input.GetKeyDown(KeyCode.N)) {
                SetEmoticon(Emoticons.vfx_emo_negative, true);
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
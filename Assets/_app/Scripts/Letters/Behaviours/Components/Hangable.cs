using UnityEngine;
using System.Collections;

namespace EA4S
{
    [RequireComponent(typeof(LetterObjectView))]
    /// <summary>
    /// Manage player drag behaviour.
    /// </summary>
    public class Hangable : MonoBehaviour
    {

        bool OnDrag = false;
        Transform tmpTarget;
        LetterObjectView letterView;

        void Start() {
            letterView = GetComponent<LetterObjectView>();
        }

        void OnMouseDown() {
            OnDrag = true;
            gameObject.GetComponent<Animator>().Play("hold");
            letterView.Model.State = LetterObjectState.Grab_State;
            // Audio - quick and dirty
            AudioSource audio = MiniGame.FastCrowd.FastCrowd.Instance.GetComponent<AudioSource>();
            audio.clip = Instantiate<AudioClip>(Resources.Load("Audio/Vox/Letters/Names/VOX_Letters_" + GetComponent<LetterObjectView>().Model.Data.Key) as AudioClip);
            audio.Play();
        }

        void OnMouseUp() {
            OnDrag = false;
            gameObject.GetComponent<Animator>().Play("run");
            letterView.Model.State = LetterObjectState.Run_State;
        }

        void Update() {
            if (OnDrag) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100)) {
                    transform.position = hit.point;
                }
            }
        }
        
    }
}

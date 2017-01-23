using UnityEngine;
using EA4S.Audio;

namespace EA4S.Minigames.MissingLetter
{
    public class FeedbackClickDisable : MonoBehaviour
    {

        void OnMouseDown()
        {
            if(!m_bIsPlaying)
            {
                m_bIsPlaying = true;
                AudioManager.I.PlaySound(Sfx.Splat);
                StartCoroutine(Utils.LaunchDelay(0.5f, setPlaying, false));
            }
        }

        void setPlaying(bool _isPlaying)
        {
            m_bIsPlaying = _isPlaying;
        }

        private bool m_bIsPlaying = false;
    }
}

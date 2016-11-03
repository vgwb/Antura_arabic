using UnityEngine;

namespace EA4S.Egg
{
    public class EggEmoticonsController
    {
        EmoticonsController emoticonsController;

        float emoticonsCloseTime = 2f;
        float emoticonsCloseTimer = 0f;
        bool emoticonsClosed;
        Emoticons? currentEmoticon;

        public EggEmoticonsController(Transform parent, GameObject emoticonsPrefab)
        {
            emoticonsController = Object.Instantiate(emoticonsPrefab).GetComponent<EmoticonsController>();

            emoticonsController.transform.SetParent(parent);
            emoticonsController.transform.localPosition = new Vector3(0f, 3f);
            emoticonsController.SetEmoticon(Emoticons.vfx_emo_exclamative, true);

            parent.localScale = new Vector3(3f, 3f, 3f);

            CloseEmoticons();
        }

        public void Update(float delta)
        {
            if (!emoticonsClosed)
            {
                emoticonsCloseTimer -= delta;

                if (emoticonsCloseTimer <= 0f)
                {
                    CloseEmoticons();
                }
            }
        }

        public void EmoticonHappy()
        {
            OpenEmoticons(Emoticons.vfx_emo_happy);
        }

        public void EmoticonPositive()
        {
            OpenEmoticons(Emoticons.vfx_emo_positive);
        }

        public void EmoticonNegative()
        {
            OpenEmoticons(Emoticons.vfx_emo_negative);
        }

        void OpenEmoticons(Emoticons icon)
        {
            if (!currentEmoticon.HasValue || (currentEmoticon.HasValue && currentEmoticon.Value != icon))
            {
                currentEmoticon = icon;
                emoticonsController.Open(false);
                emoticonsController.SetEmoticon(icon, true);
            }

            emoticonsCloseTimer = emoticonsCloseTime;
            emoticonsClosed = false;
        }

        void CloseEmoticons()
        {
            emoticonsController.Open(false);
            emoticonsCloseTimer = 0f;
            emoticonsClosed = true;
            currentEmoticon = null;
        }
    }
}
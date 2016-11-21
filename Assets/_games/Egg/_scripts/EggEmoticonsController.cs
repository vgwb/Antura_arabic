using UnityEngine;

namespace EA4S.Egg
{
    public class EggEmoticonsController
    {
        EmoticonsController emoticonsController;

        bool autoClose;

        float emoticonsCloseTime = 2f;
        float emoticonsCloseTimer = 0f;
        bool emoticonsClosed;
        Emoticons? currentEmoticon;
        
        PaletteColors internalColor = PaletteColors.white;
        PaletteTone internalTone = PaletteTone.light;
        PaletteColors externalColor = PaletteColors.white;
        PaletteTone externalTone = PaletteTone.light;
        PaletteColors cineticLinesColor = PaletteColors.white;
        PaletteTone cineticLinesTone = PaletteTone.light;
        
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
            if (!emoticonsClosed && autoClose)
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
            internalColor = PaletteColors.azure;
            internalTone = PaletteTone.dark;
            externalColor = PaletteColors.green;
            externalTone = PaletteTone.mid;
            cineticLinesColor = PaletteColors.orange;
            cineticLinesTone = PaletteTone.dark;

            OpenEmoticons(Emoticons.vfx_emo_happy);

            autoClose = true;
        }

        public void EmoticonPositive()
        {
            internalColor = PaletteColors.green;
            internalTone = PaletteTone.mid;
            externalColor = PaletteColors.black;
            externalTone = PaletteTone.pure;
            cineticLinesColor = PaletteColors.yellow;
            cineticLinesTone = PaletteTone.dark;

            OpenEmoticons(Emoticons.vfx_emo_positive);

            autoClose = true;
        }

        public void EmoticonNegative()
        {
            internalColor = PaletteColors.red;
            internalTone = PaletteTone.dark;
            externalColor = PaletteColors.red;
            externalTone = PaletteTone.mid;
            cineticLinesColor = PaletteColors.azure;
            cineticLinesTone = PaletteTone.dark;

            OpenEmoticons(Emoticons.vfx_emo_negative);

            autoClose = true;
        }

        public void EmoticonInterrogative()
        {
            internalColor = PaletteColors.green;
            internalTone = PaletteTone.dark;
            externalColor = PaletteColors.red;
            externalTone = PaletteTone.light;
            cineticLinesColor = PaletteColors.pink;
            cineticLinesTone = PaletteTone.mid;

            OpenEmoticons(Emoticons.vfx_emo_interrogative);

            autoClose = false;
        }

        void OpenEmoticons(Emoticons icon)
        {
            if (!currentEmoticon.HasValue || (currentEmoticon.HasValue && currentEmoticon.Value != icon))
            {
                currentEmoticon = icon;
                emoticonsController.Open(false);
                UpdateEmoticonsColor();
                emoticonsController.SetEmoticon(icon, true);
            }

            emoticonsCloseTimer = emoticonsCloseTime;
            emoticonsClosed = false;
        }

        public void CloseEmoticons()
        {
            emoticonsController.Open(false);
            emoticonsCloseTimer = 0f;
            emoticonsClosed = true;
            currentEmoticon = null;
        }

        void UpdateEmoticonsColor()
        {
            changeMaterials(MaterialManager.LoadMaterial(internalColor, internalTone), emoticonsController.Internal);
            changeMaterials(MaterialManager.LoadMaterial(externalColor, externalTone), emoticonsController.External);
            changeMaterials(MaterialManager.LoadMaterial(cineticLinesColor, cineticLinesTone), emoticonsController.Cinetic);
        }

        void changeMaterials(Material _material, SkinnedMeshRenderer[] _meshRenderer)
        {
            foreach (var item in _meshRenderer)
            {
                SkinnedMeshRenderer m = item.gameObject.GetComponent<SkinnedMeshRenderer>();
                m.materials = new Material[] { _material };
            }
        }
    }
}
using UnityEngine;
using UnityEngine.Assertions;

namespace EA4S.MissingLetter {
    public class MissingLetterEmoticonsController {

        #region API
        public MissingLetterEmoticonsController(GameObject _EmoticonsController) {
            Assert.IsNotNull<EmoticonsController>(_EmoticonsController.GetComponent<EmoticonsController>(), "Please attach the EmoticonsController script to the prefab : " + _EmoticonsController.name);
            emoticonsController = _EmoticonsController.GetComponent<EmoticonsController>();
        }

        public void init(Transform parent) {
            emoticonsController.transform.SetParent(parent);
            emoticonsController.transform.localPosition = new Vector3(0, 6.5f);
            emoticonsController.transform.localScale = new Vector3(3f, 3f, 3f);
            emoticonsController.transform.forward = Vector3.back;

            CloseEmoticons();
        }

        public void EmoticonPositive() {
            internalColor = PaletteColors.green;
            internalTone = PaletteTone.mid;
            externalColor = PaletteColors.black;
            externalTone = PaletteTone.pure;
            cineticLinesColor = PaletteColors.yellow;
            cineticLinesTone = PaletteTone.dark;

            OpenEmoticons(Emoticons.vfx_emo_positive);

            autoClose = true;
        }

        public void EmoticonNegative() {
            internalColor = PaletteColors.red;
            internalTone = PaletteTone.dark;
            externalColor = PaletteColors.red;
            externalTone = PaletteTone.mid;
            cineticLinesColor = PaletteColors.azure;
            cineticLinesTone = PaletteTone.dark;

            OpenEmoticons(Emoticons.vfx_emo_negative);

            autoClose = true;
        }


        public void CloseEmoticons() {
            emoticonsController.Open(false);

            emoticonsClosed = true;
            currentEmoticon = null;
        }
        #endregion

        #region PRIVATE_FUNCTION
        void OpenEmoticons(Emoticons icon) {
            if (!currentEmoticon.HasValue || (currentEmoticon.HasValue && currentEmoticon.Value != icon)) {
                currentEmoticon = icon;
                emoticonsController.Open(false);
                UpdateEmoticonsColor();
                emoticonsController.SetEmoticon(icon, true);
            }

            emoticonsClosed = false;
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
        #endregion

        #region VARS
        EmoticonsController emoticonsController;

        bool autoClose;
        bool emoticonsClosed;
        Emoticons? currentEmoticon;

        PaletteColors internalColor = PaletteColors.white;
        PaletteTone internalTone = PaletteTone.light;
        PaletteColors externalColor = PaletteColors.white;
        PaletteTone externalTone = PaletteTone.light;
        PaletteColors cineticLinesColor = PaletteColors.white;
        PaletteTone cineticLinesTone = PaletteTone.light;
        #endregion
    }
}
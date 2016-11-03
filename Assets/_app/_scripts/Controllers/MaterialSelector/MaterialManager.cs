using UnityEngine;

namespace EA4S {

    public class MaterialManager {


        public PaletteColors Color;
        public PaletteTone Tone;

        public const string MATERIALS_REOURCES_PATH = "Materials/Palettes/";
        //public const string MATERIALS_FOLDER_DIFFUSE_DESATURATED = "diffuse_desaturated/";
        //public const string MATERIALS_FOLDER_DIFFUSE_SATURATED = "diffuse_saturated/";
        //public const string MATERIALS_FOLDER_SPECULAR_SATURATED = "specular_saturated/";
        //public const string MATERIALS_FOLDER_TRANSPARENT_SATURATED = "transparent_saturated/";

        public static Material LoadMaterial(PaletteColors _color, PaletteTone _tone, PaletteType _type = PaletteType.diffuse_saturated) {
            return LoadMaterial(string.Format("{1}_{2}", _color.ToString(), _tone.ToString()), _type);
        }

        public static Material LoadMaterial(string _materialID, PaletteType _type = PaletteType.diffuse_saturated) {
            Material m = Resources.Load<Material>(string.Format("{0}{1}", string.Format("{0}{1}/", MATERIALS_REOURCES_PATH, _type.ToString()), _materialID));
            if (m == null) {
                m = Resources.Load<Material>(string.Format("{0}{1}_{2}", MATERIALS_REOURCES_PATH, "white", "pure"));
                //Debug.LogFormat("Material not found {0}_{1} in path {2}", _color, _tone, MATERIALS_REOURCES_PATH);
            }
            return m;
        }

    }

}

using System;
using Antura.Rewards;
using UnityEngine;

namespace Antura.Rewards
{
    public class MaterialManager
    {
        public const string MATERIALS_REOURCES_PATH = "Materials/Palettes/";
        public const string TEXTURES_MATERIALS = "AnturaStuff/Textures_and_Materials/";

        public static Material LoadMaterial(PaletteColors _color, PaletteTone _tone, PaletteType _type = PaletteType.diffuse_saturated)
        {
            return LoadMaterial(string.Format("{0}_{1}", _color.ToString(), _tone.ToString()), _type);
        }

        public static Material LoadMaterial(string _materialID, PaletteType _type = PaletteType.diffuse_saturated)
        {
            Material m = Resources.Load<Material>(string.Format("{0}{1}",
                string.Format("{0}{1}/", MATERIALS_REOURCES_PATH, _type.ToString()), _materialID));
            if (m == null) {
                m = Resources.Load<Material>(string.Format("{0}{1}_{2}", MATERIALS_REOURCES_PATH, "white", "pure"));
                //Debug.LogFormat("Material not found {0}_{1} in path {2}", _color, _tone, MATERIALS_REOURCES_PATH);
            }
            return m;
        }


        public static Material LoadTextureMaterial(string _materialID, string _variationId)
        {
            string materialName = string.Format("{0}{1}_{2}", TEXTURES_MATERIALS, _materialID, _variationId);
            Material m = Resources.Load<Material>(materialName);
            return m;
        }
    }
}

namespace Antura.Core
{
    /// <summary>
    /// Contains materials pair needed to set right color of Starndard Rewards.
    /// </summary>
    public struct MaterialPair
    {
        public Material Material1;
        public Material Material2;

        public MaterialPair(string _material1Name, string _material1PaletteType, string _material2Name, string _material2PaletteType)
        {
            Material1 = MaterialManager.LoadMaterial(_material1Name, (PaletteType) Enum.Parse(typeof(PaletteType), _material1PaletteType));
            Material2 = MaterialManager.LoadMaterial(_material2Name, (PaletteType) Enum.Parse(typeof(PaletteType), _material2PaletteType));
        }
    }
}
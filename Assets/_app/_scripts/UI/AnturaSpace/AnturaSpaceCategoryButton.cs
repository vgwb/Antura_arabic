// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/25

using UnityEngine;

namespace EA4S
{
    public class AnturaSpaceCategoryButton : UIButton
    {
        public enum AnturaSpaceCategory
        {
            Unset,
            HEAD,
            Ears, // Output as EAR_R and EAR_L
            NOSE,
            JAW,
            NECK,
            BACK,
            TAIL,
            Texture,
            Decal
        }

        public AnturaSpaceCategory Category;
    }
}
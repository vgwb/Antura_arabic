// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/11/25

using UnityEngine;

namespace EA4S
{
    public class AnturaSpaceCategoryButton : UIButton
    {
        public enum AnturaSpaceCategory
        {
            Head,
            Ears,
            Nose,
            Mouth,
            Neck,
            Back,
            Tail,
            Texture,
            Decal
        }

        public AnturaSpaceCategory Category;
    }
}
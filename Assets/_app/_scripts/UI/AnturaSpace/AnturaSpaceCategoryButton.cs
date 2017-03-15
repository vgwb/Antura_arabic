using EA4S.Rewards;
using UnityEngine;

namespace EA4S.UI
{
    /// <summary>
    /// Button for a category in the Antura Space scene.
    /// </summary>
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

        GameObject icoNew;

        public void SetAsNew(bool _isNew)
        {
            if (icoNew == null) icoNew = this.GetComponentInChildren<AnturaSpaceNewIcon>().gameObject;
            icoNew.SetActive(_isNew);
        }
    }
}
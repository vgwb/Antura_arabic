namespace EA4S
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
    }
}
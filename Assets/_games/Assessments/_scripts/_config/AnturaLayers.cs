using UnityEngine;

namespace EA4S
{
    /// <summary>
    /// Fast conversion to game layers
    /// </summary>
    public static class AnturaLayers
    {
        static AnturaLayers()
        {
            ModelsOverUI = LayerMask.NameToLayer( ModelsOverUIString);
        }

        public static readonly string ModelsOverUIString = "ModelsOverUI";
        public static int _modelsOverUI;

        public static int ModelsOverUI { get { return _modelsOverUI; } private set { _modelsOverUI = value; } }
       
    }
}

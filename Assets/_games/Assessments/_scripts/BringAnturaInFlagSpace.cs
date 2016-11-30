using UnityEngine;
using System.Collections;

namespace EA4S.Assessment
{
    /// <summary>
    /// Set culling flag for antura to ModelsOverUI to avoid graphical glitches
    /// (the model may instantiate new parts depending on which items are unlocked
    /// by player)
    /// </summary>
    public class BringAnturaInFlagSpace : MonoBehaviour
    {   
        IEnumerator Start()
        {
            //Give times to instantiate Antura's Parts
            gameObject.SetLayerRecursive( AnturaLayers.ModelsOverUI);
            yield return null;
            gameObject.SetLayerRecursive( AnturaLayers.ModelsOverUI);
            yield return null;
            gameObject.SetLayerRecursive( AnturaLayers.ModelsOverUI);
        }
    }
}

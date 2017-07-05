using UnityEngine;

namespace Antura
{
    public static class GameObjectExtension
    {
        public static void SetLayerRecursive( this GameObject me, int layer)
        {
            me.layer = layer;

            foreach (Transform t in me.transform)
                t.gameObject.SetLayerRecursive( layer);
        }
    }
}

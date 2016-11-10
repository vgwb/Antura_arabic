using UnityEngine;
using System.Collections;

namespace EA4S
{ 
    [ExecuteInEditMode]
    public class AutoWorldPrefab : MonoBehaviour
    {
        public WorldPrefabSet prefabSet;
        GameObject instance;

#if UNITY_EDITOR
        WorldID lastTestWorld = WorldID.Default;
        public WorldID testWorld;
#endif

        void UpdatePrefab(GameObject prefab)
        {
            if (instance != null)
                Destroy(instance);

            instance = Instantiate(prefab);
            instance.hideFlags = HideFlags.DontSave;
            instance.transform.SetParent(transform);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
        }

        public void Start()
        {
            var prefab = WorldManager.I.GetPrefab(prefabSet);

            UpdatePrefab(prefab);

        }


#if UNITY_EDITOR
        void Update()
        {
            if (testWorld != lastTestWorld)
            {
                lastTestWorld = testWorld;

                var prefab = WorldManager.I.GetPrefab(prefabSet, testWorld);

                UpdatePrefab(prefab);
            }
        }
#endif
    }
}

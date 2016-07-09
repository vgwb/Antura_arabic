using UnityEngine;
using System.Collections;
using Fabric;

namespace Fabric
{
#if UNITY_EDITOR
    using UnityEditor;

    class FabricSpringBoardListener : UnityEditor.AssetModificationProcessor
    {
#if UNITY_4_6
       public static string[] OnWillSaveAssets(string[] assets)
        {
            if(FabricSpringBoard._isPresent)
            {
                FabricSpringBoard.DestroyFabricManagerInEditor();
            }
            return assets;
        }
#else
        public static void OnWillSaveAssets(string[] assets)
        {
            if(FabricSpringBoard._isPresent)
            {
                FabricSpringBoard.DestroyFabricManagerInEditor();
            }
        }
#endif
    }

#endif

    [ExecuteInEditMode]
    public class FabricSpringBoard : MonoBehaviour
    {
        public string _fabricManagerPrefabPath;

        public static bool _isPresent = false;

        public FabricSpringBoard()
        {
            _isPresent = true;
        }

        void OnEnable()
        {
            _isPresent = true;
        }

        void Awake()
        {
            Load();
        }

        public void Load()
        {
            FabricManager fabricAudioManager = GetFabricManagerInEditor();

            if (!fabricAudioManager)
            {
                GameObject prefabGO = Resources.Load(_fabricManagerPrefabPath, typeof(GameObject)) as GameObject;

                if (prefabGO)
                {

#if UNITY_EDITOR			
                    if(!EditorApplication.isPlayingOrWillChangePlaymode && Application.isEditor)
				    {
					    //GameObject fabricAudioManagerGO = PrefabUtility.InstantiatePrefab(prefabGO) as GameObject;
					    //GameObjectUtils.SetHideFlagsRecursive(fabricAudioManagerGO, HideFlags.DontSave, true);	
					    //GameObjectUtils.SetSavableRecursive(fabricAudioManagerGO, false);	
					    //Debug.Log("Creating");
				    }
				    else
#endif
                    {
                        GameObject.Instantiate(prefabGO);
                        //Debug.Log("Creating");
                    }
                }
            }
        }

#if UNITY_EDITOR	
    
    public void Unload()
    {
        DestroyFabricManagerInEditor();
    }
    
    static public void DestroyFabricManagerInEditor()
    {
        if (GetFabricManagerInEditor() != null)
        {
            GameObject.DestroyImmediate(GetFabricManagerInEditor().gameObject);
        }	
    }
	
	void OnDestroy()
	{
		if(Application.isEditor && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
		{
			//Debug.Log("Destroying " + UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode + UnityEditor.EditorApplication.isPlaying);
			if(GetFabricManagerInEditor() != null)
			{
				GameObject.DestroyImmediate(GetFabricManagerInEditor().gameObject);	
			}
		}	
	
        _isPresent = false;
	}
	
	void OnDisable()
	{
        //Debug.Log("On Disable");
        //if(Application.isEditor && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        //{
        //    if(GetFabricManagerInEditor() != null)
        //    {
        //        Debug.Log("Destroying");
        //        GameObject.DestroyImmediate(GetFabricManagerInEditor().gameObject);	
        //    }
        //}
	}	
#endif
        static public FabricManager GetFabricManagerInEditor()
        {
            FabricManager[] fabricManagers = Resources.FindObjectsOfTypeAll(typeof(FabricManager)) as FabricManager[];
            for (int i = 0; i < fabricManagers.Length; i++)
            {
                if (fabricManagers[i].gameObject != null && fabricManagers[i].hideFlags != HideFlags.HideInHierarchy)
                {
                    return fabricManagers[i];
                }
            }
            return null;
        }
    }
}
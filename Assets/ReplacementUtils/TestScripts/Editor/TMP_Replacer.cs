#if UNITY_EDITOR

using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Replacement;

public class TMP_Replacer : MonoBehaviour
{
	
    [MenuItem("Tools/Replace Text Mesh Pro")]
    public static void ReplaceTextMeshPro(MenuCommand command)
    {
        bool saveModifications = false;

        // Find the prefab with the data we want
        var referencer = Resources.Load<TMP_Replacer_Referencer>("TMP_Replacer_Referencer");

        var editorSceneInfos = EditorBuildSettings.scenes;
        foreach (var editorSceneInfo in editorSceneInfos)
        {
            if (!editorSceneInfo.enabled) continue;

            Debug.Log("Checking scene: " + editorSceneInfo.path);
            var scene = EditorSceneManager.OpenScene(editorSceneInfo.path);
            if (!scene.isLoaded) continue;
            if (!scene.IsValid()) continue;

            PerformFullReplacement<TextMeshPro_OLD, TextMeshPro>(referencer);
            PerformFullReplacement<TextMeshProUGUI_OLD, TextMeshProUGUI>(referencer);

            if (saveModifications)
            {
                if (!EditorSceneManager.MarkSceneDirty(scene)) Debug.Log("Not set as dirty!");
                if (!EditorSceneManager.SaveScene(scene)) Debug.Log("Not saved!");

                // Reopen and close the scene to fix references
                EditorSceneManager.OpenScene(editorSceneInfo.path);
            }

            // ReplaceAllComponentsOfType<TextMeshProUGUI_OLD, TextMeshProUGUI>();
        }

    }

    static void PerformFullReplacement<TFrom, TTo>(TMP_Replacer_Referencer referencer)    
            where TFrom : Component, TTo
            where TTo : Component
    {
    
        // Get all references to From
        var referencesDict = ReplacementUtility.CollectObjectsReferencingComponent<TFrom>();
        foreach (var pair in referencesDict)
            Debug.Log("Found " + pair.Value.Count + " references of " + ReplacementUtility.ToS(pair.Key));

        // Get all references of old assets from From
        var dependencyDict = ReplacementUtility.CollectObjectsOfTypeTheComponentDependsOn<TFrom, TMP_FontAsset_OLD>();
        foreach (var pair in dependencyDict)
            Debug.Log("Found " + pair.Value.Count + " dependencies for " + ReplacementUtility.ToS(pair.Key));

        // Replace all references with To
        var replacementDict = ReplacementUtility.ReplaceAllComponentsOfType<TFrom, TTo>();
        foreach (var pair in replacementDict)
            Debug.Log("Replaced component " + ReplacementUtility.ToS(pair.Key) + " with " + ReplacementUtility.ToS(pair.Value));

        // Replace all references in To for Assets that From referred to that should now be different (could not copy as field as they are different now)
        ReplacementUtility.PlaceAllAssetReferencesMirroring<TFrom,TTo, TMP_FontAsset_OLD, TMP_FontAsset, FontReplacementPair>(replacementDict, referencer.fonts, dependencyDict);

        // Replace all references for Components that referred to From and now should refer to To
        ReplacementUtility.ReplaceAllComponentReferences(replacementDict, referencesDict);
    }
}

#endif
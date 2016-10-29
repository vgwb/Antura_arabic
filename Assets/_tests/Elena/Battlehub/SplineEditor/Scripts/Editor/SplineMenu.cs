using UnityEngine;
using UnityEditor;
using System.Linq;

using Battlehub.RTHandles;
namespace Battlehub.SplineEditor
{
    public static class SplineMenu
    {
        const string root = "Battlehub/SplineEditor/";

        [MenuItem("Tools/Spline/Create")]
        public static void Create()
        {            
            GameObject spline = new GameObject();
            spline.name = "Spline";

            Undo.RegisterCreatedObjectUndo(spline, "Battlehub.Spline.Create");

            Spline splineComponent = spline.AddComponent<Spline>();
            splineComponent.SetControlPointMode(ControlPointMode.Mirrored);

            Camera sceneCam = SceneView.lastActiveSceneView.camera;
            spline.transform.position = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 5f));

            Selection.activeGameObject = spline.gameObject;
        }

        [MenuItem("Tools/Spline/Create Runtime Editor", validate = true)]
        public static bool CanCreateRuntimeEditor()
        {
            return !Object.FindObjectOfType<SplineRuntimeEditor>() && SplineRuntimeEditor.Instance == null;
        }

        [MenuItem("Tools/Spline/Create Runtime Editor")]
        public static void CreateRuntimeEditor()
        {
            GameObject uiCommandsGo = InstantiatePrefab("CommandsPanel.prefab");
            CreateRuntimeEditor(uiCommandsGo, "Spline Runtime Editor");
        }

        public static void CreateRuntimeEditor(GameObject commandsPanel, string name)
        {
            GameObject go = new GameObject();
            go.name = name;;
            go.AddComponent<SplineRuntimeEditor>();
            go.AddComponent<RuntimeSceneView>();

            GameObject uiEditorGO = InstantiatePrefab("EditorUI.prefab");
            uiEditorGO.transform.SetParent(go.transform, false);
            commandsPanel.transform.SetParent(uiEditorGO.transform, false);

            Undo.RegisterCreatedObjectUndo(go, "Battlehub.Spline.CreateRuntimeEditor");
        }

        public static GameObject InstantiatePrefab(string name)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath("Assets/" + root + "Prefabs/" + name, typeof(GameObject));
            return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        }

        [MenuItem("Tools/Spline/Set Mode/Free", validate = true)]
        private static bool CanSetFreeMode()
        {
            return CanSetMode();
        }

        [MenuItem("Tools/Spline/Set Mode/Aligned", validate = true)]
        private static bool CanSetAlignedMode()
        {
            return CanSetMode();
        }

        [MenuItem("Tools/Spline/Set Mode/Mirrored", validate = true)]
        private static bool CanSetMirroredMode()
        {
            return CanSetMode();
        }

        private static bool CanSetMode()
        {
            GameObject[] selected = Selection.gameObjects;
            return selected.Any(s => s.GetComponentInParent<Spline>());
        }

        [MenuItem("Tools/Spline/Set Mode/Free")]
        private static void SetFreeMode()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                SetMode(gameObjects[i], ControlPointMode.Free);
            }

        }

        [MenuItem("Tools/Spline/Set Mode/Aligned")]
        private static void SetAlignedMode()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                SetMode(gameObjects[i], ControlPointMode.Aligned);
            }
        }

        [MenuItem("Tools/Spline/Set Mode/Mirrored")]
        private static void SetMirroredMode()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                SetMode(gameObjects[i], ControlPointMode.Mirrored);
            }
        }

        private static void SetMode(GameObject selected, ControlPointMode mode)
        {
            Spline spline = selected.GetComponentInParent<Spline>();
            if (spline == null)
            {
                return;
            }

            SplineControlPoint selectedControlPoint = selected.GetComponent<SplineControlPoint>();
            Undo.RecordObject(spline, "Battlehub.Spline.SetMode");
            EditorUtility.SetDirty(spline);

            if (selectedControlPoint != null)
            {
                spline.SetControlPointMode(selectedControlPoint.Index, mode);
            }
            else
            {
                spline.SetControlPointMode(mode);
            }
        }

        [MenuItem("Tools/Spline/Append _&4", validate = true)]
        private static bool CanAppend()
        {
            GameObject selected = Selection.activeObject as GameObject;
            if (selected == null)
            {
                return false;
            }

            return selected.GetComponentInParent<Spline>();
        }

        [MenuItem("Tools/Spline/Append _&4")]
        private static void Append()
        {
            GameObject selected = Selection.activeObject as GameObject;
            Spline spline = selected.GetComponentInParent<Spline>();
            Undo.RecordObject(spline, "Battlehub.Spline.Append");
            spline.Extend();
            EditorUtility.SetDirty(spline);
            Selection.activeGameObject = spline.GetComponentsInChildren<SplineControlPoint>(true).Last().gameObject;
        }

        [MenuItem("Tools/Spline/Prepend _&5", validate = true)]
        private static bool CanPrepend()
        {
            GameObject selected = Selection.activeObject as GameObject;
            if (selected == null)
            {
                return false;
            }

            return selected.GetComponentInParent<Spline>();
        }

        [MenuItem("Tools/Spline/Prepend _&5")]
        private static void Prepend()
        {
            GameObject selected = Selection.activeObject as GameObject;
            Spline spline = selected.GetComponentInParent<Spline>();
            Undo.RecordObject(spline, "Battlehub.Spline.Prepend");
            spline.Extend(true);
            EditorUtility.SetDirty(spline);
            Selection.activeGameObject = spline.GetComponentsInChildren<SplineControlPoint>(true).First().gameObject;
        }

        [MenuItem("Tools/Spline/Remove Curve", validate = true)]
        private static bool CanRemove()
        {
            GameObject selected = Selection.activeObject as GameObject;
            if (selected == null)
            {
                return false;
            }

            return selected.GetComponent<SplineControlPoint>() && selected.GetComponentInParent<Spline>();
        }

        [MenuItem("Tools/Spline/Remove Curve")]
        private static void Remove()
        {
            GameObject selected = Selection.activeObject as GameObject;
            SplineControlPoint ctrlPoint = selected.GetComponent<SplineControlPoint>();
            Spline spline = selected.GetComponentInParent<Spline>();
            Selection.activeGameObject = spline.gameObject;
            Undo.RecordObject(spline, "Battlehub.Spline.Remove");
            spline.Remove((ctrlPoint.Index - 1) / 3);
            EditorUtility.SetDirty(spline);
        }

        [MenuItem("Tools/Spline/Smooth", validate = true)]
        private static bool CanSmooth()
        {
            GameObject selected = Selection.activeObject as GameObject;
            if (selected == null)
            {
                return false;
            }

            return selected.GetComponentInParent<Spline>();
        }

        [MenuItem("Tools/Spline/Smooth")]
        private static void Smooth()
        {
            GameObject selected = Selection.activeObject as GameObject;
            Spline spline = selected.GetComponentInParent<Spline>();
            Undo.RecordObject(spline, "Battlehub.Spline.Remove");
            spline.Smooth();
            EditorUtility.SetDirty(spline);
        }



    }
}


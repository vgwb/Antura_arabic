using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Fabric
{
    [CustomEditor(typeof(FabricSpringBoard))]
    public class FabricSpringBoardEditor : Editor
    {
        [MenuItem("Fabric/Utils/SpringBoard")]
        static void About()
        {
            GameObject component = new GameObject("Fabric SpringBoard");

            component.AddComponent<FabricSpringBoard>();

            GameObject target = Selection.activeGameObject;
            if (target != null)
            {
                component.transform.parent = target.transform;
            }
        }

        FabricSpringBoard springBoard;

        private void OnEnable()
        {
            springBoard = target as FabricSpringBoard;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal("Box");

            GUILayout.Label("Fabric Prefab:", GUILayout.MaxWidth(100));

            Rect drop_area = GUILayoutUtility.GetRect(100.0f, 20.0f, GUILayout.ExpandWidth(true));

            string label = "Drop Fabric Manager Prefab here!!";

            var orig = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (springBoard._fabricManagerPrefabPath != null && springBoard._fabricManagerPrefabPath != "")
            {
                GUI.backgroundColor = Color.green;
                label = springBoard._fabricManagerPrefabPath;
            }

            GUI.Box(drop_area, label);
            GUI.backgroundColor = orig;

            DragAndDropAudioClip(drop_area, ref springBoard._fabricManagerPrefabPath);

            if (GUILayout.Button("Clear", GUILayout.MaxWidth(50)))
            {
                springBoard._fabricManagerPrefabPath = "";
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load"))
            {
                FabricSpringBoard fabricSpringBoard = target as FabricSpringBoard;
                fabricSpringBoard.Load();
            }

            if (GUILayout.Button("Unlaod"))
            {
                FabricSpringBoard fabricSpringBoard = target as FabricSpringBoard;
                fabricSpringBoard.Unload();
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drop_area"></param>
        /// <param name="audioClipPath"></param>
        void DragAndDropAudioClip(Rect drop_area, ref string audioClipPath)
        {
            UnityEngine.Event evt = UnityEngine.Event.current;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        UnityEngine.Object dragged_object = DragAndDrop.objectReferences[0];

                        if (dragged_object != null)
                        {
                            audioClipPath = AssetDatabase.GetAssetPath(dragged_object);

                            int index = audioClipPath.LastIndexOf("Resources/");
                            if (index >= 0)
                            {
                                audioClipPath = audioClipPath.Remove(0, index);
                                audioClipPath = audioClipPath.Replace("Resources/", "");
                                audioClipPath = audioClipPath.Replace(".prefab", "");
                            }
                        }
                    }
                    break;
            }
        }
    }
}
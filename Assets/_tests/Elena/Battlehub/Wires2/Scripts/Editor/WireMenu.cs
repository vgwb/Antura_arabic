using UnityEngine;
using UnityEditor;
using System.Linq;
using Battlehub.MeshDeformer2;
using Battlehub.MeshTools;
using Battlehub.Integration;
using Battlehub.SplineEditor;

namespace Battlehub.Wire2
{
    public static class WireMenu
    {
        private const string m_root = "Battlehub/Wires2/";

        static WireMenu()
        {
            MeshCombinerIntegration.BeginEditPivot += OnBeginEditPivot;
            MeshCombinerIntegration.Combined += OnCombined;
        }

        private static void OnBeginEditPivot(IntegrationArgs args)
        {
            GameObject go = args.GameObject;
            Wire wire = go.GetComponentInParent<Wire>();
            if (wire != null)
            {
                if (!Rollback(wire.gameObject))
                {
                    args.Cancel = true;
                }
            }
        }

        private static void OnCombined(IntegrationArgs args)
        {
            GameObject go = args.GameObject;
            if (go.GetComponent<MeshDeformer>() != null)
            {
                CleanupCombined(go);
            }
        }

        [MenuItem("Tools/Wires/Create")]
        public static void Create()
        {
            GameObject wire = InstantiatePrefab("Wire.prefab", m_root);
            if(!WiresIntegration.RaiseBeforeWireCreated(wire, null))
            {
                Object.DestroyImmediate(wire);
                return;
            }
            Undo.RegisterCreatedObjectUndo(wire, "Battlehub.Wire Create");
            

            wire.AddComponent<Wire>();

            Camera sceneCam = SceneView.lastActiveSceneView.camera;
            wire.transform.position = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 5f));
           
            Selection.activeGameObject = wire.gameObject;
        }


        [MenuItem("Tools/Wires/Create Runtime Editor", validate = true)]
        public static bool CanCreateRuntimeEditor()
        {
            return SplineMenu.CanCreateRuntimeEditor();
        }

        [MenuItem("Tools/Wires/Create Runtime Editor")]
        public static void CreateRuntimeEditor()
        {
            const string root = "Battlehub/MeshDeformer2/";
            GameObject commandsPanelGO = InstantiatePrefab("CommandsPanel.prefab", root);
            SplineMenu.CreateRuntimeEditor(commandsPanelGO, "Mesh Deformer Runtime Component");
        }

        [MenuItem("Tools/Wires/Set Spline Mode/Rigid", validate = true)]
        private static bool CanSetRigidMode()
        {
            return CanSetMode();
        }

        [MenuItem("Tools/Wires/Set Spline Mode/Free", validate = true)]
        private static bool CanSetFreeMode()
        {
            return CanSetMode();
        }

        [MenuItem("Tools/Wires/Set Spline Mode/Aligned", validate = true)]
        private static bool CanSetAlignedMode()
        {
            return CanSetMode();
        }

        [MenuItem("Tools/Wires/Set Spline Mode/Mirrored", validate = true)]
        private static bool CanSetMirroredMode()
        {
            return CanSetMode();
        }

        private static bool CanSetMode()
        {
            GameObject[] selected = Selection.gameObjects;
            return selected.Any(s => s.GetComponentInParent<Wire>());
        }

        [MenuItem("Tools/Wires/Set Spline Mode/Rigid")]
        private static void SetRigidMode()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                SetMode(gameObjects[i], true, ControlPointMode.Free);
            }
        }

        [MenuItem("Tools/Wires/Set Spline Mode/Free")]
        private static void SetFreeMode()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                SetMode(gameObjects[i], false, ControlPointMode.Free);
            }

        }

        [MenuItem("Tools/Wires/Set Spline Mode/Aligned")]
        private static void SetAlignedMode()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                SetMode(gameObjects[i], false, ControlPointMode.Free);
            }
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                SetMode(gameObjects[i], false, ControlPointMode.Aligned);
            }
        }

        [MenuItem("Tools/Wires/Set Spline Mode/Mirrored")]
        private static void SetMirroredMode()
        {
            GameObject[] gameObjects = Selection.gameObjects;
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                SetMode(gameObjects[i], false, ControlPointMode.Free);
            }
            for (int i = 0; i < gameObjects.Length; ++i)
            {
                SetMode(gameObjects[i], false, ControlPointMode.Mirrored);
            }
        }

        private static void SetMode(GameObject selected, bool isRigid, ControlPointMode mode)
        {
            MeshDeformer meshDeformer = selected.GetComponentInParent<MeshDeformer>();
            if (meshDeformer == null)
            {
                return;
            }
            Scaffold selectedScaffold = selected.GetComponent<Scaffold>();
            ControlPoint selectedControlPoint = selected.GetComponent<ControlPoint>();


            Undo.RecordObject(meshDeformer, "Battlehub.MeshDeformer.SetRigidMode");
            MeshDeformerEditor.RecordScaffolds(meshDeformer, "Battlehub.MeshDeformer.SetRigidMode");
            EditorUtility.SetDirty(meshDeformer);

            if (selectedScaffold != null && selectedScaffold.gameObject != meshDeformer.gameObject)
            {
                ScaffoldWrapper scaffold = meshDeformer.Scaffolds.Where(s => s.Obj == selectedScaffold).FirstOrDefault();
                if (scaffold != null)
                {
                    for (int i = 0; i < scaffold.CurveIndices.Length; ++i)
                    {
                        int curveIndex = scaffold.CurveIndices[i];
                        if (mode == ControlPointMode.Free)
                        {
                            meshDeformer.SetIsRigid(curveIndex * 3, isRigid);
                        }

                        if (!isRigid)
                        {
                            meshDeformer.SetControlPointMode(curveIndex * 3, mode);
                            meshDeformer.SetControlPointMode(curveIndex * 3 + 3, mode);
                        }
                    }
                }
                else
                {
                    Debug.LogError("scaffold not found");
                }

            }
            else if (selectedControlPoint != null)
            {
                if (mode == ControlPointMode.Free)
                {
                    meshDeformer.SetIsRigid(selectedControlPoint.Index, isRigid);
                }

                if (!isRigid)
                {
                    meshDeformer.SetControlPointMode(selectedControlPoint.Index, mode);
                }
            }
            else
            {
                ScaffoldWrapper[] scaffolds = meshDeformer.Scaffolds;
                for (int s = 0; s < scaffolds.Length; ++s)
                {
                    ScaffoldWrapper scaffold = scaffolds[s];
                    for (int i = 0; i < scaffold.CurveIndices.Length; ++i)
                    {
                        int curveIndex = scaffold.CurveIndices[i];
                        if (mode == ControlPointMode.Free)
                        {
                            meshDeformer.SetIsRigid(curveIndex * 3, isRigid);
                        }

                        if (!isRigid)
                        {
                            meshDeformer.SetControlPointMode(curveIndex * 3, mode);
                            meshDeformer.SetControlPointMode(curveIndex * 3 + 3, mode);
                        }
                    }
                }
            }
        }

        [MenuItem("Tools/Wires/Append _&1", validate = true)]
        private static bool CanAppend()
        {
            GameObject selected = Selection.activeObject as GameObject;
            if (selected == null)
            {
                return false;
            }

            return selected.GetComponentInParent<Wire>();
        }

        [MenuItem("Tools/Wires/Append _&1")]
        private static void Append()
        {
            GameObject selected = Selection.activeObject as GameObject;
            Wire wire = selected.GetComponentInParent<Wire>();
            Undo.RecordObject(wire, "Battlehub.Wire.Append");
            Undo.RegisterCreatedObjectUndo(wire.Extend(), "Battlehub.Wire.Append");
            EditorUtility.SetDirty(wire);
            Selection.activeGameObject = wire.GetComponentsInChildren<ControlPoint>(true).Last().gameObject;
        }

        [MenuItem("Tools/Wires/Prepend _&2", validate = true)]
        private static bool CanPrepend()
        {
            GameObject selected = Selection.activeObject as GameObject;
            if (selected == null)
            {
                return false;
            }

            return selected.GetComponentInParent<Wire>();
        }

        [MenuItem("Tools/Wires/Prepend _&2")]
        private static void Prepend()
        {
            GameObject selected = Selection.activeObject as GameObject;
            Wire wire = selected.GetComponentInParent<Wire>();
            Undo.RecordObject(wire, "Battlehub.Wire.Prepend");
            Scaffold[] scaffolds = wire.GetComponentsInChildren<Scaffold>();
            foreach (Scaffold scaffold in scaffolds)
            {
                Undo.RecordObject(scaffold, "Battlehub.Wire.Prepend");
            }
            Undo.RegisterCreatedObjectUndo(wire.Extend(true), "Battlehub.Wire.Prepend");
            EditorUtility.SetDirty(wire);
            Selection.activeGameObject = wire.GetComponentsInChildren<ControlPoint>(true).First().gameObject;
        }

        [MenuItem("Tools/Wires/Remove Curve", validate = true)]
        private static bool CanRemove()
        {
            GameObject selected = Selection.activeObject as GameObject;
            if (selected == null)
            {
                return false;
            }

            return selected.GetComponent<ControlPoint>() && selected.GetComponentInParent<Wire>();
        }

        [MenuItem("Tools/Wires/Remove Curve")]
        private static void Remove()
        {
            GameObject selected = Selection.activeObject as GameObject;
            ControlPoint ctrlPoint = selected.GetComponent<ControlPoint>();
            Wire wire = selected.GetComponentInParent<Wire>();
            Selection.activeGameObject = wire.gameObject;

            Undo.RecordObject(wire, "Battlehub.Wire.Remove");
            MeshDeformerEditor.RecordScaffolds(wire, "Battlehub.Wire.Remove");

            GameObject removeObject;
            wire.Remove((ctrlPoint.Index - 1) / 3, out removeObject);
            if (removeObject != null)
            {
                Undo.DestroyObjectImmediate(removeObject);
            }

            EditorUtility.SetDirty(wire);
        }

        [MenuItem("Tools/Wires/Duplicate", validate = true)]
        private static bool CanDuplicate()
        {
            GameObject selected = Selection.activeObject as GameObject;
            return selected != null && selected.GetComponentInParent<Wire>() != null;
        }

        [MenuItem("Tools/Wires/Duplicate")]
        private static void Duplicate()
        {
            GameObject selected = Selection.activeObject as GameObject;
            GameObject copy = GameObject.Instantiate(selected.GetComponentInParent<Wire>().gameObject);

            Undo.RegisterCreatedObjectUndo(copy, "Battlehub.Wires.Duplicate");
            Wire wire = copy.GetComponentInParent<Wire>();
            wire.WrapAndDeformAll();

            Selection.activeGameObject = copy;
        }

        [MenuItem("Tools/Wires/Postprocessing/Create LineRenderer", validate = true)]
        private static bool CanCreateLineRenderer()
        {
            GameObject selected = Selection.activeObject as GameObject;
            if (selected == null)
            {
                return false;
            }

            return selected.GetComponentInParent<Wire>();
        }
        //[MenuItem("Tools/Wires/Postprocessing/Smooth Spline", validate = true)]
        //private static bool CanFit()
        //{
        //    GameObject selected = Selection.activeObject as GameObject;
        //    if (selected == null)
        //    {
        //        return false;
        //    }

        //    return selected.GetComponentInParent<Wire>();
        //}

        //[MenuItem("Tools/Wires/Postprocessing/Smooth Spline")]
        //private static void Fit()
        //{
        //    GameObject selected = Selection.activeObject as GameObject;
        //    Spline spline = selected.GetComponentInParent<Spline>();
        //    if (spline is MeshDeformer)
        //    {
        //        MeshDeformer deformer = (MeshDeformer)spline;
        //        Undo.RecordObject(deformer, "Battlehub.Wires.Fit");
        //        MeshDeformerEditor.RecordScaffolds(deformer, "Battlehub.Wires.Fit");
        //        EditorUtility.SetDirty(deformer);
        //    }
        //    else
        //    {
        //        Undo.RecordObject(spline, "Battlehub.Wires.Fit");
        //        EditorUtility.SetDirty(spline);
        //    }

        //    spline.Smooth();
        //}

        [MenuItem("Tools/Wires/Postprocessing/Create LineRenderer")]
        private static void CreateLineRenderer()
        {
            GameObject selected = Selection.activeObject as GameObject;
            Wire wire = selected.GetComponentInParent<Wire>();

            GameObject lineWire = InstantiatePrefab("LineWire.prefab", m_root);
            Undo.RegisterCreatedObjectUndo(lineWire, "Battlehub.Wires.Create Line Renderer");

            lineWire.transform.position = wire.transform.position;
            lineWire.transform.rotation = wire.transform.rotation;
            lineWire.transform.localScale = wire.transform.localScale;

            LineRenderer lineRenderer = lineWire.GetComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;

            int pointsCount = wire.Approximation * wire.CurveCount + 1;
            Vector3[] points = new Vector3[pointsCount];
            for(int i = 0; i < points.Length; ++i)
            {
                float t = i;
                t = t / (points.Length - 1);
                points[i] =  wire.GetPointLocal(t);
            }

            lineRenderer.SetVertexCount(points.Length);
            lineRenderer.SetPositions(points);
            lineRenderer.SetWidth(wire.WireRadius * 2, wire.WireRadius * 2);

            Selection.activeGameObject = lineWire;
        }



        [MenuItem("Tools/Wires/Postprocessing/Remove Deformer", validate = true)]
        private static bool CanRemoveDeformer()
        {
            GameObject selected = Selection.activeObject as GameObject;
            if (selected == null)
            {
                return false;
            }

            return selected.GetComponentInParent<Wire>();
        }

        [MenuItem("Tools/Wires/Postprocessing/Remove Deformer")]
        private static void RemoveDeformer()
        {
            GameObject selected = Selection.activeObject as GameObject;
            MeshDeformer deformer = selected.GetComponentInParent<MeshDeformer>();
            ControlPoint[] controlPoints = deformer.GetComponentsInChildren<ControlPoint>(true);
            for (int i = 0; i < controlPoints.Length; ++i)
            {
                Undo.DestroyObjectImmediate(controlPoints[i].gameObject);
            }

            Scaffold[] scaffolds = deformer.GetComponentsInChildren<Scaffold>();
            for (int i = 0; i < scaffolds.Length; ++i)
            {
                Undo.DestroyObjectImmediate(scaffolds[i]);
            }

            Undo.DestroyObjectImmediate(deformer);
        }

        [MenuItem("Tools/Wires/Postprocessing/Rollback", validate = true)]
        private static bool CanRollback()
        {
            GameObject selected = Selection.activeObject as GameObject;
            if (selected == null)
            {
                return false;
            }

            return selected.GetComponentInParent<Wire>();
        }


        [MenuItem("Tools/Wires/Postprocessing/Rollback")]
        private static void Rollback()
        {
            GameObject selected = Selection.activeObject as GameObject;
            Rollback(selected);
        }

        private static bool Rollback(GameObject selected)
        {
            MeshDeformer deformer = selected.GetComponentInParent<MeshDeformer>();
            if (deformer != null)
            {
                selected = deformer.gameObject;
                Selection.activeGameObject = selected;
            }

            MeshFilter meshFilter = selected.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                EditorUtility.DisplayDialog("MeshFilter required", "Select object with MeshFilter component", "OK");
                return false;
            }

          
            if (deformer != null)
            {
                bool ok = !EditorUtility.DisplayDialog("Are you sure?", "This action is irreversible. Deformation will be lost", "Yes", "No");
                if (!ok)
                {
                    return false;
                }
                ControlPoint[] controlPoints = deformer.GetComponentsInChildren<ControlPoint>(true);
                for (int i = 0; i < controlPoints.Length; ++i)
                {
                    Object.DestroyImmediate(controlPoints[i].gameObject);
                }

                Scaffold[] scaffolds = deformer.GetComponentsInChildren<Scaffold>();
                for (int i = 0; i < scaffolds.Length; ++i)
                {
                    if (scaffolds[i].gameObject != deformer.gameObject)
                    {
                        Object.DestroyImmediate(scaffolds[i].gameObject);
                    }
                }

                Mesh original = deformer.Original;
                meshFilter.sharedMesh = original;

                Object.DestroyImmediate(deformer);
            }

            Scaffold scaffold = selected.GetComponent<Scaffold>();
            if (scaffold != null)
            {
                Object.DestroyImmediate(scaffold);
            }

            return true;
        }

        [MenuItem("Tools/Wires/Postprocessing/Combine And Save", validate = true)]
        private static bool CanCombineAndSave()
        {
            GameObject selected = Selection.activeObject as GameObject;
            if (selected == null)
            {
                return false;
            }

            return selected.GetComponentInParent<Wire>();
        }

        [MenuItem("Tools/Wires/Postprocessing/Combine And Save")]
        private static void CombineAndSave()
        {
            GameObject selected = Selection.activeObject as GameObject;
            MeshDeformer deformer = selected.GetComponentInParent<MeshDeformer>();
            GameObject[] gameObjects = deformer.GetComponentsInChildren<Scaffold>().Select(s => s.gameObject).ToArray();

            CombineResult combineResult = MeshUtils.Combine(gameObjects, deformer.gameObject);
            if (combineResult != null)
            {
                CleanupCombined(combineResult.GameObject);
                MeshUtils.SaveMesh(new[] { combineResult.GameObject }, "Battlehub/");
            }
            else
            {
                Debug.LogWarning("Unable to Combine and Save");
            }
        }

        private static void CleanupCombined(GameObject gameObject)
        {
            MeshDeformer deformer = gameObject.GetComponent<MeshDeformer>();
            if (deformer != null)
            {
                Object.DestroyImmediate(deformer);
            }
            Scaffold scaffold = gameObject.GetComponent<Scaffold>();
            if (scaffold != null)
            {
                Object.DestroyImmediate(scaffold);
            }
        }

        public static GameObject InstantiatePrefab(string name, string root)
        {
            Object prefab = AssetDatabase.LoadAssetAtPath("Assets/" + root + "Prefabs/" + name, typeof(GameObject));
            return (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        }
    }
}


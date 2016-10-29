using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Battlehub.WiresLegacy
{
	public class WireMenu 
	{


		// Add a new menu item under an existing menu

		private static string root = "Battlehub/WiresLegacy/";
	
        
		[MenuItem("Tools/Wire Legacy/Create")]
		private static void Create()
		{
		
			string wireMaterialPath = "Assets/"  + root + "Wire.mat"; 
			Material wireMaterial = (Material)AssetDatabase.LoadAssetAtPath(wireMaterialPath, typeof(Material));


			GameObject wire;

			GameObject[] selectedObjects = SelectionHelper.SelectedObjects.OfType<GameObject>().ToArray();

			if(selectedObjects.Length >= 3)
			{
				wire = new GameObject("Wire", typeof(Wire));
				wire.transform.position = selectedObjects[0].transform.position;
				Wire wireComponent = wire.GetComponent<Wire>();
				wireComponent.WireMaterial = wireMaterial;

				/*
				foreach(GameObject obj in selectedObjects)
				{
					obj.transform.parent = wire.transform;
				}*/


				Object knotStartPrefab = AssetDatabase.LoadAssetAtPath("Assets/" + root + "Prefabs/WireStart.prefab", typeof(GameObject));
				GameObject knotStart = (GameObject)PrefabUtility.InstantiatePrefab(knotStartPrefab);
				knotStart.transform.SetParent(wire.transform);
				knotStart.transform.position = selectedObjects[0].transform.position;

				for(int i = 1; i < selectedObjects.Length - 1; ++i)
				{
					Object knotPrefab = AssetDatabase.LoadAssetAtPath("Assets/" + root + "Prefabs/Knot.prefab", typeof(GameObject));
					GameObject knot1 = (GameObject)PrefabUtility.InstantiatePrefab(knotPrefab);
					knot1.transform.SetParent(wire.transform);
					knot1.transform.position = selectedObjects[i].transform.position;
				}

				Object knotEndPrefab = AssetDatabase.LoadAssetAtPath("Assets/" + root + "Prefabs/WireEnd.prefab", typeof(GameObject));
				GameObject knotEnd =(GameObject)PrefabUtility.InstantiatePrefab(knotEndPrefab);
				knotEnd.transform.SetParent(wire.transform);
				knotEnd.transform.position = selectedObjects[selectedObjects.Length - 1].transform.position;

				SelectionHelper.Clear();
				Selection.activeObject = SceneView.currentDrawingSceneView;
			}
			else
			{
                Selection.activeObject = SceneView.lastActiveSceneView;
                Camera sceneCam = SceneView.lastActiveSceneView.camera;
                Vector3 spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f,0.5f,10f));
				
				wire = new GameObject("Wire", typeof(Wire));
				Wire wireComponent = wire.GetComponent<Wire>();
				wireComponent.WireMaterial = wireMaterial;
				wire.transform.position = spawnPos;

				Object knotStartPrefab = AssetDatabase.LoadAssetAtPath("Assets/" + root + "Prefabs/WireStart.prefab", typeof(GameObject));
				GameObject knotStart = (GameObject)PrefabUtility.InstantiatePrefab(knotStartPrefab);
				knotStart.transform.parent = wire.transform;
				knotStart.transform.localPosition = new Vector3(0.0f, 0.0f, 10.0f);
				
				
				Object knotPrefab = AssetDatabase.LoadAssetAtPath("Assets/" + root + "Prefabs/Knot.prefab", typeof(GameObject));
				GameObject knot1 = (GameObject)PrefabUtility.InstantiatePrefab(knotPrefab);
				knot1.transform.parent = wire.transform;
				knot1.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
				
				Object knotEndPrefab = AssetDatabase.LoadAssetAtPath("Assets/" + root + "Prefabs/WireEnd.prefab", typeof(GameObject));
				GameObject knotEnd =(GameObject)PrefabUtility.InstantiatePrefab(knotEndPrefab);
				knotEnd.transform.parent = wire.transform;
				knotEnd.transform.localPosition = new Vector3(0.0f, 0.0f, -10.0f);

			}

		
			Selection.activeObject = wire;
		}

		[MenuItem("Tools/Wire Legacy/Duplicate Knot")]
		private static void DuplicateKnot()
		{
			GameObject knot = Selection.activeObject as GameObject;
			if(knot == null)
			{
				Debug.LogWarning("Nothing to duplicate. Select knot.");
				return;
			}

			Knot knotComponent = knot.GetComponent<Knot>();
			if(knotComponent == null)
			{
				Debug.LogWarning("Nothing to duplicate. Select knot.");
				return;
			}

			if(knotComponent.transform.parent == null)
			{
				Debug.LogWarning("Nothing to duplicate. Select child knot of wire");
				return;
			}

			GameObject wire = knotComponent.transform.parent.gameObject;
			Wire wireComponent = wire.GetComponent<Wire>();
			if(wireComponent == null)
			{
				Debug.LogWarning("Nothing to duplicate. Select child knot of wire");
				return;
			}

			List<Knot> knots = wire.GetComponentsInChildren<Knot>().ToList();
			wire.transform.DetachChildren();

			int index = knots.IndexOf(knotComponent);
			if(index == 0)
			{
				index++;
			}

			Object knotPrefab = AssetDatabase.LoadAssetAtPath("Assets/" + root + "Prefabs/Knot.prefab", typeof(GameObject));
			GameObject duplicate = (GameObject)PrefabUtility.InstantiatePrefab(knotPrefab);
			knots.Insert(index, duplicate.GetComponent<Knot>());

			foreach(Knot k in knots)
			{
				k.transform.SetParent(wire.transform);
			}
			duplicate.transform.localPosition = knotComponent.transform.localPosition;
			wireComponent.CreateWire();
			Selection.activeObject = duplicate;
		}

		[MenuItem("Tools/Wire Legacy/Remove Knot")]
		private static void RemoveKnot()
		{
			GameObject knot = Selection.activeObject as GameObject;
			if(knot == null)
			{
				Debug.LogWarning("Nothing to delete. Select knot.");
				return;
			}
			
			Knot knotComponent = knot.GetComponent<Knot>();
			if(knotComponent == null)
			{
				Debug.LogWarning("Nothing to delete. Select knot.");
				return;
			}
			
			if(knotComponent.transform.parent == null)
			{
				Debug.LogWarning("Nothing to delete. Select child knot of wire");
				return;
			}
			
			GameObject wire = knotComponent.transform.parent.gameObject;
			Wire wireComponent = wire.GetComponent<Wire>();
			if(wireComponent == null)
			{
				Debug.LogWarning("Nothing to delete. Select child knot of wire");
				return;
			}

			List<Knot> knots = wire.GetComponentsInChildren<Knot>().ToList();
			if(knots.Count <= 3)
			{
				Debug.LogWarning("Unable to delete knot. At least 3 knots required.");
				return;
			}


			wire.transform.DetachChildren();
			
			int index = knots.IndexOf(knotComponent);
			GameObject.DestroyImmediate(knots[index].gameObject);
			knots.RemoveAt(index);

			foreach(Knot k in knots)
			{
				k.transform.SetParent(wire.transform);
			}
			wireComponent.CreateWire();
		}

		[MenuItem("Tools/Wire Legacy/Subdivide")]
		private static void Subdivide()
		{
			GameObject wire = Selection.activeObject as GameObject;
			if(wire == null)
			{
				Debug.LogWarning("Nothing to subdivde. Select wire.");
				return;
			}

			Wire wireComponent = wire.GetComponent<Wire>();
			if(wireComponent == null)
			{
				Debug.LogWarning("Nothing to subdivde. Select wire.");
				return;
			}


			List<Knot> knots = wire.GetComponentsInChildren<Knot>().ToList();
			wire.transform.DetachChildren();

			int count = knots.Count;
			for(int i = count - 1; i >= 1; i-- )
			{
				Object knotPrefab = AssetDatabase.LoadAssetAtPath("Assets/" + root + "Prefabs/Knot.prefab", typeof(GameObject));
				GameObject knot = (GameObject)PrefabUtility.InstantiatePrefab(knotPrefab);
				knots.Insert(i, knot.GetComponent<Knot>());
			}

			foreach(Knot knot in knots)
			{
				knot.transform.SetParent(wire.transform);
			}

			VertexHelper helper = new VertexHelper();
			for(int i = 0; i < knots.Count - 2; i += 2 )
			{
				Knot knot = knots[i];
				Knot nextKnot = knots[i + 2];

				helper.Init(knot, nextKnot, wireComponent.Slices);
				helper.SetT(0.5f);
				knots[i + 1].transform.localPosition = helper.GetCenter();
			}

			wireComponent.LOD *= 2.0f;
			wireComponent.CreateWire();
		}
		

		// Add a menu item with multiple levels of nesting
		
		[MenuItem("Tools/Wire Legacy/Bake")]
		private static void Bake()
		{
			GameObject wire = Selection.activeObject as GameObject;
			if(wire == null)
			{
				Debug.LogWarning("Nothing to bake. Select wire.");
				return;
			}
			if(wire.GetComponent<Wire>() == null)
			{
				Debug.LogWarning("Nothing to bake. Select wire.");
				return;
			}
			MeshFilter filter = wire.GetComponent<MeshFilter>();
			if(filter == null)
			{
				Debug.LogWarning("Nothing to bake. Select wire with MeshFilter");
				return;
			}

			if(!System.IO.Directory.Exists(Application.dataPath + "/" + root + "/BakedWires"))
			{
				AssetDatabase.CreateFolder("Assets/" + root.Remove(root.Length - 1), "BakedWires");
			}
			//Mesh meshToSave = Object.Instantiate() as Mesh; 
			string path = "Assets/" + root + "BakedWires/BakedMesh" + System.Guid.NewGuid();
			Debug.Log("Mesh baked: " + path);
			AssetDatabase.CreateAsset(filter.sharedMesh, path);
			AssetDatabase.SaveAssets();

			//Mesh mesh = new Mesh();
		}

		[MenuItem("Tools/Wire Legacy/Clear Baked")]
		private static void ClearBaked()
		{
			GameObject wire = Selection.activeObject as GameObject;
			if(wire == null)
			{
				Debug.LogWarning("Nothing to clear. Select wire.");
				return;
			}
			if(wire.GetComponent<Wire>() == null)
			{
				Debug.LogWarning("Nothing to clear. Select wire.");
				return;
			}
			MeshFilter filter = wire.GetComponent<MeshFilter>();


			if(filter == null)
			{
				Debug.Log("Already cleared.");
				return;
			}

			string path = "Assets/" + root + "BakedWires/" + filter.sharedMesh.name;
			AssetDatabase.DeleteAsset(path);

			Debug.Log("Mesh cleared: " + path);
			Object.DestroyImmediate(filter);

			wire.GetComponent<Wire>().CreateWire();
		}
	}
}

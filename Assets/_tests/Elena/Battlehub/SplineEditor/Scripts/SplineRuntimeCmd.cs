using System.Runtime.Serialization;
using UnityEngine;
using Battlehub.RTHandles;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System;

namespace Battlehub.SplineEditor
{
    public sealed class VersionDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
            {
                Type typeToDeserialize = null;

                assemblyName = Assembly.GetExecutingAssembly().FullName;

                // The following line of code returns the type. 
                typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));

                return typeToDeserialize;
            }

            return null;
        }
    }



    public class SplineRuntimeCmd : MonoBehaviour
    {
        public virtual void Append()
        {
            if (SplineRuntimeEditor.Instance != null)
            {
                Spline spline = SplineRuntimeEditor.Instance.SelectedSpline as Spline;
                if (spline != null)
                {
                    spline.Extend();
                }
            }
        }

        public virtual void Prepend()
        {
            if (SplineRuntimeEditor.Instance != null)
            {
                Spline spline = SplineRuntimeEditor.Instance.SelectedSpline as Spline;
                if (spline != null)
                {
                    spline.Extend(true);
                }
            }
        }

  

        public virtual void Remove()
        {
            if (SplineRuntimeEditor.Instance != null)
            {
                Spline spline = SplineRuntimeEditor.Instance.SelectedSpline as Spline;
                if (spline != null)
                {
                    GameObject selection = RuntimeSelection.activeGameObject;
                    if (selection != null)
                    {
                        SplineControlPoint ctrlPoint = selection.GetComponent<SplineControlPoint>();
                        if (ctrlPoint != null)
                        {
                            spline.Remove((ctrlPoint.Index - 1) / 3);
                        }
                        RuntimeSelection.activeGameObject = spline.gameObject;
                    }
                }
            }
        }

    

        public virtual void Smooth()
        {
            if (SplineRuntimeEditor.Instance != null)
            {
                SplineBase spline = SplineRuntimeEditor.Instance.SelectedSpline;
                if (spline != null)
                {
                    spline.Smooth();
                }
            }
        }

        public virtual void SetMirroredMode()
        {
            if (SplineRuntimeEditor.Instance != null)
            {
                SplineBase spline = SplineRuntimeEditor.Instance.SelectedSpline;
                if (spline != null)
                {
                    spline.SetControlPointMode(ControlPointMode.Mirrored);
                }
            }
        }

     

        public virtual void SetAlignedMode()
        {
            if (SplineRuntimeEditor.Instance != null)
            {
                SplineBase spline = SplineRuntimeEditor.Instance.SelectedSpline;
                if (spline != null)
                {
                    spline.SetControlPointMode(ControlPointMode.Aligned);
                }
            }
        }

        public virtual void SetFreeMode()
        {
            if (SplineRuntimeEditor.Instance != null)
            {
                SplineBase spline = SplineRuntimeEditor.Instance.SelectedSpline;
                if (spline != null)
                {
                    spline.SetControlPointMode(ControlPointMode.Free);
                }
            }
        }


        public virtual void Load()
        {
            string dataAsString = PlayerPrefs.GetString("SplineEditorSave");
            if(string.IsNullOrEmpty(dataAsString))
            {
                return;
            }
            SplineBase[] splines = FindObjectsOfType<SplineBase>();
            SplineSnapshot[] snapshots = DeserializeFromString<SplineSnapshot[]>(dataAsString);

            //Should be replaced with more sophisticated load & save & validation logic
            if (splines.Length != snapshots.Length)
            {
                Debug.LogError("Wrong data in save file");
                return;  
                //throw new NotImplementedException("Wrong data in save file.");
            }

            for(int i = 0; i < snapshots.Length; ++i)
            {
                splines[i].Load(snapshots[i]);
            }
        }

        /// <summary>
        /// NOTE: THIS FUNCTION IS PROVIDED AS AN EXAMPLE AND DOES NOT SAVE ANY UNITY GAMEOBJECTS (ONLY SPLINE DATA).
        /// </summary>
        public virtual void Save()
        {
            SplineBase[] splines = FindObjectsOfType<SplineBase>();
            SplineSnapshot[] snapshots = new SplineSnapshot[splines.Length];
            for (int i = 0; i < snapshots.Length; ++i)
            {
                snapshots[i] = splines[i].Save();
            }
            string dataAsString = SerializeToString(snapshots);
            PlayerPrefs.SetString("SplineEditorSave", dataAsString);
        }
    

        private static TData DeserializeFromString<TData>(string settings)
        {
            byte[] b = Convert.FromBase64String(settings);
            using (var stream = new MemoryStream(b))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return (TData)formatter.Deserialize(stream);
            }
        }

        private static string SerializeToString<TData>(TData settings)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, settings);
                stream.Flush();
                stream.Position = 0;
                return Convert.ToBase64String(stream.ToArray());
            }
        }
    }

}

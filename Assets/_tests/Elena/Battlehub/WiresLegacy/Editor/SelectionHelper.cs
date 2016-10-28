using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Battlehub.WiresLegacy
{
	[InitializeOnLoad]
	public class SelectionHelper
	{
		private static List<object> m_selectedGameObjects;
		private static object m_activeObject;

			// Use this for initialization
	 	static SelectionHelper () 
		{
			m_selectedGameObjects = new List<object>();
			m_activeObject = Selection.activeObject;
			UnityEditor.EditorApplication.update += OnEditorUpdate;
		}

		public static void Clear()
		{
			m_selectedGameObjects.Clear();
		}

		public static ICollection SelectedObjects
		{
			get { return m_selectedGameObjects; }
		}

		
		private static void OnEditorUpdate()
		{
			if(m_selectedGameObjects == null)
			{
				return;
			}

			int delta = Selection.objects.Length - m_selectedGameObjects.Count;
			if(m_activeObject != Selection.activeObject && delta == 0)
			{
				m_activeObject = Selection.activeObject;
				if(!m_selectedGameObjects.Contains(m_activeObject))
				{
					m_selectedGameObjects.Clear();
				}
			}
			else if(delta > 1  && m_selectedGameObjects.Count > 0 ||  delta < -1 )
			{
				m_activeObject = Selection.activeObject;
				m_selectedGameObjects.Clear();
			}
			else if(delta == 1 || delta == -1)
			{
				if(delta < 0)
				{
					for(int i = 0; i < m_selectedGameObjects.Count; ++i)
					{
						object gameObject = m_selectedGameObjects[i];
						if(!Selection.objects.Contains(gameObject))
						{
							m_selectedGameObjects.RemoveAt(i);
							break;
						}
					}
				}
				else
				{
					for(int i = 0; i < Selection.objects.Length; ++i)
					{
						if(!m_selectedGameObjects.Contains(Selection.objects[i]))
						{
							m_selectedGameObjects.Add(Selection.objects[i]);
							break;
						}
					}
				}
			}
		}
		

	}

}


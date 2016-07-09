using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using PygmyMonkey.AdvancedBuilder.Utils;

namespace PygmyMonkey.AdvancedBuilder
{
	public class DistributionsPlatformRenderer : IDefaultRenderer
	{
		/*
		 * DistributionPlatform data
		 */
		private List<DistributionPlatform> m_distributionPlatformList;

		/*
		 * Constructor
		 */
		public DistributionsPlatformRenderer(List<DistributionPlatform> distributionPlatformList)
		{
			m_distributionPlatformList = distributionPlatformList;
		}

		/*
		 * Draw in inspector
		 */
		public void drawInspector()
		{
			GUIUtils.DrawCategoryLabel("Distribution platforms");
			foreach (DistributionPlatform distributionPlatform in m_distributionPlatformList)
			{
				EditorGUILayout.BeginHorizontal();

				distributionPlatform.isActive = EditorGUILayout.Toggle(distributionPlatform.isActive, GUILayout.Width(15f));

				GUI.backgroundColor = distributionPlatform.isNameValid() ? Color.white : Color.red;
				distributionPlatform.name = EditorGUILayout.TextField(distributionPlatform.name);
				GUI.backgroundColor = Color.white;

				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("x", GUILayout.Width(25f)))
				{
					m_distributionPlatformList.Remove(distributionPlatform);
					return;
				}
				GUI.backgroundColor = Color.white;

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.BeginVertical(LegacyGUIStyle.CenterMarginStyle);
			{
				if (GUILayout.Button("Add distribution platform"))
				{
					m_distributionPlatformList.Add(new DistributionPlatform("Distribution Platform XXX", true));
				}
			}
			EditorGUILayout.EndVertical();
		}


		/*
		 * Check for warnings and errors
		 */
		public void checkWarningsAndErrors(ErrorReporter errorReporter)
		{
			/*
			 * If multiple distribution platform share the same name
			 */
			int duplicateCount = m_distributionPlatformList.GroupBy(x => x.name).Where(x => x.Count() > 1).Count();
			if (duplicateCount != 0)
			{
				errorReporter.addError("Each distribution platform must have a distinct name.");
			}

			/*
			 * If a distribution platform name is Default
			 */
			bool hasDefault = m_distributionPlatformList.Where(x => x.name.Equals("Default")).Count() != 0;
			if (hasDefault)
			{
				errorReporter.addError("A distribution platform can't have the name 'Default'. It is reserved for Advanced Builder, sorry...");
			}

			/*
			 * If a distribution platform name is Default
			 */
			bool isNameIncorrect = m_distributionPlatformList.Where(x => !x.isNameValid()).Count() != 0;
			if (isNameIncorrect)
			{
				errorReporter.addError("A distribution platform name can only have letters, numbers and spaces.");
			}
		}
	}
}
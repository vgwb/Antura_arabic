using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using PygmyMonkey.AdvancedBuilder.Utils;

namespace PygmyMonkey.AdvancedBuilder
{
	public class ReleaseTypesRenderer : IDefaultRenderer
	{
		/*
		 * ReleaseTypes data
		 */
		private ReleaseTypes m_releaseTypes;
		
		
		/*
		 * Constructor
		 */
		public ReleaseTypesRenderer(ReleaseTypes releaseTypes)
		{
			m_releaseTypes = releaseTypes;
		}
		
		
		/*
		 * Draw in inspector
		 */
		public void drawInspector()
		{
			foreach (ReleaseType releaseType in m_releaseTypes.getReleaseTypeList())
			{
				ReleaseTypeRenderer releaseTypeRenderer = new ReleaseTypeRenderer(releaseType);
				
				if (GUIUtils.DrawHeader(releaseType.name, true, ref releaseType.isActive))
				{
					GUIUtils.BeginContents();
					releaseTypeRenderer.drawInspector();
					
					if (GUIUtils.DrawDeleteRedButton())
					{
						m_releaseTypes.getReleaseTypeList().Remove(releaseType);
						break;
					}
					GUIUtils.EndContents();
				}
			}

			GUILayout.Space(5f);

			EditorGUILayout.BeginHorizontal(LegacyGUIStyle.CenterMarginStyle);
			if (GUILayout.Button("Add Release Type"))
			{
				ReleaseType releaseType = new ReleaseType(m_releaseTypes.getReleaseTypeList().Count + 1);
				m_releaseTypes.getReleaseTypeList().Add(releaseType);
			}
			EditorGUILayout.EndHorizontal();
		}
		
		
		/*
		 * Check for warnings and errors
		 */
		public void checkWarningsAndErrors(ErrorReporter errorReporter)
		{
			foreach (ReleaseType releaseType in m_releaseTypes.getReleaseTypeList())
			{
				ReleaseTypeRenderer releaseTypeRenderer = new ReleaseTypeRenderer(releaseType);
				releaseTypeRenderer.checkWarningsAndErrors(errorReporter);
			}

			/*
			 * If no release type is added
			 */
			if (m_releaseTypes.getReleaseTypeList().Count == 0)
			{
				errorReporter.addError("You need to have at least one release type.");
			}
			else
			{
				/*
				 * If no release type is active
				 */
				if (!m_releaseTypes.getReleaseTypeList().Any(x => x.isActive))
				{
					errorReporter.addError("You need to have at least one release type selected.");
				}


				/*
				 * If multiple release type share the same name
				 */
				int duplicateCount = m_releaseTypes.getReleaseTypeList().GroupBy(x => x.name).Where(x => x.Count() > 1).Count();
				if (duplicateCount != 0)
				{
					errorReporter.addError("Each release type must have a distinct name.");
				}
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	public class ReleaseTypeRenderer : IDefaultRenderer
	{
		/*
		 * ReleaseType data
		 */
		private ReleaseType m_releaseType;
		
		
		/*
		 * Constructor
		 */
		public ReleaseTypeRenderer(ReleaseType releaseType)
		{
			m_releaseType = releaseType;
		}
		
		
		/*
		 * Draw in inspector
		 */
		public void drawInspector()
		{
			GUI.backgroundColor = isNameValid() ? Color.white : Color.red;
			m_releaseType.name = EditorGUILayout.TextField(new GUIContent("Name", "The name of the release type, only for display purpose"), m_releaseType.name);
			
			GUI.backgroundColor = isBundleIdentifierValid() ? Color.white : Color.red;
			m_releaseType.bundleIdentifier = EditorGUILayout.TextField(new GUIContent("Bundle Identifier", "The name of the bundle identifier (iOS), or package name (Android)"), m_releaseType.bundleIdentifier);

			GUI.backgroundColor = isProductNameValid() ? Color.white : Color.red;
			m_releaseType.productName = EditorGUILayout.TextField(new GUIContent("Product Name", "The name of your product, that will appear under the icon on iOS and Android"), m_releaseType.productName);

			GUI.backgroundColor = Color.white;
			
			GUILayout.Space(5f);
		}
		
		
		/*
		 * Check for warnings and errors
		 */
		public void checkWarningsAndErrors(ErrorReporter errorReporter)
		{
			if (!m_releaseType.isActive)
			{
				return;
			}

			if (!isNameValid())
			{
				errorReporter.addError("You must specify a name for each release type.");
			}
			
			if (!isBundleIdentifierValid())
			{
				errorReporter.addError("You must specify a bundleIndentifier for the release type '" + m_releaseType.name + "'.");
			}
			
			if (!isProductNameValid())
			{
				errorReporter.addError("You must specify a product name for the release type '" + m_releaseType.name + "'.");
			}
		}


		private bool isNameValid()
		{
			return !string.IsNullOrEmpty(m_releaseType.name) && Regex.IsMatch(m_releaseType.name, @"^[A-Za-z0-9 _]*[A-Za-z0-9][A-Za-z0-9 _]*$");
		}

		private bool isBundleIdentifierValid()
		{
			return !string.IsNullOrEmpty(m_releaseType.bundleIdentifier);
		}

		private bool isProductNameValid()
		{
			return !string.IsNullOrEmpty(m_releaseType.productName);
		}
	}
}
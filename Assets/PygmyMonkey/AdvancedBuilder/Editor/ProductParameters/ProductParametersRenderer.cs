using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.AdvancedBuilder
{
	public class ProductParametersRenderer : IDefaultRenderer
	{
		/*
		 * ProductParameters data
		 */
		private ProductParameters m_productParameters;
		
		
		/*
		 * Constructor
		 */
		public ProductParametersRenderer(ProductParameters productParameters)
		{
			m_productParameters = productParameters;
		}
		
		
		/*
		 * Draw in inspector
		 */
		public void drawInspector()
		{
			GUI.backgroundColor = isBundleVersionFormatValid() ? Color.white : Color.red;
			m_productParameters.bundleVersion = EditorGUILayout.TextField(new GUIContent("Bundle Version", "The version name of your product, such as: 1.3.5"), m_productParameters.bundleVersion);
			GUI.backgroundColor = Color.white;
		}


		/*
		 * Check for warnings and errors
		 */
		public void checkWarningsAndErrors(ErrorReporter errorReporter)
		{
			if (!isBundleVersionFormatValid())
			{
				errorReporter.addError("Bundle Version has bad format\nMust be xx.xx or xx.xx.xx\n(x can be 1 or 2 digits)");
			}
		}
		
		
		private bool isBundleVersionFormatValid()
		{
			return Regex.IsMatch(m_productParameters.bundleVersion, @"^[0-9]{1,2}([,.][0-9]{1,2})([,.][0-9]{1,2})([,.][0-9]{1,2})?$");
		}
	}
}
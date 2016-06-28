using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace PygmyMonkey.AdvancedBuilder
{
	public class ErrorReporter
	{
		private List<string> m_warningList = new List<string>();
		private List<string> m_errorList = new List<string>();
		
		public void addWarning(string message)
		{
			m_warningList.Add(message);
		}
		
		public void addError(string message)
		{
			m_errorList.Add(message);
		}
		
		public List<string> getWarningList()
		{
			return m_warningList;
		}
		
		public List<string> getErrorList()
		{
			return m_errorList;
		}
		
		public int getWarningCount()
		{
			return m_warningList.Count;
		}
		
		public int getErrorCount()
		{
			return m_errorList.Count;
		}
	}
}

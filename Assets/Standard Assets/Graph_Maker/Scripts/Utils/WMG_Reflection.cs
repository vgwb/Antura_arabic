using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Static utility class for using C# reflection functions, used in WMG_Data_Source.
/// </summary>
public static class WMG_Reflection {

	// Windows phone 8 platform
	#if !UNITY_EDITOR && UNITY_WINRT
	public static bool IsValueType(Type type)
	{
		return type.GetTypeInfo().IsValueType;
	}
	
	private static IEnumerable<Type> GetBaseTypes(Type type)
	{
		yield return type;
		
		var baseType = type.GetTypeInfo().BaseType;
		
		if (baseType != null)
		{
			foreach (var t in GetBaseTypes(baseType))
			{
				yield return t;
			}
		}
	}
	
	public static PropertyInfo GetProperty(Type type, string name)
	{
		return
			GetBaseTypes(type)
				.Select(baseType => baseType.GetTypeInfo().GetDeclaredProperty(name))
				.FirstOrDefault(property => property != null);
	}
	
	public static MethodInfo GetMethod(Type type, string name)
	{
		return
			GetBaseTypes(type)
				.Select(baseType => baseType.GetTypeInfo().GetDeclaredMethod(name))
				.FirstOrDefault(method => method != null);
	}
	
	public static FieldInfo GetField(Type type, string name)
	{
		return
			GetBaseTypes(type)
				.Select(baseType => baseType.GetTypeInfo().GetDeclaredField(name))
				.FirstOrDefault(field => field != null);
	}
	
	public static bool IsEnum(Type type)
	{
		return type.GetTypeInfo().IsEnum;
	}
	
	public static Delegate CreateDelegate(Type type, object target, MethodInfo method)
	{
		return method.CreateDelegate(type, target);
	}
	
	public static bool IsAssignableFrom(Type first, Type second)
	{
		return first.GetTypeInfo().IsAssignableFrom(second.GetTypeInfo());
	}
	#else
	public static bool IsValueType(Type type)
	{
		return type.IsValueType;
	}
	
	public static PropertyInfo GetProperty(Type type, string name)
	{
		return type.GetProperty(name);
	}
	
	public static MethodInfo GetMethod(Type type, string name)
	{
		return type.GetMethod(name);
	}
	
	public static bool IsEnum(Type type)
	{
		return type.IsEnum;
	}
	
	public static FieldInfo GetField(Type type, string name)
	{
		return type.GetField(name);
	}

	public static Delegate CreateDelegate(Type type, object target, MethodInfo method)
	{
		return Delegate.CreateDelegate(type, target, method);
	}
	
	public static bool IsAssignableFrom(Type first, Type second)
	{
		return first.IsAssignableFrom(second);
	}

	/*
	public static T GetCopyOf<T>(this Component comp, T other) where T : Component
	{
		Type type = comp.GetType();
		if (type != other.GetType()) return null; // type mis-match
		BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
		PropertyInfo[] pinfos = type.GetProperties(flags);
		foreach (var pinfo in pinfos) {
			if (pinfo.CanWrite) {
				try {
					pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
				}
				catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
			}
		}
		FieldInfo[] finfos = type.GetFields(flags);
		foreach (var finfo in finfos) {
			finfo.SetValue(comp, finfo.GetValue(other));
		}
		return comp as T;
	}
	*/
	#endif
}

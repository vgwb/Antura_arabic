using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// Observable collection
public class WMG_List<T> : IEnumerable<T>
{
	public List<T> list { get; private set; }
	
	#pragma warning disable 67
	// editorChanged, countChanged, oneValChanged, index
	public event Action<bool,bool,bool,int> Changed = delegate { };
	//public event Action BeforeChange = delegate { };
	#pragma warning restore 67

	/// <summary>
	/// Never call this outside variable declaration!!! Use Clear() / SetList() instead.
	/// </summary>
	public WMG_List() {
		list = new List<T>();
	}

	public void SetList(IEnumerable<T> collection) {
		//BeforeChange();
		List<T> prevList = new List<T> (list);
		list = new List<T>(collection);
		if (prevList.Count == list.Count) {
			bool aValChanged = false;
			int index = -1;
			for (int i = 0; i < prevList.Count; i++) {
				if (!prevList [i].Equals (list [i])) {
					if (aValChanged) {
						Changed (false, false, false, -1);
						return;
					}
					index = i;
					aValChanged = true;
				}
			}
			if (index != -1) {
				Changed(false, false, true, index);
			}
		} else {
			Changed (false, true, false, -1);
		}
	}

	public void SetListNoCb(IEnumerable<T> collection, ref List<T> _list) {
		list = new List<T>(collection);
		_list = new List<T>(collection);
	}

	public int Count { get {return list.Count; }}

	public IEnumerator<T> GetEnumerator() {
		return list.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return list.GetEnumerator();
	}

	public void Add(T item)
	{
		//BeforeChange();
		list.Add(item);
		Changed(false, true, false, -1);
	}
	public void AddNoCb(T item, ref List<T> _list)
	{
		list.Add(item);
		_list.Add(item);
	}
	public void Remove(T item)
	{
		//BeforeChange();
		list.Remove(item);
		Changed(false, true, false, -1);
	}
	public void RemoveAt(int index)  {
		//BeforeChange();
		list.RemoveAt(index);
		Changed(false, true, false, -1);
	}
	public void RemoveAtNoCb(int index, ref List<T> _list)  {
		list.RemoveAt(index);
		_list.RemoveAt(index);
	}
	public void AddRange(IEnumerable<T> collection)
	{
		//BeforeChange();
		list.AddRange(collection);
		Changed(false, true, false, -1);
	}
	public void RemoveRange(int index, int count)
	{
		//BeforeChange();
		list.RemoveRange(index, count);
		Changed(false, true, false, -1);
	}
	public void Clear()
	{
		//BeforeChange();
		list.Clear();
		Changed(false, true, false, -1);
	}
	public void Sort()
	{
		//BeforeChange();
		list.Sort();
		Changed(false, false, false, -1);
	}
	public void Sort(Comparison<T> comparison)
	{
		//BeforeChange();
		list.Sort(comparison);
		Changed(false, false, false, -1);
	}
	public void Insert(int index, T item)
	{
		//BeforeChange();
		list.Insert(index, item);
		Changed(false, true, false, -1);
	}
	public void InsertRange(int index, IEnumerable<T> collection)
	{
		//BeforeChange();
		list.InsertRange(index, collection);
		Changed(false, true, false, -1);
	}
	public void RemoveAll(Predicate<T> match)
	{
		//BeforeChange();
		list.RemoveAll(match);
		Changed(false, true, false, -1);
	}
	public void Reverse() {
		//BeforeChange();
		list.Reverse();
		Changed(false, false, false, -1);
	}
	public void Reverse(int index, int count) {
		//BeforeChange();
		list.Reverse(index, count);
		Changed(false, false, false, -1);
	}

	public T this[int index]
	{
		get
		{
			return list[index];
		}
		set
		{
			//BeforeChange();
			list[index] = value;
			Changed(false, false, true, index);
		}
	}

	public void SetValNoCb(int index, T val, ref List<T> _list) {
		list [index] = val;
		_list[index] = val;
	}

	public void SizeChangedViaEditor() {
		Changed(true, true, false, -1);
	}

	public void ValueChangedViaEditor(int index) {
		Changed(true, false, true, index);
	}

	public void SetListViaEditor(IEnumerable<T> collection) {
		list = new List<T>(collection);
	}
	
	public void SetValViaEditor(int index, T val) {
		list [index] = val;
	}

}
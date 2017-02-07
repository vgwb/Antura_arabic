using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IWMG_Caching_Functions {
	void updateCacheAndFlagList<T>(ref List<T> cache, List<T> val, ref bool flag);

	void updateCacheAndFlag<T>(ref T cache, T val, ref bool flag);

	void SwapVals<T>(ref T val1, ref T val2);

	void SwapValsList<T>(ref List<T> val1, ref List<T> val2);
}

public class WMG_Caching_Functions : IWMG_Caching_Functions {

	public void updateCacheAndFlag<T>(ref T cache, T val, ref bool flag) {
		if (!EqualityComparer<T>.Default.Equals(cache, val)) {
			cache = val;
			flag = true;
		}
	}

	public void updateCacheAndFlagList<T>(ref List<T> cache, List<T> val, ref bool flag) {
		if (cache.Count != val.Count) {
			cache = new List<T>(val);
			flag = true;
		}
		else {
			for (int i = 0; i < val.Count; i++) {
				if (!EqualityComparer<T>.Default.Equals(val[i], cache[i])) {
					cache = new List<T>(val);
					flag = true;
					break;
				}
			}
		}
	}

	public void SwapVals<T>(ref T val1, ref T val2) {
		T tmp = val1;
		val1 = val2;
		val2 = tmp;
	}

	public void SwapValsList<T>(ref List<T> val1, ref List<T> val2) {
		List<T> tmp = new List<T>(val1);
		val1 = val2;
		val2 = tmp;
	}
}

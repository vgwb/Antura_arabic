using UnityEngine;
using System.Collections;

public class GP_Snapshot  {
	public GP_SnapshotMeta meta;
	public byte[] bytes;
	public string stringData;


	public GP_Snapshot() {
		meta =  new GP_SnapshotMeta();
	}
}

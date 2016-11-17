using UnityEngine;
using System.Collections;

public class DancingDotsQuadManager : MonoBehaviour {

	public GameObject[] quads;
	public float minTime = 0.1f;
	public float maxTime = 0.5f;

	// Use this for initialization
	void Start () {
		StartCoroutine(CR_AnimateQuads());
	}

	void SwapQuads (GameObject Quad1, GameObject Quad2)
	{
		Vector3 temp = Quad1.transform.position;
		Quad1.transform.position = Quad2.transform.position;
		Quad2.transform.position = temp;
	}

	IEnumerator CR_AnimateQuads()
	{
		do
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(minTime, maxTime));
			int Q1 = UnityEngine.Random.Range(0,quads.Length);
			int Q2 = -1;
			do 
			{
				Q2 = UnityEngine.Random.Range(0,quads.Length);
			} while (Q1 == Q2);
			SwapQuads(quads[Q1],quads[Q2]);

		} while (true);

	}

	// Update is called once per frame
	void Update () {
	
	}
}

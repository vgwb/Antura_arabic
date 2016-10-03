using UnityEngine;
using System.Collections;

public class WrongTubes : MonoBehaviour
{
    public WrongTube[] tubes;

	void Start ()
    {
	
	}
	
	public void DropLetter (System.Action callback)
    {
        tubes[Random.Range(0, tubes.Length)].DropLetter(callback);
    }
}

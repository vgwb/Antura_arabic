using UnityEngine;

public class move_test_Script : MonoBehaviour {

	public float moveSpeed;
	// Use this for initialization
	void Start () {
		moveSpeed = 10f;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (moveSpeed * Input.GetAxis ("Horizontal") * Time.deltaTime, 0f, moveSpeed * Input.GetAxis ("Vertical") * Time.deltaTime);
		transform.Rotate (0f, moveSpeed * Input.GetAxis ("Horizontal") * Time.deltaTime*20, 0f);
	}
}

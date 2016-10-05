using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MazeCharacter: MonoBehaviour {

	//for internal use:
	private delegate void VoidDelegate();


	public List<Vector3> characterWayPoints;
	public int health = 4;
	public GameObject collider;
	public GameObject Fruits;
	public GameObject cracks;
	public bool characterIsMoving;



	int currentCharacterWayPoint;


	Vector3 initialPosition;
	Quaternion initialRotation;
	Vector3 targetPos;
	Quaternion targetRotation;
	int currentWayPoint;



	List<GameObject> lines;
	List<GameObject> _cracks;
	List<GameObject> _fruits;

	int currentFruitIndex;

	private bool startCheckingForCollision = false;
	private bool donotHandleBorderCollision = false;
	void Start()
	{
		lines = new List<GameObject>();
		characterIsMoving = false;
		characterWayPoints = new List<Vector3>();
		currentCharacterWayPoint = 0;

		initialPosition = transform.position;
		targetPos = initialPosition;

		initialRotation = transform.rotation;
		targetRotation = initialRotation;

		currentWayPoint = 0;
		collider.GetComponent<MeshRenderer> ().enabled = false;
		collider.SetActive(false);

		characterWayPoints.Add(initialPosition);

		//cracks to display:
		_cracks = new List<GameObject> ();
		cracks.SetActive (true);
		foreach (Transform child in cracks.transform) {
			child.gameObject.SetActive (false);
			_cracks.Add (child.gameObject);
		}


		//fruits to collect
		_fruits = new List<GameObject> ();
		int i = 0;
		foreach (Transform child in Fruits.transform) {
			//child.gameObject.SetActive (i==0||i==1? true:false);
			child.gameObject.name = "fruit_" + (i++);
			child.gameObject.GetComponent<BoxCollider> ().enabled = false;
			_fruits.Add (child.gameObject);

		}
		currentFruitIndex = 0;
	}

	void OnTriggerEnter(Collider other)
	{
		if (donotHandleBorderCollision)
			return;
		
		print ("Colliding with: " + other.gameObject.name);

		if (other.gameObject.name == "BorderCollider") {

			//if this is the 1st hit ignore it:
			if (!startCheckingForCollision) {
				startCheckingForCollision = true;
				return;
			}

			wasHit ();
		}

		if (other.gameObject.name.IndexOf ("fruit_") == 0) {
			//we hit a fruit make sure it is in order:
			int index = int.Parse( other.gameObject.name.Substring(6));

			if (index == currentFruitIndex) {
				_fruits [currentFruitIndex].SetActive (false);
				currentFruitIndex++;
				/*if (currentFruitIndex + 1 < _fruits.Count - 1)
					_fruits [currentFruitIndex + 1].SetActive (true);*/
			} else {
				//lose?
				waitAndRestartScene();
			}
		}
	}


	void OnTriggerExit(Collider other)
	{
		print ("trigger exit " + other.gameObject.name);

		if (other.gameObject.name == "MazeLetter") {
			//if the character completely exits the maze letter:
			//stop for a second and restart the level:
			waitAndRestartScene();
		}
	
	}

	void wasHit()
	{
		_cracks [_cracks.Count- health].SetActive (true);
		health--;


		StartCoroutine (shakeCamera (0.5f, 0.5f));

		if (health == 0) {

			waitAndRestartScene ();
			return;
		}

	
		//stop checking for border collision for half a second
		StartCoroutine(waitAndPerformCallback(0.5f,()=>{
			donotHandleBorderCollision = true;
		},
			()=>{
				donotHandleBorderCollision = false;
			}));

		//stop moving the character for a second
		StartCoroutine(waitAndPerformCallback(1,()=>{
			characterIsMoving = false;
		},
			()=>{
				characterIsMoving = true;
			}));
		

	}

	IEnumerator shakeCamera(float duration, float magnitude) {
	        
	    float elapsed = 0.0f;
	    
	    Vector3 originalCamPos = Camera.main.transform.position;
	    
	    while (elapsed < duration) {
	        
	        elapsed += Time.deltaTime;          
	        
	        float percentComplete = elapsed / duration;         
	        float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);
	        
	        // map value to [-1, 1]
	        float x = Random.value * 2.0f - 1.0f;
	        float y = Random.value * 2.0f - 1.0f;
	        x *= magnitude * damper;
	        y *= magnitude * damper;
	        
	        Camera.main.transform.position = new Vector3(x, y, originalCamPos.z);
	            
	        yield return null;
	    }
	    
	    Camera.main.transform.position = originalCamPos;
	}

	void waitAndRestartScene()
	{
		print ("lose");
		//stop for a second and restart the level:
		StartCoroutine(waitAndPerformCallback(1,()=>{
			donotHandleBorderCollision = true;
			characterIsMoving = false;
		},
			()=>{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}));


	}

	//corutine to handle pausing a bit then resuming
	IEnumerator waitAndPerformCallback(float seconds,VoidDelegate init, VoidDelegate callback)
	{
		init ();

		yield return new WaitForSeconds (seconds);

		callback ();
	}

	public bool canMouseBeDown()
	{
		if (_fruits.Count == 0)
			return false;
		
		float distance = transform.position.z - Camera.main.transform.position.z;
		Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
		pos = Camera.main.ScreenToWorldPoint(pos);

		//check distance to first fruit:
		pos.z = _fruits[0].transform.position.z;

		return ((pos - _fruits [0].transform.position).sqrMagnitude) <= 1;


	}

	public void initMovement()
	{
		characterIsMoving = true;

		foreach (GameObject fruit in _fruits) {
			fruit.GetComponent<BoxCollider> ().enabled = true;
		}
	}

	public void calculateMovementAndRotation()
	{
		//if(victory) return;

		Vector3 previousPosition = targetPos;
		float distance = transform.position.z - Camera.main.transform.position.z;
		targetPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
		targetPos = Camera.main.ScreenToWorldPoint(targetPos);

		if(previousPosition != initialPosition && previousPosition != targetPos)
			DrawLine(previousPosition, targetPos, Color.red);





		var dir = transform.position - characterWayPoints[currentCharacterWayPoint];
		var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		targetRotation =  Quaternion.AngleAxis(angle, Vector3.forward) * initialRotation;


		if(previousPosition != targetPos)
		{
			print (targetPos);
			targetPos.z = -0.2f;
			characterWayPoints.Add(targetPos);

		}	

	}

	void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.6f)
	{
		start.z = end.z = -0.5f;//-0.1f;
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Unlit/Color"));
		lr.SetColors(color, color);

		lr.SetWidth(0.3f, 0.3f);
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);

		lines.Add(myLine);
		//GameObject.Destroy(myLine, duration);
	}

	void Update()
	{
		if (characterIsMoving) {
			transform.position = Vector3.MoveTowards (transform.position, characterWayPoints[currentCharacterWayPoint], Time.deltaTime*2);
			var dir = transform.position - characterWayPoints[currentCharacterWayPoint];
			var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			targetRotation =  Quaternion.AngleAxis(angle, Vector3.forward) * initialRotation;

			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 5);


			if((transform.position - characterWayPoints[currentCharacterWayPoint]).magnitude == 0 && currentCharacterWayPoint < characterWayPoints.Count-1){

				currentCharacterWayPoint++;

				//reached the end:
				if (currentCharacterWayPoint == characterWayPoints.Count-1) {
					//arrived!
					if (currentFruitIndex == _fruits.Count) {
						print ("Won");
					} else
						print ("Lost");

					waitAndRestartScene ();
				}

				//enable collider when we reach the second waypoint
				if (currentCharacterWayPoint == 1)
					collider.SetActive (true);
			}
		}
	}
}

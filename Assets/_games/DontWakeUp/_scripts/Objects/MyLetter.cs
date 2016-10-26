using UnityEngine;
using System.Collections;
using ArabicSupport;
using TMPro;

namespace EA4S.DontWakeUp
{
    public class MyLetter : MonoBehaviour
    {

        public GameObject DrawingGO;
        public GameObject TextGO;
        TextMeshProUGUI TextWord;

        //        float SpeedLimit = 16f;

        public EA4S.SplineTrailRenderer trailReference;
        public string groundLayerName = "Terrain";
        public string playerLayerName = "Default";
        public Vector3 trailOffset = new Vector3(0, 0.02f, 0);

        private bool dragging = false;
        bool draggingStarted;
        bool overDestinationMarker;

        //        bool colliding;

        //        float startingY;
        Vector3 liftedOffset = new Vector3(0, 1.5f, 0);

        //        Vector3 lastMousePosition = Vector3.zero;

        bool inOverSpeed;


        //    void OnCollisionStay(Collision collisionInfo) {
        //        foreach (ContactPoint contact in collisionInfo.contacts) {
        //            Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
        //        }
        //    }
        //

        //        void OnTriggeEnter(Collider other) {
        //            Debug.Log("triggero WON " + other.gameObject.name);
        //            if (other.gameObject.tag == "Destination") {
        //
        //                GameDontWakeUp.Instance.Won();
        //            }
        //        }

        void Start()
        {
            TextGO.SetActive(false);
            DrawingGO.SetActive(false);
        }

        void OnDisable()
        {
            if (trailReference != null)
                trailReference.Clear();
        }

        public void Init(string wordCode)
        {
            // Debug.Log("MyLetter Init " + wordCode);
            draggingStarted = false;
            overDestinationMarker = false;
            DrawingGO.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + wordCode);
            DrawingGO.SetActive(false);
            TextGO.SetActive(true);
            TextGO.GetComponent<TextMeshPro>().text = ArabicFixer.Fix(DontWakeUpManager.Instance.currentWord.Data.Arabic, false, false);

            //startingY = transform.position.y;

            trailReference.Clear();
        }

        void LetterDropped()
        {
            DontWakeUpManager.Instance.SpeakCurrentLetter();
            if (overDestinationMarker) {
                DontWakeUpManager.Instance.RoundWon();
            } else {
                DontWakeUpManager.Instance.RoundLost(How2Die.Fall);
            }
        }


        void OnTriggerEnter(Collider other)
        {
            if (DontWakeUpManager.Instance.currentState == DontWakeUpMinigameState.Playing) {
                //Debug.Log("OnTriggerEnter " + other.gameObject.name);
                // GameDontWakeUp.Instance.dangering.InDanger(false);
                //colliding = true;
                if (other.gameObject.tag == "Alert") {
                    if (other.gameObject.name.Contains("alarm")) {
                        DontWakeUpManager.Instance.InDanger(true, How2Die.TouchedAlarm);
                    } else {
                        DontWakeUpManager.Instance.InDanger(true, How2Die.TouchedDog);
                    }
                }
                if (other.gameObject.tag == "Obstacle") {
                    DontWakeUpManager.Instance.RoundLost(How2Die.TouchedDog);
                }
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (DontWakeUpManager.Instance.currentState == DontWakeUpMinigameState.Playing) {
                //Debug.Log("OnTriggerStay " + other.gameObject.name);
                //            if (other.gameObject.tag == "Obstacle") {
                //                GameDontWakeUp.Instance.dangering.InDanger(true);
                //            }

                if (other.gameObject.tag == "Marker") {
                    if (other.gameObject.GetComponent<Marker>().Type == MarkerType.Goal) {
                        overDestinationMarker = true;
                    } else {
                        overDestinationMarker = false;
                    }
                } else {
                    overDestinationMarker = false;
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (DontWakeUpManager.Instance.currentState == DontWakeUpMinigameState.Playing) {
                //Debug.Log("OnTriggerExit " + other.gameObject.name);
                if (other.gameObject.tag == "Alert") {
                    DontWakeUpManager.Instance.InDanger(false, How2Die.Null);
                }
                if (other.gameObject.tag == "Marker") {
                    overDestinationMarker = false;
                }

                //colliding = false;
            }
        }


        //
        //    // This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)
        //    public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
        //
        //    // This stores the finger that's currently dragging this GameObject
        //    private Lean.LeanFinger draggingFinger;
        //
        //    protected virtual void OnEnable() {
        //        Lean.LeanTouch.OnFingerDown += OnFingerDown;
        //        Lean.LeanTouch.OnFingerUp += OnFingerUp;
        //    }
        //
        //    protected virtual void OnDisable() {
        //        Lean.LeanTouch.OnFingerDown -= OnFingerDown;
        //        Lean.LeanTouch.OnFingerUp -= OnFingerUp;
        //    }
        //
        //    protected virtual void LateUpdate() {
        //        if (draggingFinger != null) {
        //            Lean.LeanTouch.MoveObject(transform, draggingFinger.DeltaScreenPosition, Camera.main);
        //        }
        //    }
        //
        //    public void OnFingerDown(Lean.LeanFinger finger) {
        //        // Raycast information
        //        var ray = finger.GetRay();
        //        var hit = default(RaycastHit);
        //
        //        // Was this finger pressed down on a collider?
        //        if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true) {
        //            // Was that collider this one?
        //            if (hit.collider.gameObject == gameObject) {
        //                // Set the current finger to this one
        //                draggingFinger = finger;
        //            }
        //        }
        //    }
        //
        //    public void OnFingerUp(Lean.LeanFinger finger) {
        //        if (finger == draggingFinger) {
        //            draggingFinger = null;
        //        }
        //    }
        //



        void Update()
        {
            if (DontWakeUpManager.Instance.currentState == DontWakeUpMinigameState.Playing) {
                if (Input.GetMouseButtonDown(0)) {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

                    if (Physics.Raycast(ray, out hit, float.MaxValue, LayerNameToIntMask(playerLayerName))) {
                        dragging = true;
                        draggingStarted = true;
                        MoveOnFloor();
                        trailReference.Clear();
                        DontWakeUpManager.Instance.SpeakCurrentLetter();
                    }
                } else if (Input.GetMouseButtonUp(0)) {
                    dragging = false;
                }

                if (Input.GetMouseButton(0) && dragging) {
                    //if (!colliding) {
                    MoveOnFloor();
                    //}
                }

                if (Input.GetMouseButtonUp(0)) {
                    dragging = false;
                    if (draggingStarted) {
                        LetterDropped();
                    }
                }
            }
        }



        void MoveOnFloor()
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x,
                            Input.mousePosition.y, 0)), out hit, float.MaxValue, LayerNameToIntMask(groundLayerName))) {
                trailReference.transform.position = hit.point + trailOffset;

                transform.position = hit.point + liftedOffset + trailOffset;

                //                mouseDelta = Input.mousePosition - lastMousePosition;
                //                lastMousePosition = Input.mousePosition;

                //Debug.Log(mouseDelta.magnitude);
                //                if (mouseDelta.magnitude > SpeedLimit) {
                //                    inOverSpeed = true;
                //                    DontWakeUpManager.Instance.InDanger(true, How2Die.TooFast);
                //                } else {
                //                    if (inOverSpeed) {
                //                        inOverSpeed = false;
                //                        DontWakeUpManager.Instance.InDanger(false, How2Die.Null);
                //                    }
                //                }

            }
        }

        static int LayerNameToIntMask(string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);

            if (layer == 0)
                return int.MaxValue;

            return 1 << layer;
        }

    }

}
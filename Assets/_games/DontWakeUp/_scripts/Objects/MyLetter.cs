using UnityEngine;
using System.Collections;

namespace EA4S.DontWakeUp
{
    public class MyLetter : MonoBehaviour
    {

        public GameObject LetterDrawing;

        public EA4S.SplineTrailRenderer trailReference;
        public string groundLayerName = "Terrain";
        public string playerLayerName = "Default";
        public Vector3 trailOffset = new Vector3(0, 0.02f, 0);

        private bool dragging = false;
        bool draggingStarted;
        bool overDestinationMarker;

        bool colliding;

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

        public void Init(string wordCode) {
            // Debug.Log("MyLetter Init " + wordCode);
            draggingStarted = false;
            overDestinationMarker = false;
            LetterDrawing.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/LivingLetters/Drawings/drawing-" + wordCode);
            trailReference.Clear();
        }

        void CheckWin() {
            if (overDestinationMarker) {
                GameDontWakeUp.Instance.RoundWon();
            } else {
                GameDontWakeUp.Instance.RoundLost(How2Die.Fall);
            }
        }


        void OnTriggerEnter(Collider other) {
            //Debug.Log("OnTriggerEnter " + other.gameObject.name);
            // GameDontWakeUp.Instance.dangering.InDanger(false);
            colliding = true;
            if (other.gameObject.tag == "Obstacle") {
                GameDontWakeUp.Instance.InDanger(true);
            }

        }

        void OnTriggerStay(Collider other) {
            //Debug.Log("triggero " + other.gameObject.name);
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

        void OnTriggerExit(Collider other) {
            if (other.gameObject.tag == "Obstacle") {
                GameDontWakeUp.Instance.InDanger(false);


            }
            if (other.gameObject.tag == "Marker") {
                overDestinationMarker = false;
            }

            colliding = false;
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



        void Update() {
            if (GameDontWakeUp.Instance.currentState == MinigameState.Playing) {
                if (Input.GetMouseButtonDown(0)) {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

                    if (Physics.Raycast(ray, out hit, float.MaxValue, LayerNameToIntMask(playerLayerName))) {
                        dragging = true;
                        draggingStarted = true;
                        MoveOnFloor();
                        trailReference.Clear();
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
                        CheckWin();
                    }
                }
            }
        }



        void MoveOnFloor() {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, 
                            Input.mousePosition.y, 0)), out hit, float.MaxValue, LayerNameToIntMask(groundLayerName))) {
                trailReference.transform.position = hit.point + trailOffset;

                transform.position = hit.point + trailOffset;

            }
        }

        static int LayerNameToIntMask(string layerName) {
            int layer = LayerMask.NameToLayer(layerName);

            if (layer == 0)
                return int.MaxValue;

            return 1 << layer;
        }
    }
   
}
using UnityEngine;
using System.Collections;

namespace EA4S.MixedLetters
{
    public class AnturaController : MonoBehaviour
    {
        public static AnturaController instance;

        public BoxCollider boxCollider;

        private Vector3 velocity;
        private System.Action notifyFightBeganCallback;

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }

            boxCollider.enabled = true;
        }

        void Update()
        {
            Vector3 position = transform.position;
            position += velocity * Time.deltaTime;
            transform.position = position;

            if (IsOffScreen() && velocity.x * transform.position.x > 0)
            {
                Disable();
            }
        }

        public void EnterScene(System.Action notifyFightBeganCallback)
        {
            Vector3 victimLLPosition = VictimLLController.instance.transform.position;
            Vector3 anturaPosition = victimLLPosition;

            float frustumHeight = 2.0f * Mathf.Abs(anturaPosition.z - Camera.main.transform.position.z) * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight * Camera.main.aspect;

            anturaPosition.x = frustumWidth / 2 + 2f;

            if (victimLLPosition.x > 0)
            {
                anturaPosition.x *= -1;
            }

            velocity.x = victimLLPosition.x - anturaPosition.x;

            if (velocity.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
            }

            else
            {
                transform.rotation = Quaternion.Euler(0, 270, 0);
            }

            transform.position = anturaPosition;

            this.notifyFightBeganCallback = notifyFightBeganCallback;
        }

        public void ExitScene()
        {

        }

        private bool IsOffScreen()
        {
            float frustumHeight = 2.0f * Mathf.Abs(transform.position.z - Camera.main.transform.position.z) * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight * Camera.main.aspect;
            float halfFrustumWidth = frustumWidth / 2;

            if (Mathf.Abs(transform.position.x) - 2f > halfFrustumWidth)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Destination")
            {
                notifyFightBeganCallback.Invoke();
            }
        }

        public void SetPosition(Vector3 position)
        {
            SetPositionWithOffset(position, Vector3.zero);
        }

        public void SetPositionWithOffset(Vector3 position, Vector3 offset)
        {
            transform.position = position + offset;
        }

        public void Reset()
        {
            velocity = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
using UnityEngine;
using System.Collections;

namespace Battlehub.RTHandles
{
    [AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
    public class MouseOrbit : MonoBehaviour
    {
        public Vector3 Target;
        public float Distance = 5.0f;
        public float XSpeed = 5.0f;
        public float YSpeed = 5.0f;

        public float YMinLimit = -360f;
        public float YMaxLimit = 360f;

        public float DistanceMin = .5f;
        public float DistanceMax = 5000f;

        private float m_x = 0.0f;
        private float m_y = 0.0f;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            Vector3 angles = transform.eulerAngles;
            m_x = angles.y;
            m_y = angles.x;
        }

        private void LateUpdate()
        {
            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");

            Vector2 delta = new Vector2(deltaX, deltaY);

            deltaX = delta.x * XSpeed;
            deltaY = delta.y * YSpeed;
            
            m_x += deltaX;
            m_y -= deltaY;
            m_y = ClampAngle(m_y, YMinLimit, YMaxLimit);

            Zoom();
        }

        public void Zoom()
        {
            Quaternion rotation = Quaternion.Euler(m_y, m_x, 0);

            Distance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * Distance, DistanceMin, DistanceMax);

            //RaycastHit hit;
            //if (Physics.Linecast(Target, transform.position, out hit))
            //{
            //    Debug.Log(hit.distance);
            //    Distance -= hit.distance;
            //}
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -Distance);
            Vector3 position = rotation * negDistance + Target;

            
            transform.rotation = rotation;
            transform.position = position;
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
            return Mathf.Clamp(angle, min, max);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace EA4S
{
    [RequireComponent(typeof(Image))]
    public class SpriteLineRenderer : MonoBehaviour
    {

        public float lineWidth = 2f;

        public RectTransform imageRectTransform;
        public Color Color;
        public Vector3 startingPos;
        public bool IsInUse = false;

        Vector3 currentPos;

        void OnStart()
        {
        }

        void Update()
        {
            //currentPos = Input.mousePosition;

            //OnDraw(startingPos, new Vector3(currentPos.x, currentPos.y, 0f));
        }

        public void SetColor(Color _color)
        {
            Color = _color;
            GetComponent<Image>().color = Color;
        }

        public void OnDraw(Vector3 pointA, Vector3 pointB)
        {
            Vector3 differenceVector = pointB - pointA;

            imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, lineWidth);
            imageRectTransform.position = pointA;
            float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
            imageRectTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
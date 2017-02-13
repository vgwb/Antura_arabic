using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Class used in place of GraphicsRaycaster for generating UI mouse events that work with non-squares (using alpha pixel values of textures).
/// </summary>
public class WMG_Raycaster : GraphicRaycaster
{
	public float AlphaThreshold = .9f;

	List<RaycastResult> exclusions = new List<RaycastResult>();

	protected override void OnEnable ()
	{
		base.OnEnable();
		GraphicRaycaster defaultRaycaster = GetComponent<GraphicRaycaster>();
		if (defaultRaycaster != null && defaultRaycaster != this) {
			DestroyImmediate(defaultRaycaster);
		}
	}

	public override void Raycast (PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		base.Raycast(eventData, resultAppendList);
		exclusions.Clear();
		foreach (RaycastResult result in resultAppendList)
		{
			Image objImage = result.gameObject.GetComponent<Image>();
			if (objImage == null) continue;

			WMG_Raycatcher objAlphaCheck = result.gameObject.GetComponent<WMG_Raycatcher>();
			if (objAlphaCheck == null) continue;

			try
			{
				RectTransform objTrans = result.gameObject.transform as RectTransform;

				// evaluating pointer position relative to object local space
				Vector3 pointerGPos;
				if (eventCamera)
				{
					var objPlane = new Plane(objTrans.forward, objTrans.position);
					float distance;
					var cameraRay = eventCamera.ScreenPointToRay(eventData.position);
					objPlane.Raycast(cameraRay, out distance);
					pointerGPos = cameraRay.GetPoint(distance);
				}
				else
				{
					pointerGPos = eventData.position;
					float rotationCorrection = (-objTrans.forward.x * (pointerGPos.x - objTrans.position.x) - objTrans.forward.y * (pointerGPos.y - objTrans.position.y)) / objTrans.forward.z;
					pointerGPos += new Vector3(0, 0, objTrans.position.z + rotationCorrection);
				}
				Vector3 pointerLPos = objTrans.InverseTransformPoint(pointerGPos);

				var objTex = objImage.mainTexture as Texture2D;
				var texRect = objImage.sprite.textureRect;
				float texCorX = pointerLPos.x * (texRect.width / objTrans.sizeDelta.x) + texRect.width * objTrans.pivot.x;
				float texCorY = pointerLPos.y * (texRect.height / objTrans.sizeDelta.y) + texRect.height * objTrans.pivot.y;

				float alpha = objTex.GetPixel((int)(texCorX + texRect.x), (int)(texCorY + texRect.y)).a;

				if (objImage.type == Image.Type.Filled) {
					float angle = Mathf.Atan2(texCorY/texRect.height-0.5f, texCorX/texRect.width-0.5f) * Mathf.Rad2Deg;
					if (angle < 0) angle += 360;
					bool masked = true;
					if (objImage.fillMethod == Image.FillMethod.Radial360) {
						float angleFill = objImage.fillAmount * 360;
						if (objImage.fillOrigin == (int)Image.Origin360.Top) {
							angle -= 90;
						}
						else if (objImage.fillOrigin == (int)Image.Origin360.Left) {
							angle -= 180;
						}
						else if (objImage.fillOrigin == (int)Image.Origin360.Bottom) {
							angle -= 270;
						}
						if (angle < 0) angle += 360;
						if (objImage.fillClockwise) {
							if (angle > -angleFill+360) masked = false;
						}
						else {
							if (angle < angleFill) masked = false;
						}
					}
					if (masked) alpha = 0;
				}

				// exclude based on alpha
				if (objAlphaCheck.IncludeMaterialAlpha) alpha *= objImage.color.a;
				if (alpha < AlphaThreshold) exclusions.Add(result);
			}
			catch (UnityException e)
			{
				if (Application.isEditor) {
					Debug.LogWarning(string.Format("Alpha check failed: {0}", e.Message));
				}
			};
		}

		resultAppendList.RemoveAll(res => exclusions.Contains(res));
	}
}

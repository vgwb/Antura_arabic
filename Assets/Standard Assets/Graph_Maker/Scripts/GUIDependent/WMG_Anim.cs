using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;

/// <summary>
/// A static utility class for performing animations.
/// </summary>
public static class WMG_Anim {

	public static void animFill(GameObject obj, float duration, Ease easeType, float animTo) {
		Image comp = obj.GetComponent<Image>();
		DOTween.To(()=> comp.fillAmount, x=> comp.fillAmount = x, animTo, duration).SetEase(easeType).SetUpdate(false);
	}

	public static void animColor(GameObject obj, float duration, Ease easeType, Color animTo) {
		Graphic comp = obj.GetComponent<Graphic>();
		DOTween.To(()=> comp.color, x=> comp.color = x, animTo, duration).SetEase(easeType).SetUpdate(false);
	}

	public static void animRotation(GameObject obj, float duration, Ease easeType, Vector3 animTo, bool relative) {
		obj.transform.DOLocalRotate(animTo, duration, RotateMode.FastBeyond360).SetEase(easeType).SetUpdate(false).SetRelative(relative);
	}

	public static void animRotationCallbackC(GameObject obj, float duration, Ease easeType, Vector3 animTo, bool relative, TweenCallback onComp) {
		obj.transform.DOLocalRotate(animTo, duration, RotateMode.FastBeyond360).SetEase(easeType).SetUpdate(false).SetRelative(relative)
			.OnComplete(onComp);
	}

	public static void animRotationCallbackU(GameObject obj, float duration, Ease easeType, Vector3 animTo, bool relative, TweenCallback onUpd) {
		obj.transform.DOLocalRotate(animTo, duration, RotateMode.FastBeyond360).SetEase(easeType).SetUpdate(false).SetRelative(relative)
			.OnUpdate(onUpd);
	}

	public static void animRotationCallbacks(GameObject obj, float duration, Ease easeType, Vector3 animTo, bool relative, TweenCallback onUpd, TweenCallback onComp) {
		obj.transform.DOLocalRotate(animTo, duration, RotateMode.FastBeyond360).SetEase(easeType).SetUpdate(false).SetRelative(relative)
			.OnUpdate(onUpd).OnComplete(onComp);
	}

	public static void animPositionCallbackC(GameObject obj, float duration, Ease easeType, Vector3 animTo, TweenCallback onComp) { 
		DOTween.To(()=> obj.transform.localPosition, x=> obj.transform.localPosition = x, animTo, duration).SetEase(easeType).SetUpdate(false)
			.OnComplete(onComp);
	}

	public static void animPosition(GameObject obj, float duration, Ease easeType, Vector3 animTo) {
		DOTween.To(()=> obj.transform.localPosition, x=> obj.transform.localPosition = x, animTo, duration).SetEase(easeType).SetUpdate(false);
	}

	public static void animSize(GameObject obj, float duration, Ease easeType, Vector2 animTo) {
		RectTransform comp = obj.GetComponent<RectTransform>();
		DOTween.To(()=> comp.sizeDelta, x=> comp.sizeDelta = x, animTo, duration).SetEase(easeType).SetUpdate(false);
	}

	public static void animPositionCallbacks(GameObject obj, float duration, Ease easeType, Vector3 animTo, TweenCallback onUpd, TweenCallback onComp, string tid) {
		DOTween.To(()=> obj.transform.localPosition, x=> obj.transform.localPosition = x, animTo, duration).SetEase(easeType).SetUpdate(false)
			.OnUpdate(onUpd).OnComplete(onComp).SetId(tid);
	}

	public static void animScale(GameObject obj, float duration, Ease easeType, Vector3 animTo, float delay) {
		DOTween.To(()=> obj.transform.localScale, x=> obj.transform.localScale = x, animTo, duration).SetEase(easeType).SetUpdate(false).SetDelay(delay);
	}

	public static void animScaleCallbackC(GameObject obj, float duration, Ease easeType, Vector3 animTo, TweenCallback onComp) {
		DOTween.To(()=> obj.transform.localScale, x=> obj.transform.localScale = x, animTo, duration).SetEase(easeType).SetUpdate(false)
			.OnComplete(onComp);
	}

	public static void animScaleSeqInsert(ref Sequence seq, float insTime, GameObject obj, float duration, Ease easeType, Vector3 animTo, float delay) {
		seq.Insert(insTime,
		           DOTween.To(()=> obj.transform.localScale, x=> obj.transform.localScale = x, animTo, duration).SetEase(easeType).SetUpdate(false).SetDelay(delay)
		           );
	}

	public static void animScaleSeqAppend(ref Sequence seq, GameObject obj, float duration, Ease easeType, Vector3 animTo, float delay) {
		seq.Append(
			DOTween.To(()=> obj.transform.localScale, x=> obj.transform.localScale = x, animTo, duration).SetEase(easeType).SetUpdate(false).SetDelay(delay)
		           );
	}

	public static void animInt(DOGetter<int> getter, DOSetter<int> setter, float duration, int animTo) {
		DOTween.To(getter, setter, animTo, duration).SetUpdate(false);
	}

	public static void animFloat(DOGetter<float> getter, DOSetter<float> setter, float duration, float animTo) {
		DOTween.To(getter, setter, animTo, duration).SetUpdate(false);
	}

	public static void animFloatCallbackU(DOGetter<float> getter, DOSetter<float> setter, float duration, float animTo, TweenCallback onUpd, Ease easeType = Ease.Linear) {
		DOTween.To(getter, setter, animTo, duration).SetEase(easeType).SetUpdate(false).OnUpdate(onUpd);
	}

	public static void animFloatCallbacks(DOGetter<float> getter, DOSetter<float> setter, float duration, float animTo, TweenCallback onUpd, TweenCallback onComp, Ease easeType = Ease.Linear) {
		DOTween.To(getter, setter, animTo, duration).SetEase(easeType).SetUpdate(false).OnUpdate(onUpd).OnComplete(onComp);
	}

	public static void animVec2(DOGetter<Vector2> getter, DOSetter<Vector2> setter, float duration, Vector2 animTo, Ease easeType = Ease.Linear) {
		DOTween.To(getter, setter, animTo, duration).SetEase(easeType).SetUpdate(false);
	}

	public static void animVec2CallbackU(DOGetter<Vector2> getter, DOSetter<Vector2> setter, float duration, Vector2 animTo, TweenCallback onUpd, Ease easeType = Ease.Linear) {
		DOTween.To(getter, setter, animTo, duration).SetEase(easeType).SetUpdate(false).OnUpdate(onUpd);
	}
}

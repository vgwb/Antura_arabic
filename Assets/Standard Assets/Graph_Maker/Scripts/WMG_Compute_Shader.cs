using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent (typeof (RawImage))]
public class WMG_Compute_Shader : MonoBehaviour {

	public ComputeShader computeShader;
	public float[] pointVals = new float[4000];

	int kernelHandle;
	RenderTexture renderTexture;
	int texSize = 512;
	RawImage rawImg;
	bool hasInit = false;

	public void Init() {
		if (hasInit) return;
		hasInit = true;
		kernelHandle = computeShader.FindKernel ("CSMain");
		rawImg = this.gameObject.GetComponent<RawImage>();
		renderTexture = new RenderTexture (texSize, texSize, 24);
		renderTexture.enableRandomWrite = true;
		renderTexture.Create ();
	}

	void Start() {
		Init();
	}

	public void dispatchAndUpdateImage() {
		computeShader.SetInt("texSize", texSize);
		computeShader.SetTexture (kernelHandle, "Result", renderTexture);
		computeShader.Dispatch (kernelHandle, texSize / 8, texSize / 8, 1);
		rawImg.texture = (Texture)renderTexture;
	}
}

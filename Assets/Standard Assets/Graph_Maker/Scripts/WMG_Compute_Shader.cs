using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent (typeof (RawImage))]
/// <summary>
/// Class used to represent a texture that is procedurally generated using a Compute Shader.
/// </summary>
public class WMG_Compute_Shader : MonoBehaviour {

	public ComputeShader computeShader;

	int texSize;
	int kernelHandle;
	RenderTexture renderTexture;
	RawImage rawImg;
	bool hasInit = false;

	/// <summary>
	/// Initializes by creating a render texture with the specified resolution.
	/// </summary>
	/// <param name="textureResolution">Texture resolution.</param>
	public void Init(int textureResolution) {
		if (hasInit) return;
		texSize = textureResolution;
		hasInit = true;
		kernelHandle = computeShader.FindKernel ("CSMain");
		rawImg = this.gameObject.GetComponent<RawImage>();
		renderTexture = new RenderTexture (texSize, texSize, 24);
		renderTexture.enableRandomWrite = true;
		renderTexture.Create ();
	}

	/// <summary>
	/// Runs the compute shader and updates the texture.
	/// </summary>
	public void dispatchAndUpdateImage() {
		computeShader.SetInt("texSize", texSize);
		computeShader.SetTexture (kernelHandle, "Result", renderTexture);
		computeShader.Dispatch (kernelHandle, texSize / 8, texSize / 8, 1);
		rawImg.texture = (Texture)renderTexture;
	}

	void OnDisable() {
		renderTexture.Release ();
		DestroyImmediate (renderTexture);
	}
}
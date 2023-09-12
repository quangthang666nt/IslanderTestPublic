using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class CameraRenderTextureController : MonoBehaviour
{
	public Camera m_Camera;

	public bool m_UseDynamicScale;

	public RenderTexture RenderTexture { get; private set; }

	private void Start()
	{
		CreateRenderTexture();
	}

	private void CreateRenderTexture()
	{
		DestroyRenderTexture();
		if (m_Camera != null)
		{
			RenderTexture = new RenderTexture(Screen.width, Screen.height, 24, m_Camera.allowHDR ? DefaultFormat.HDR : DefaultFormat.LDR);
			RenderTexture.useDynamicScale = m_UseDynamicScale;
			RenderTexture.Create();
			m_Camera.targetTexture = RenderTexture;
		}
	}

	private void Update()
	{
		if (RenderTexture == null || Screen.width != RenderTexture.width || Screen.height != RenderTexture.height)
		{
			CreateRenderTexture();
		}
	}

	private void OnDestroy()
	{
		DestroyRenderTexture();
	}

	private void DestroyRenderTexture()
	{
		if (RenderTexture != null)
		{
			RenderTexture.Release();
			Object.Destroy(RenderTexture);
		}
		RenderTexture = null;
	}
}

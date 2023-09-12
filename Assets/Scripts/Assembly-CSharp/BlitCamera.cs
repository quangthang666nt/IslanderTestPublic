using UnityEngine;
using UnityEngine.Rendering;

public class BlitCamera : MonoBehaviour
{
	public Camera m_MainCamera;

	public CameraRenderTextureController m_MainCameraTexture;

	public CameraRenderTextureController m_CameraToBlitIn;

	public string m_BufferName = "Blit UI";

	public Material m_AlphaBlitMaterial;

	private CommandBuffer m_CommandBuffer;

	private void Start()
	{
		m_CommandBuffer = new CommandBuffer();
		m_CommandBuffer.name = m_BufferName;
		SetupBuffer();
		m_MainCamera.AddCommandBuffer(CameraEvent.AfterEverything, m_CommandBuffer);
	}

	private void SetupBuffer()
	{
		m_CommandBuffer.Clear();
		m_CommandBuffer.SetGlobalTexture("_TempBlit", m_MainCameraTexture.RenderTexture);
		m_CommandBuffer.Blit(m_CameraToBlitIn.RenderTexture, BuiltinRenderTextureType.CameraTarget, m_AlphaBlitMaterial);
	}

	private void Update()
	{
		SetupBuffer();
	}
}

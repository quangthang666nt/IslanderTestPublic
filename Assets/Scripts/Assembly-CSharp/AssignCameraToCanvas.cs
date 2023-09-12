using UnityEngine;

public class AssignCameraToCanvas : MonoBehaviour
{
	public Canvas m_Canvas;

	public float m_PlaneDistance = 10f;

	public Camera m_Camera;

	private void Start()
	{
		if (m_Canvas != null && m_Camera != null)
		{
			m_Canvas.renderMode = RenderMode.ScreenSpaceCamera;
			m_Canvas.planeDistance = m_PlaneDistance;
			m_Canvas.worldCamera = m_Camera;
		}
	}
}

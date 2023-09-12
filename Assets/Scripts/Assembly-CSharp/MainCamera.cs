using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
	private static Camera cam;

	public Camera m_FullResCamera;

	public static Camera FullResCamera { get; private set; }

	public static Camera Cam => cam;

	private void Awake()
	{
		cam = GetComponent<Camera>();
		FullResCamera = m_FullResCamera;
	}

	private void LateUpdate()
	{
		if (Cam != null && FullResCamera != null)
		{
			FullResCamera.fieldOfView = Cam.fieldOfView;
		}
	}
}

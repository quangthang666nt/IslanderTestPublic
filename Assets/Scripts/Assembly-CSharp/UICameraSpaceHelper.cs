using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class UICameraSpaceHelper : MonoBehaviour
{
	private static float planeDistance;

	private static Canvas cnvs;

	private static RectTransform transCnvs;

	private static Camera cam;

	public Camera screenSpaceCamera;

	public static float PlaneDistance => planeDistance;

	public static RectTransform TransCnvs => transCnvs;

	public static Camera Cam => cam;

	private void Awake()
	{
		cnvs = GetComponent<Canvas>();
		transCnvs = GetComponent<RectTransform>();
		planeDistance = cnvs.planeDistance;
		cam = screenSpaceCamera;
	}

	public static Vector3 ScreenPointToCanvasPoint(Vector3 _position)
	{
		_position.z = planeDistance;
		return cam.ScreenToWorldPoint(_position);
	}

	public static Vector3 CanvasPointToScreenPoint(Vector3 _position)
	{
		return cam.WorldToScreenPoint(_position);
	}

	public static Vector3 ScreenPointToCanvasPointUnscaledZ(Vector3 _position)
	{
		return cam.ScreenToWorldPoint(_position);
	}
}

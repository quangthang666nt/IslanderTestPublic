using UnityEngine;

[ExecuteInEditMode]
public class FaceCamera : MonoBehaviour
{
	private enum FaceMode
	{
		forward = 0,
		up = 1,
		right = 2
	}

	[SerializeField]
	[Range(0f, 1f)]
	private float fBlendFactor = 1f;

	[SerializeField]
	private bool bDontTilt;

	[SerializeField]
	private FaceMode faceMode = FaceMode.up;

	private Vector3 v3OriginalUp;

	private void Start()
	{
		switch (faceMode)
		{
		case FaceMode.up:
			v3OriginalUp = base.transform.up;
			break;
		case FaceMode.forward:
			v3OriginalUp = base.transform.forward;
			break;
		case FaceMode.right:
			v3OriginalUp = base.transform.right;
			break;
		}
	}

	private void Update()
	{
		Vector3 position = Camera.main.transform.position;
		if (bDontTilt)
		{
			position.y = base.transform.position.y;
		}
		switch (faceMode)
		{
		case FaceMode.up:
			base.transform.up = Vector3.Lerp(v3OriginalUp, position - base.transform.position, fBlendFactor);
			break;
		case FaceMode.forward:
			base.transform.forward = Vector3.Lerp(v3OriginalUp, position - base.transform.position, fBlendFactor);
			break;
		case FaceMode.right:
			base.transform.right = Vector3.Lerp(v3OriginalUp, position - base.transform.position, fBlendFactor);
			break;
		}
	}
}

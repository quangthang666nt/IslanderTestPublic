using UnityEngine;

public class MoveWithCursor : MonoBehaviour
{
	[SerializeField]
	private float fScaleOnClick = 1.2f;

	[SerializeField]
	private float fAnimationTime = 0.2f;

	private Transform tThisTransform;

	private float fAnimationTimer = 1f;

	private float fCurrAnimTime;

	private Vector3 v3OriginalScale;

	private void Start()
	{
		tThisTransform = base.transform;
		v3OriginalScale = tThisTransform.localScale;
	}

	private void Update()
	{
		tThisTransform.position = InputManager.Singleton.InputDataCurrent.v3PointerScreenPos;
		if (Input.GetMouseButton(0))
		{
			fAnimationTimer = 0f;
		}
		if (fAnimationTimer < 1f)
		{
			fCurrAnimTime = fAnimationTimer / fAnimationTime;
			tThisTransform.localScale = Vector3.Lerp(v3OriginalScale * fScaleOnClick, v3OriginalScale, fCurrAnimTime);
		}
		if (fAnimationTimer >= 1f)
		{
			tThisTransform.localScale = v3OriginalScale;
		}
		fAnimationTimer += Time.deltaTime;
	}
}

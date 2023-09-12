using UnityEngine;

public class HoverAnimation : MonoBehaviour
{
	[SerializeField]
	private float fMaxHeightOffset = 1f;

	[SerializeField]
	private float fAnimSpeed = 1f;

	[SerializeField]
	private Vector3 v3StartPos = Vector3.zero;

	private float fAnimationOffset;

	private void Start()
	{
		fAnimationOffset = Random.value;
	}

	private void Update()
	{
		float num = Mathf.Sin((Time.time + fAnimationOffset) * fAnimSpeed) * fMaxHeightOffset;
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, v3StartPos.y + num, base.transform.localPosition.z);
	}
}

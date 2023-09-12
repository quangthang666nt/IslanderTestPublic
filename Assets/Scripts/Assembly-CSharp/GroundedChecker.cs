using UnityEngine;

public class GroundedChecker : MonoBehaviour
{
	public float fScanRange = 0.05f;

	public float fScanOffset = 0.025f;

	public virtual bool BCheckGrounded(LayerMask _lmGrounded)
	{
		return FCheckInside(_lmGrounded) >= 0f;
	}

	public virtual bool BCheckGrounded(LayerMask _lmGrounded, LayerMask _lmGroundedBreaker)
	{
		float num = FCheckInside(_lmGrounded);
		if (num < 0f)
		{
			return false;
		}
		float num2 = FCheckInside(_lmGroundedBreaker);
		if (num2 < 0f)
		{
			return true;
		}
		if (num2 < num)
		{
			return false;
		}
		return true;
	}

	protected float FCheckInside(LayerMask _lm)
	{
		if (Physics.Raycast(base.transform.position + Vector3.up * fScanOffset, Vector3.down, out var hitInfo, 10000000f, _lm))
		{
			if (hitInfo.distance < fScanRange)
			{
				return hitInfo.distance;
			}
			if (Vector3.Angle(hitInfo.normal, Vector3.down) < 90f)
			{
				return 0f;
			}
		}
		return -1f;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(base.transform.position, 0.01f);
		Gizmos.DrawCube(base.transform.position + Vector3.up * fScanOffset + Vector3.down * fScanRange * 0.5f, new Vector3(0.005f, fScanRange, 0.005f));
	}
}

using UnityEngine;

public class AntiGroundedChecker : GroundedChecker
{
	public override bool BCheckGrounded(LayerMask _lmGrounded)
	{
		return !(FCheckInside(_lmGrounded) >= 0f);
	}

	public override bool BCheckGrounded(LayerMask _lmGrounded, LayerMask _lmGroundedBreaker)
	{
		float num = FCheckInside(_lmGrounded);
		if (num < 0f)
		{
			return true;
		}
		float num2 = FCheckInside(_lmGroundedBreaker);
		if (num2 < 0f)
		{
			return false;
		}
		if (num2 < num)
		{
			return true;
		}
		return false;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position, 0.01f);
		Gizmos.DrawCube(base.transform.position + Vector3.up * fScanOffset + Vector3.down * fScanRange * 0.5f, new Vector3(0.005f, fScanRange, 0.005f));
	}
}

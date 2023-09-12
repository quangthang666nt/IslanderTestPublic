using UnityEngine;

[ExecuteInEditMode]
public class RotateTowards : MonoBehaviour
{
	[SerializeField]
	private GameObject goRotateTowards;

	[SerializeField]
	private bool bRotateTowardsMainCam;

	[SerializeField]
	private Vector3 v3Up;

	private Transform trans;

	private void Start()
	{
		trans = base.transform;
	}

	private void Update()
	{
		Vector3 zero = Vector3.zero;
		if (!bRotateTowardsMainCam)
		{
			zero = goRotateTowards.transform.position - trans.position;
			trans.rotation = Quaternion.LookRotation(zero, v3Up);
		}
		else
		{
			zero = Camera.main.transform.position - trans.position;
			trans.rotation = Quaternion.LookRotation(zero, v3Up);
		}
	}
}

using UnityEngine;

public class UiSimpleBillboard : MonoBehaviour
{
	private Transform transTarget;

	private Transform trans;

	private void Start()
	{
		trans = base.transform;
		transTarget = MainCamera.Cam.transform;
	}

	private void Update()
	{
		trans.LookAt(transTarget);
	}
}

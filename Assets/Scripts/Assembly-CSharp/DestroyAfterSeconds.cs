using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
	[SerializeField]
	private float fLifeTime = 5f;

	[SerializeField]
	private GameObject goOverrideDestroyThis;

	private void Start()
	{
		if ((bool)goOverrideDestroyThis)
		{
			Object.Destroy(goOverrideDestroyThis, fLifeTime);
		}
		else
		{
			Object.Destroy(base.gameObject, fLifeTime);
		}
	}
}

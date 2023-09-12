using UnityEngine;

public class ActiveOnPlatform : MonoBehaviour
{
	[SerializeField]
	private bool shouldDestroy;

	[SerializeField]
	private bool activeOnPC;

	[SerializeField]
	private bool activeOnXboxOne;

	[SerializeField]
	private bool activeOnXboxSeriesX;

	[SerializeField]
	private bool activeOnPS4;

	[SerializeField]
	private bool activeOnPS5;

	[SerializeField]
	private bool activeOnSwitch;

	private void Awake()
	{
		if (!activeOnPC)
		{
			base.gameObject.SetActive(value: false);
			if (shouldDestroy)
			{
				Object.DestroyImmediate(base.gameObject);
			}
		}
	}
}

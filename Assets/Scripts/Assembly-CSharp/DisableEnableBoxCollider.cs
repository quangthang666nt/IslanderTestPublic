using UnityEngine;

public class DisableEnableBoxCollider : MonoBehaviour
{
	private void Update()
	{
		BoxCollider component = GetComponent<BoxCollider>();
		if ((bool)component)
		{
			component.enabled = false;
			component.enabled = true;
		}
		Collider component2 = GetComponent<Collider>();
		if ((bool)component2)
		{
			component2.enabled = false;
			component2.enabled = true;
		}
		base.enabled = false;
	}
}

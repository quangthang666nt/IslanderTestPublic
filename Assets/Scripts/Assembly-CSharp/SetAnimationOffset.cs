using UnityEngine;

public class SetAnimationOffset : MonoBehaviour
{
	private void Start()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		if ((bool)component)
		{
			component.material.SetFloat("_AnimOffset", Random.Range(0f, 1000f));
		}
		base.enabled = false;
	}
}

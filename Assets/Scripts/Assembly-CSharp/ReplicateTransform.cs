using UnityEngine;

[ExecuteInEditMode]
public class ReplicateTransform : MonoBehaviour
{
	public Transform transformToReplicate;

	private void Update()
	{
		if (!(transformToReplicate == null))
		{
			base.transform.position = transformToReplicate.position;
			base.transform.rotation = transformToReplicate.rotation;
			base.transform.localScale = transformToReplicate.localScale;
		}
	}
}

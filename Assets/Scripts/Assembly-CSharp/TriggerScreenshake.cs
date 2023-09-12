using UnityEngine;

public class TriggerScreenshake : MonoBehaviour
{
	[Range(0f, 2f)]
	public float fStrength;

	private void Start()
	{
		ScreenshakeManager.Shake(fStrength);
	}
}

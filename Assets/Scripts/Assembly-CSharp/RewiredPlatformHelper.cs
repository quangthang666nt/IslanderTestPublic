using UnityEngine;

public class RewiredPlatformHelper : MonoBehaviour
{
	[Header("Only PS4")]
	[SerializeField]
	private bool allowMouseInput;

	[SerializeField]
	private bool allowMouseInputIfTouchSupported;

	[SerializeField]
	private bool allowTouchInput;

	private void Awake()
	{
	}
}

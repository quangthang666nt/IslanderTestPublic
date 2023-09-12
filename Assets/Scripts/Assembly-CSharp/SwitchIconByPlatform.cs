using UnityEngine;
using UnityEngine.UI;

public class SwitchIconByPlatform : MonoBehaviour
{
	[SerializeField]
	private Sprite PCIconOverride;

	[SerializeField]
	private Sprite SwitchIconOverride;

	[SerializeField]
	private Sprite XboxOneIconOverride;

	[SerializeField]
	private Sprite PS4IconOverride;

	[SerializeField]
	private Image imageToOverride;

	private void Start()
	{
		imageToOverride.sprite = PCIconOverride;
	}
}

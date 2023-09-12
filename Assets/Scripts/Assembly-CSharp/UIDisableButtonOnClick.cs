using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIDisableButtonOnClick : MonoBehaviour
{
	[SerializeField]
	private float disableTime = 1f;

	private Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(Disable);
	}

	private void OnEnable()
	{
		button.enabled = true;
	}

	private void OnDisable()
	{
		button.enabled = false;
	}

	private void Disable()
	{
		StartCoroutine(DisableRoutine());
	}

	private IEnumerator DisableRoutine()
	{
		button.enabled = false;
		yield return new WaitForSeconds(disableTime);
		button.enabled = true;
	}
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationQuitter : MonoBehaviour
{
	[Header("Attributes")]
	[SerializeField]
	private float gamepadQuitVelocity = 0.5f;

	[SerializeField]
	private AnimationCurve gamepadQuitVelocityCurve;

	[SerializeField]
	private float quitDelay = 0.1f;

	[SerializeField]
	private bool saveBeforeQuit = true;

	[Header("References")]
	[SerializeField]
	private Image inputIconOutline;

	private bool active;

	private float amount;

	private void Update()
	{
		if (active)
		{
			if (InputManager.Singleton.InputDataCurrent.bUIExitUp)
			{
				active = false;
				inputIconOutline.gameObject.SetActive(value: false);
				return;
			}
			amount = Mathf.Clamp01(amount + gamepadQuitVelocity * Time.deltaTime);
			inputIconOutline.fillAmount = Mathf.InverseLerp(0f, 1f, gamepadQuitVelocityCurve.Evaluate(amount));
			if (inputIconOutline.fillAmount == 1f)
			{
				if (AudioManager.singleton != null)
				{
					AudioManager.singleton.PlayMenuClick();
				}
				Quit();
			}
		}
		else if (InputManager.Singleton.InputDataCurrent.bUIExit)
		{
			active = true;
			amount = 0f;
			inputIconOutline.gameObject.SetActive(value: true);
			inputIconOutline.fillAmount = 0f;
		}
	}

	private void OnDisable()
	{
		active = false;
		inputIconOutline.gameObject.SetActive(value: false);
	}

	public void Quit()
	{
		StartCoroutine(QuitRoutine());
	}

	private IEnumerator QuitRoutine()
	{
		yield return new WaitForSeconds(quitDelay);
		if (saveBeforeQuit)
		{
			SaveLoadManager.PerformAutosave(force: true);
			while (SaveLoadManager.singleton.m_IsSaving)
			{
				Debug.Log("Waiting for Save before Quiting");
				yield return null;
			}
			Debug.Log("Saved!");
		}
		Application.Quit();
	}
}

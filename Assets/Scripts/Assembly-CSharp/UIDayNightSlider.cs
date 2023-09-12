using SCS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIDayNightSlider : MonoBehaviour
{
	[SerializeField]
	private Slider sunSlider;

	[SerializeField]
	private Slider moonSlider;

	[SerializeField]
	private Slider handlerSlider;

	[SerializeField]
	private Toggle pauseDNCycleToggle;

	private void OnEnable()
	{
		DayNightCycle instance = DayNightCycle.Instance;
		if (!(instance == null))
		{
			SetConstValues(instance.GetStartDay(), instance.GetStartNight());
			handlerSlider?.onValueChanged.AddListener(OnChangedTime);
			if (pauseDNCycleToggle != null)
			{
				pauseDNCycleToggle.isOn = instance.IsUpdatingCycle;
				pauseDNCycleToggle?.onValueChanged.AddListener(OnPauseTime);
			}
		}
	}

	private void OnDisable()
	{
		handlerSlider?.onValueChanged.RemoveListener(OnChangedTime);
	}

	public void ToggleCycle()
	{
		pauseDNCycleToggle.isOn = !pauseDNCycleToggle.isOn;
	}

	private void OnPauseTime(bool isActive)
	{
		DayNightCycle.Instance.SetCycleActive(isActive);
	}

	private void OnChangedTime(float time)
	{
		DayNightCycle instance = DayNightCycle.Instance;
		if (time - instance.NormalizedTime != 0f)
		{
			instance.ManualUpdateCycle(time);
			if (pauseDNCycleToggle != null && pauseDNCycleToggle.isOn)
			{
				pauseDNCycleToggle.isOn = false;
			}
		}
	}

	private void SetConstValues(float sunValue, float moonValue)
	{
		sunSlider.value = sunValue;
		moonSlider.value = moonValue;
	}

	private void UpdateHandler(float val)
	{
		handlerSlider.value = val;
	}

	private void Update()
	{
		UpdateHandler(DayNightCycle.Instance.NormalizedTime);
	}
}

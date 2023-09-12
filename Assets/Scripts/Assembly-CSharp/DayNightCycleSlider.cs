using SCS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycleSlider : MonoBehaviour
{
	private Slider slider;

	private void Start()
	{
		slider = GetComponent<Slider>();
		slider.onValueChanged.AddListener(UpdatePercentDayNightCycle);
	}

	private void UpdatePercentDayNightCycle(float cycle)
	{
		DayNightCycle.Instance.ManualUpdateCycle(cycle);
	}
}

using UnityEngine;

namespace SCS.Gameplay
{
	public class DNObjectExclusive : MonoBehaviour
	{
		[SerializeField]
		private DNCycleParameters.EDayState activeDayState;

		public void SetActiveDayState(DNCycleParameters.EDayState newDayState)
		{
			activeDayState = newDayState;
			ForceUpdateToCurrentDayState();
		}

		private void Awake()
		{
			DayNightCycle.Instance.AddListenerEvent(DayCallback, NightCallback);
			ForceUpdateToCurrentDayState();
		}

		private void ForceUpdateToCurrentDayState()
		{
			if (DayNightCycle.Instance.DayState == DNCycleParameters.EDayState.Day || DayNightCycle.Instance.DayState == DNCycleParameters.EDayState.Sunrise)
			{
				DayCallback();
			}
			else
			{
				NightCallback();
			}
		}

		private void DayCallback()
		{
			if (activeDayState == DNCycleParameters.EDayState.Day || activeDayState == DNCycleParameters.EDayState.Sunrise)
			{
				base.gameObject.SetActive(value: true);
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}

		private void NightCallback()
		{
			if (activeDayState == DNCycleParameters.EDayState.Night || activeDayState == DNCycleParameters.EDayState.Sunset)
			{
				base.gameObject.SetActive(value: true);
			}
			else
			{
				base.gameObject.SetActive(value: false);
			}
		}

		private void OnDestroy()
		{
			DayNightCycle.Instance.RemoveListenerEvent(DayCallback, NightCallback);
		}
	}
}

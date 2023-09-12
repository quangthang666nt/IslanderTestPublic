using System;
using System.Collections;
using UnityEngine;

namespace SCS.Gameplay
{
	public class DNCycleSunController : MonoBehaviour
	{
		[SerializeField]
		private Light sun;

		private Coroutine sunLightTransition;

		private float currentLightTime;

		private void Start()
		{
			Light componentInChildren = GetComponentInChildren<Light>();
			if (componentInChildren != null && componentInChildren.type == LightType.Directional)
			{
				sun = componentInChildren;
			}
			else
			{
				Debug.LogError("DNCycleSunController: NO CHILDS WITH DIRECTIONAL LIGHTS");
			}
			DayNightCycle instance = DayNightCycle.Instance;
			instance.OnCycleUpdate = (DayNightCycle.UpdateDelegate)Delegate.Combine(instance.OnCycleUpdate, new DayNightCycle.UpdateDelegate(UpdateSun));
			DayNightCycle.Instance.AddListenerEvent(SetDayLighColor, SetNightLighColor);
			ForceUpdate();
		}

		private void ForceUpdate()
		{
			UpdateSun(DayNightCycle.Instance.NormalizedTime);
		}

		private void UpdateGeneralRotation()
		{
			float[] array = new float[4] { 0f, 180f, 180f, 360f };
			float statePerfent;
			DNCycleParameters.EDayState dayNightPercentCompleted = DayNightCycle.Instance.GetDayNightPercentCompleted(out statePerfent);
			float y = Mathf.Lerp(array[(int)dayNightPercentCompleted * 2], array[(int)dayNightPercentCompleted * 2 + 1], statePerfent);
			base.transform.rotation = Quaternion.Euler(0f, y, 0f);
		}

		private AnimationCurve GetInclinationAnimationCurve(DNCycleParameters.EDayState dayState)
		{
			return dayState switch
			{
				DNCycleParameters.EDayState.Day => DayNightCycle.Instance.DayLightInclinationCurve, 
				DNCycleParameters.EDayState.Night => DayNightCycle.Instance.NightLightInclinationCurve, 
				DNCycleParameters.EDayState.Sunrise => DayNightCycle.Instance.SunriseLightInclinationCurve, 
				DNCycleParameters.EDayState.Sunset => DayNightCycle.Instance.NightfallLightInclinationCurve, 
				_ => DayNightCycle.Instance.DayLightInclinationCurve, 
			};
		}

		private void UpdateInclination()
		{
			DayNightCycle instance = DayNightCycle.Instance;
			float[] array = new float[8] { instance.SunriseLightInclination, instance.DayLightInclination, instance.NightfallLightInclination, instance.NightLightInclination, instance.NightLightInclination, instance.SunriseLightInclination, instance.DayLightInclination, instance.NightfallLightInclination };
			float statePerfent;
			DNCycleParameters.EDayState stateTransitionPercentCompleted = DayNightCycle.Instance.GetStateTransitionPercentCompleted(out statePerfent);
			AnimationCurve inclinationAnimationCurve = GetInclinationAnimationCurve(stateTransitionPercentCompleted);
			int num = (int)stateTransitionPercentCompleted * 2;
			float x = Mathf.Lerp(array[num], array[num + 1], inclinationAnimationCurve.Evaluate(statePerfent));
			sun.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
		}

		private void SetDayLighColor()
		{
			if (sunLightTransition != null)
			{
				StopCoroutine(sunLightTransition);
			}
			sunLightTransition = StartCoroutine(TransitionSunLight(day: true));
		}

		private void SetNightLighColor()
		{
			if (sunLightTransition != null)
			{
				StopCoroutine(sunLightTransition);
			}
			sunLightTransition = StartCoroutine(TransitionSunLight(day: false));
		}

		private IEnumerator TransitionSunLight(bool day)
		{
			Color sunColor = DayNightCycle.Instance.SunColor.Evaluate(DayNightCycle.Instance.NormalizedTime);
			Color MoonColor = DayNightCycle.Instance.MoonColor.Evaluate(DayNightCycle.Instance.NormalizedTime);
			while ((currentLightTime != 0f && day) || (currentLightTime != 1f && !day))
			{
				if (!day)
				{
					currentLightTime += Time.deltaTime;
					if (currentLightTime >= 1f)
					{
						currentLightTime = 1f;
					}
				}
				else
				{
					currentLightTime -= Time.deltaTime;
					if (currentLightTime <= 0f)
					{
						currentLightTime = 0f;
					}
				}
				sun.color = Color.Lerp(sunColor, MoonColor, currentLightTime);
				yield return null;
			}
		}

		private void UpdateSun(float fnormalizedTime)
		{
			UpdateGeneralRotation();
			UpdateInclination();
		}
	}
}

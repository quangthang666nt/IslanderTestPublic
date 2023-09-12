using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace SCS.Gameplay
{
	[CreateAssetMenu(menuName = "CUSTOM/New Day Night Cycle Parameters")]
	public class DNCycleParameters : ScriptableObject
	{
		[Serializable]
		public class CycleGradient
		{
			[SerializeField]
			private Color sunriseColor;

			[SerializeField]
			private Color dayColor;

			[SerializeField]
			private Color nightfallColor;

			[SerializeField]
			private Color nightColor;

			public Gradient GetGradient(float maxDayTime, float MaxCycleTime, float nightStart, float sunriseOffset = 0.1f, float nightfallOffset = 0.1f)
			{
				float num = maxDayTime / MaxCycleTime;
				List<GradientColorKey> list = new List<GradientColorKey>();
				list.Add(new GradientColorKey(nightColor, 0f));
				if (sunriseOffset > 0f)
				{
					list.Add(new GradientColorKey(sunriseColor, sunriseOffset));
					list.Add(new GradientColorKey(dayColor, sunriseOffset + 0.1f));
				}
				else
				{
					list.Add(new GradientColorKey(dayColor, 0.1f));
				}
				if (maxDayTime > 0f)
				{
					list.Add(new GradientColorKey(dayColor, num));
					if (nightfallOffset > 0f)
					{
						list.Add(new GradientColorKey(nightfallColor, num + nightfallOffset));
					}
					list.Add(new GradientColorKey(nightColor, num + nightfallOffset + 0.1f));
				}
				list.Add(new GradientColorKey(nightColor, 1f));
				return new Gradient
				{
					colorKeys = list.ToArray(),
					mode = GradientMode.Blend
				};
			}

			public static Gradient GetGradient(Color sunriseColor, Color dayColor, Color nightfallColor, Color nightColor, float maxDayTime, float MaxCycleTime, float nightStart, float sunriseOffset = 0.1f, float nightfallOffset = 0.1f)
			{
				float num = maxDayTime / MaxCycleTime;
				List<GradientColorKey> list = new List<GradientColorKey>();
				list.Add(new GradientColorKey(nightColor, 0f));
				if (sunriseOffset > 0f)
				{
					list.Add(new GradientColorKey(sunriseColor, sunriseOffset));
					list.Add(new GradientColorKey(dayColor, sunriseOffset + 0.1f));
				}
				else
				{
					list.Add(new GradientColorKey(dayColor, 0.1f));
				}
				if (maxDayTime > 0f)
				{
					list.Add(new GradientColorKey(dayColor, num));
					if (nightfallOffset > 0f)
					{
						list.Add(new GradientColorKey(nightfallColor, num + nightfallOffset));
					}
					list.Add(new GradientColorKey(nightColor, num + nightfallOffset + 0.1f));
				}
				list.Add(new GradientColorKey(nightColor, 1f));
				return new Gradient
				{
					colorKeys = list.ToArray(),
					mode = GradientMode.Blend
				};
			}
		}

		public enum EDayState
		{
			Day = 0,
			Night = 1,
			Sunrise = 2,
			Sunset = 3
		}

		private readonly Vector2 NightDegLimits = new Vector2(180f, 360f);

		[Header("Data")]
		[SerializeField]
		[Tooltip("How long seconds needs to complete a cycle")]
		private float maxDayTime = 120f;

		[SerializeField]
		[Tooltip("How long seconds needs to complete a cycle")]
		private float maxNightTime = 40f;

		[SerializeField]
		[Tooltip("Determines the start hour at the beggining of the scene")]
		private int startDayHour;

		[SerializeField]
		[Range(0f, 0.5f)]
		private float sunriseOffset = 0.1f;

		[SerializeField]
		[Range(0f, 0.5f)]
		private float nightFallOffset = 0.1f;

		[Header("Ambient")]
		[SerializeField]
		private CycleGradient ambientCycle;

		[SerializeField]
		private AnimationCurve ambientIntensity;

		[Header("Ambient Ground")]
		[SerializeField]
		private CycleGradient ambientCycleGround;

		[Header("Ambient Equator")]
		[SerializeField]
		private CycleGradient ambientCycleEquator;

		[Header("Fog")]
		[SerializeField]
		private CycleGradient fogCycle;

		[SerializeField]
		private AnimationCurve fogDensity;

		[SerializeField]
		private Vector2 FogDistance = new Vector2(5f, 30f);

		[Header("Sun")]
		[SerializeField]
		private Gradient sunColor;

		[SerializeField]
		private AnimationCurve sunIntensity;

		[Header("Moon")]
		[SerializeField]
		private Gradient moonColor;

		[SerializeField]
		private AnimationCurve moonIntensity;

		[Header("Buildings Lights")]
		[SerializeField]
		private Color nightLightColor;

		[Header("Shadows")]
		[SerializeField]
		private float lightInclinationAngleDay;

		[SerializeField]
		private float lightInclinationAngleNight;

		[SerializeField]
		private float lightInclinationAngleSunrise;

		[SerializeField]
		private float lightInclinationAngleNightfall;

		[Help("All curves values must go from 0 to 1.", MessageType.Info)]
		[SerializeField]
		[Tooltip("Curve for the transition from Sunrise to Day")]
		private AnimationCurve lightInclinationAnimationDay;

		[SerializeField]
		[Tooltip("Curve for the transition from Nightfall to Night")]
		private AnimationCurve lightInclinationAnimationNight;

		[SerializeField]
		[Tooltip("Curve for the transition from Night to Sunrise")]
		private AnimationCurve lightInclinationAnimationSunrise;

		[SerializeField]
		[Tooltip("Curve for the transition from Day to Nightfall")]
		private AnimationCurve lightInclinationAnimationNightfall;

		public float MaxCycleTime => maxDayTime + maxNightTime;

		public float MaxDayTime => maxDayTime;

		public float NightTime => maxNightTime;

		public int StartDayTime => startDayHour;

		public float SunriseOffset => sunriseOffset;

		public float NightFallOffset => nightFallOffset;

		public Gradient AmbientColor => ambientCycle.GetGradient(maxDayTime, MaxCycleTime, NightTime / MaxCycleTime, sunriseOffset, nightFallOffset);

		public Gradient FogColor => fogCycle.GetGradient(maxDayTime, MaxCycleTime, NightTime / MaxCycleTime, sunriseOffset, nightFallOffset);

		public Gradient SunColor => sunColor;

		public Gradient MoonColor => moonColor;

		public float DayLightInclination => lightInclinationAngleDay;

		public float NightLightInclination => lightInclinationAngleNight;

		public float SunriseLightInclination => lightInclinationAngleSunrise;

		public float NightfallLightInclination => lightInclinationAngleNightfall;

		public AnimationCurve DayLightInclinationCurve => lightInclinationAnimationDay;

		public AnimationCurve NightLightInclinationCurve => lightInclinationAnimationNight;

		public AnimationCurve SunriseLightInclinationCurve => lightInclinationAnimationSunrise;

		public AnimationCurve NightfallLightInclinationCurve => lightInclinationAnimationNightfall;

		public EDayState GetDayState(float currentTime)
		{
			float num = MaxDayTime / MaxCycleTime;
			if (currentTime < sunriseOffset + 0.1f)
			{
				return EDayState.Sunrise;
			}
			if (currentTime < num + nightFallOffset)
			{
				return EDayState.Day;
			}
			if (currentTime < num + nightFallOffset + 0.1f)
			{
				return EDayState.Sunset;
			}
			return EDayState.Night;
		}

		public EDayState GetDayState(float currentTime, out float statePercent)
		{
			float num = MaxDayTime / MaxCycleTime;
			statePercent = 0f;
			if (currentTime < sunriseOffset + 0.1f)
			{
				statePercent = currentTime / (sunriseOffset + 0.1f);
				return EDayState.Sunrise;
			}
			if (currentTime < num + nightFallOffset)
			{
				statePercent = (currentTime - num) / nightFallOffset;
				return EDayState.Day;
			}
			if (currentTime < num + nightFallOffset + 0.1f)
			{
				statePercent = (currentTime - num - nightFallOffset) / 0.1f;
				return EDayState.Sunset;
			}
			statePercent = (currentTime - num - nightFallOffset - 0.1f) / (1f - num - nightFallOffset - 0.1f);
			return EDayState.Night;
		}

		public float GetStartNightPercent()
		{
			return maxNightTime / MaxCycleTime + sunriseOffset + nightFallOffset;
		}

		public float GetStartDayPercent(float normalizedCycle, float dayNormal)
		{
			return (normalizedCycle - dayNormal) / nightFallOffset + sunriseOffset;
		}

		public EDayState GetDayNightPercentCompleted(float currentTime, out float statePercent)
		{
			float num = MaxDayTime / MaxCycleTime;
			statePercent = 0f;
			if (currentTime < num + nightFallOffset)
			{
				statePercent = currentTime / (num + nightFallOffset);
				return EDayState.Day;
			}
			statePercent = (currentTime - num - nightFallOffset) / (1f - num - nightFallOffset);
			return EDayState.Night;
		}

		public EDayState GetStateTransitionPercentCompleted(float currentTime, out float statePercent)
		{
			float num = MaxDayTime / MaxCycleTime;
			float num2 = (sunriseOffset + 0.1f) * 0.5f;
			float num3 = sunriseOffset + 0.1f + (num + nightFallOffset - (sunriseOffset + 0.1f)) * 0.5f;
			float num4 = num + nightFallOffset + (num + nightFallOffset + 0.1f - (num + nightFallOffset)) * 0.5f;
			float num5 = num + nightFallOffset + 0.1f + (1f - (num + nightFallOffset + 0.1f)) * 0.5f;
			float num6 = currentTime;
			if (num6 < num2 || num6 > num5)
			{
				float num7 = num5 - 1f;
				if (num6 > num5)
				{
					num6 -= 1f;
					statePercent = (num6 - num7) / (num2 - num7);
				}
				else
				{
					statePercent = (num6 - num7) / (num2 - num7);
				}
				return EDayState.Sunrise;
			}
			if (num6 < num3)
			{
				statePercent = (num6 - num2) / (num3 - num2);
				return EDayState.Day;
			}
			if (num6 < num4)
			{
				statePercent = (num6 - num3) / (num4 - num3);
				return EDayState.Sunset;
			}
			statePercent = (num6 - num4) / (num5 - num4);
			return EDayState.Night;
		}

		public int GetHour(float currentTime)
		{
			return Mathf.FloorToInt(currentTime / MaxCycleTime * 24f);
		}

		public float GetNormalizedTime(float currentTime)
		{
			return currentTime / MaxCycleTime;
		}

		public float MultiplierTime(float rotX)
		{
			if (!(rotX > NightDegLimits.x) || !(rotX < NightDegLimits.y))
			{
				return maxNightTime / MaxCycleTime + 1f;
			}
			return maxDayTime / MaxCycleTime;
		}

		public void UpdateCycle(Light sun, float normalized)
		{
			SetLightValue(sun, sunColor, sunIntensity, normalized);
			SetFog(normalized);
			SetAmbient(normalized);
			SetBuildingsLights(normalized);
		}

		private void SetLightValue(Light light, Gradient gradient, AnimationCurve curve, float normalized)
		{
			if (!light)
			{
				Debug.LogError("There is no light set in DNCycle");
				return;
			}
			light.color = gradient.Evaluate(normalized);
			light.intensity = curve.Evaluate(normalized);
		}

		private void SetAmbient(float normalized)
		{
			AmbientMode ambientMode = RenderSettings.ambientMode;
			Color color = ambientCycle.GetGradient(maxDayTime, MaxCycleTime, NightTime, sunriseOffset, nightFallOffset).Evaluate(normalized);
			float num = ambientIntensity.Evaluate(normalized);
			if (ambientMode == AmbientMode.Skybox)
			{
				RenderSettings.skybox.SetColor("_Tint", color);
				RenderSettings.ambientIntensity = num;
			}
			else
			{
				RenderSettings.ambientLight = color;
			}
			RenderSettings.ambientEquatorColor = ambientCycleEquator.GetGradient(maxDayTime, MaxCycleTime, NightTime, sunriseOffset, nightFallOffset).Evaluate(normalized);
			RenderSettings.ambientGroundColor = ambientCycleGround.GetGradient(maxDayTime, MaxCycleTime, NightTime, sunriseOffset, nightFallOffset).Evaluate(normalized);
		}

		private void SetFog(float normalized)
		{
			RenderSettings.fogColor = fogCycle.GetGradient(maxDayTime, MaxCycleTime, NightTime, sunriseOffset, nightFallOffset).Evaluate(normalized);
			RenderSettings.fogStartDistance = FogDistance.x;
			RenderSettings.fogEndDistance = FogDistance.y;
			RenderSettings.fogDensity = fogDensity.Evaluate(normalized);
		}

		private void SetBuildingsLights(float normalized)
		{
			Shader.SetGlobalColor("_NightLightColor", nightLightColor);
			Shader.SetGlobalFloat("_NightCycleValue", normalized);
		}
	}
}

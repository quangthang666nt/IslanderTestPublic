using System;
using UnityEngine;

namespace SCS.Gameplay
{
	public class DayNightCycle : MonoBehaviour
	{
		public delegate void UpdateDelegate(float ftime);

		private static DayNightCycle instance;

		[Header("References")]
		[SerializeField]
		private Transform rootLights;

		[SerializeField]
		private Light sun;

		[SerializeField]
		private GameObject cycleLight;

		[SerializeField]
		private GameObject noCycleLight;

		[SerializeField]
		[Range(0f, 1f)]
		private float frange;

		[SerializeField]
		private DNCycleParameters parameters;

		[SerializeField]
		private bool updateCycle = true;

		private bool disabled;

		private float fdayTime;

		private float rotX;

		public int hour;

		private float lastTime;

		private Action DayStartEvent;

		private Action NightStartEvent;

		private Action CycleDisabled;

		public UpdateDelegate OnCycleUpdate;

		private DNCycleParameters.EDayState dayState;

		public static DayNightCycle Instance => instance;

		public bool CycleActive => updateCycle;

		public bool IsCycleDisabled => disabled;

		public DNCycleParameters Parameters => parameters;

		public DNCycleParameters.EDayState DayState => dayState;

		public bool IsUpdatingCycle => updateCycle;

		public float DayLightInclination => parameters.DayLightInclination;

		public AnimationCurve DayLightInclinationCurve => parameters.DayLightInclinationCurve;

		public float NightLightInclination => parameters.NightLightInclination;

		public AnimationCurve NightLightInclinationCurve => parameters.NightLightInclinationCurve;

		public float SunriseLightInclination => parameters.SunriseLightInclination;

		public AnimationCurve SunriseLightInclinationCurve => parameters.SunriseLightInclinationCurve;

		public float NightfallLightInclination => parameters.NightfallLightInclination;

		public AnimationCurve NightfallLightInclinationCurve => parameters.NightfallLightInclinationCurve;

		public float NormalizedTime => frange;

		public float MaxCycleTime => parameters.MaxCycleTime;

		public Gradient SunColor => parameters.SunColor;

		public Gradient MoonColor => parameters.MoonColor;

		public DNCycleParameters.EDayState GetDayStateStatus(out float statePerfent)
		{
			return parameters.GetDayState(frange, out statePerfent);
		}

		public DNCycleParameters.EDayState GetDayNightPercentCompleted(out float statePerfent)
		{
			return parameters.GetDayNightPercentCompleted(frange, out statePerfent);
		}

		public float GetStartDay()
		{
			return parameters.GetStartDayPercent(0f, parameters.MaxDayTime / parameters.MaxCycleTime);
		}

		public float GetStartNight()
		{
			return parameters.GetStartNightPercent();
		}

		public DNCycleParameters.EDayState GetStateTransitionPercentCompleted(out float statePerfent)
		{
			return parameters.GetStateTransitionPercentCompleted(frange, out statePerfent);
		}

		public float RotateSpeed()
		{
			return 360f / parameters.MaxCycleTime;
		}

		public void SetRotation(float normalize)
		{
			rootLights.transform.rotation = Quaternion.Euler(Mathf.Lerp(0f, 360f, frange), Mathf.Lerp(0f, 360f, frange / 2f), 0f);
		}

		public void ManualUpdateCycle(float normalize)
		{
			updateCycle = false;
			fdayTime = normalize * parameters.MaxCycleTime;
			UpdateCycle();
		}

		public void SetCycleActive(bool isActive)
		{
			updateCycle = isActive;
		}

		public void ChangeParameters(DNCycleParameters newParameters, bool autoUpdate = true)
		{
			if ((bool)newParameters)
			{
				_ = parameters;
				parameters = newParameters;
				InitializeParametersValues();
				UpdateCycle();
			}
		}

		public void Enable()
		{
			updateCycle = true;
			disabled = false;
			cycleLight.SetActive(value: true);
			noCycleLight.SetActive(value: false);
			UpdateCycle();
		}

		public void Disable()
		{
			updateCycle = false;
			disabled = true;
			cycleLight.SetActive(value: false);
			noCycleLight.SetActive(value: true);
			CycleDisabled?.Invoke();
			InitializeParametersValues();
			UpdateCycle();
			ColorGenerator.singleton.ResetRenderSettings();
		}

		private void InitializeParametersValues()
		{
			fdayTime = (float)parameters.StartDayTime / 24f * parameters.MaxCycleTime;
			frange = parameters.GetNormalizedTime(fdayTime);
			SetRotation(frange);
		}

		private void UpdateCycle()
		{
			lastTime = fdayTime;
			fdayTime += Time.deltaTime;
			fdayTime %= parameters.MaxCycleTime;
			frange = parameters.GetNormalizedTime(fdayTime);
			rotX += Time.deltaTime * RotateSpeed();
			if (rotX >= 360f)
			{
				rotX = 0f;
			}
			SetRotation(frange);
			if ((bool)parameters)
			{
				parameters.UpdateCycle(sun, frange);
			}
			DNCycleParameters.EDayState eDayState = parameters.GetDayState(frange);
			if (dayState != eDayState)
			{
				dayState = eDayState;
				switch (dayState)
				{
				case DNCycleParameters.EDayState.Sunrise:
					DayStartEvent?.Invoke();
					break;
				case DNCycleParameters.EDayState.Sunset:
					NightStartEvent?.Invoke();
					break;
				}
			}
			OnCycleUpdate?.Invoke(frange);
		}

		private void Awake()
		{
			instance = this;
			InitializeParametersValues();
		}

		private void Start()
		{
			SaveLoadManager.singleton.eventOnTransitionMidEnd.AddListener(delegate
			{
				if (!SettingsManager.Singleton.CurrentData.gameplayData.enableDayNightCycle)
				{
					Disable();
				}
			});
		}

		private void Update()
		{
			if (updateCycle && !noCycleLight.activeInHierarchy)
			{
				UpdateCycle();
			}
		}

		public void AddListenerEvent(Action OnDayStart, Action OnNightStart, Action OnCycleDisabled = null)
		{
			if (OnDayStart != null)
			{
				DayStartEvent = (Action)Delegate.Combine(DayStartEvent, OnDayStart);
			}
			if (OnNightStart != null)
			{
				NightStartEvent = (Action)Delegate.Combine(NightStartEvent, OnNightStart);
			}
			if (OnCycleDisabled != null)
			{
				CycleDisabled = (Action)Delegate.Combine(CycleDisabled, OnCycleDisabled);
			}
		}

		public void RemoveListenerEvent(Action OnDayStart, Action OnNightStart, Action OnCycleDisabled = null)
		{
			if (OnDayStart != null)
			{
				DayStartEvent = (Action)Delegate.Remove(DayStartEvent, OnDayStart);
			}
			if (OnNightStart != null)
			{
				NightStartEvent = (Action)Delegate.Remove(NightStartEvent, OnNightStart);
			}
			if (OnCycleDisabled != null)
			{
				CycleDisabled = (Action)Delegate.Combine(CycleDisabled, OnCycleDisabled);
			}
		}
	}
}

using System;
using System.Collections;
using UnityEngine;

namespace SCS.Gameplay
{
	public class DNCycleSoundController : MonoBehaviour
	{
		private const float waterVolumeInDay = 0.3f;

		private const float waterVolumeInNight = 0.9f;

		private const float torchVolumeInNight = 0.5f;

		[Header("References")]
		[SerializeField]
		private AudioSource owlAudiosource;

		[SerializeField]
		private AudioSource roosterAudiosource;

		[SerializeField]
		private AudioSource waterAudiosource;

		[SerializeField]
		private AudioSource cricketsAudiosource;

		[SerializeField]
		private AudioSource torchAudiosource;

		[SerializeField]
		private AudioSource[] buildingAudiosources;

		[Header("Config")]
		[SerializeField]
		[Range(0f, 1f)]
		private float nightSoundTransition = 0.3f;

		[SerializeField]
		[Range(0f, 1f)]
		private float daySoundTransition = 0.3f;

		public void Start()
		{
			DayNightCycle.Instance.AddListenerEvent(DayStartEventHandler, NightStartEventHandler, CycleDisabledEventHandler);
		}

		private void OnDisable()
		{
			DayNightCycle.Instance.RemoveListenerEvent(DayStartEventHandler, NightStartEventHandler, CycleDisabledEventHandler);
		}

		private void DayStartEventHandler()
		{
			DayStart();
			StopAllCoroutines();
			StartCoroutine(LerpToOne(daySoundTransition, NightToDay, DayComplete));
		}

		private void NightStartEventHandler()
		{
			NightStart();
			StopAllCoroutines();
			StartCoroutine(LerpToOne(nightSoundTransition, DayToNight, NightComplete));
		}

		private void CycleDisabledEventHandler()
		{
			StopAllCoroutines();
			StartCoroutine(LerpToOne(daySoundTransition, NightToDay, DayComplete));
		}

		private IEnumerator LerpToOne(float time, Action<float> toone, Action completed)
		{
			float cooldown = 0f;
			while (cooldown <= time)
			{
				cooldown += Time.deltaTime;
				toone?.Invoke(cooldown);
				yield return null;
			}
			completed?.Invoke();
		}

		private void NightStart()
		{
			if ((bool)owlAudiosource)
			{
				owlAudiosource.Play();
			}
			if ((bool)cricketsAudiosource)
			{
				cricketsAudiosource.volume = 0f;
				cricketsAudiosource.Play();
			}
			if ((bool)torchAudiosource)
			{
				torchAudiosource.volume = 0f;
				torchAudiosource.Play();
			}
			for (int i = 0; i < buildingAudiosources.Length; i++)
			{
				buildingAudiosources[i].volume = 0f;
				buildingAudiosources[i].Play();
			}
		}

		private void NightToDay(float delta)
		{
			if ((bool)cricketsAudiosource)
			{
				cricketsAudiosource.volume = Mathf.Lerp(cricketsAudiosource.volume, 0f, delta - 0.1f);
			}
			if ((bool)waterAudiosource)
			{
				waterAudiosource.volume = Mathf.Lerp(waterAudiosource.volume, 0.3f, delta);
			}
			if ((bool)torchAudiosource)
			{
				torchAudiosource.volume = Mathf.Lerp(torchAudiosource.volume, 0f, delta);
			}
		}

		private void NightComplete()
		{
			if ((bool)cricketsAudiosource)
			{
				cricketsAudiosource.volume = 1f;
			}
		}

		private void DayStart()
		{
			if ((bool)roosterAudiosource)
			{
				float delay = UnityEngine.Random.Range(0f, 1f);
				roosterAudiosource.PlayDelayed(delay);
			}
			for (int i = 0; i < buildingAudiosources.Length; i++)
			{
				buildingAudiosources[i].volume = 0f;
				buildingAudiosources[i].Play();
			}
		}

		private void DayToNight(float delta)
		{
			if ((bool)cricketsAudiosource)
			{
				cricketsAudiosource.volume = Mathf.Lerp(cricketsAudiosource.volume, 0f, delta);
			}
			if ((bool)waterAudiosource)
			{
				waterAudiosource.volume = Mathf.Lerp(waterAudiosource.volume, 0.9f, delta);
			}
			for (int i = 0; i < buildingAudiosources.Length; i++)
			{
				buildingAudiosources[i].volume = Mathf.Lerp(buildingAudiosources[i].volume, 0f, delta);
			}
			if ((bool)torchAudiosource)
			{
				torchAudiosource.volume = Mathf.Lerp(torchAudiosource.volume, 0.5f, delta);
			}
		}

		private void DayComplete()
		{
			if ((bool)cricketsAudiosource)
			{
				cricketsAudiosource.Stop();
			}
			if ((bool)torchAudiosource)
			{
				torchAudiosource.Stop();
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using SCS.Gameplay;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[Serializable]
	public struct ClipData
	{
		public string sName;

		public AudioClip[] arClips;

		[Range(0f, 1f)]
		public float fVolume;

		[Range(0f, 0.5f)]
		public float fPitchRandomRange;
	}

	[Serializable]
	public class BuildingSoundSetup
	{
		[HideInInspector]
		public string sName;

		public AudioSource asAudioSource;

		public AnimationCurve acVolumeOverContribution = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		[Tooltip("At which contribution should the volume curve start and at which contribution-value should it end (reach the max volume)?")]
		public Vector2 v2MinMaxContribution = new Vector2(0f, 1f);

		public float fMaxDistance = 5f;

		public Vector2 v2MinMaxVolume = new Vector2(0f, 1f);

		[Tooltip("Set this to the same soundtype as the AudioContributers that are contributing to this sound.")]
		public BuildingSounds correspondingSoundType;

		[HideInInspector]
		public float fSmoothRef;

		[HideInInspector]
		public float fTargetVolume;
	}

	public enum BuildingSounds
	{
		village01 = 0,
		village02 = 1,
		village03 = 2,
		industrialWood01 = 3,
		industrialWood02 = 4,
		industrialStone01 = 5,
		industrialStone02 = 6,
		farming01 = 7,
		fountain01 = 8,
		windmill01 = 9
	}

	public static AudioManager singleton;

	[Header("Building Sounds")]
	[SerializeField]
	private List<BuildingSoundSetup> buildingSoundSetups;

	[Header(" ")]
	[Tooltip("Just assign a transform that marks the audioListener's position here.")]
	[SerializeField]
	private Transform tAudioListenerPosition;

	[SerializeField]
	private AnimationCurve acContributionFalloff = AnimationCurve.Linear(1f, 1f, 0f, 0f);

	[Header(" ")]
	[Header("Audio Sources")]
	[SerializeField]
	private AudioSource asStandardOneshot;

	[SerializeField]
	private AudioSource asPitchableStandardOneShots;

	[SerializeField]
	private AudioSource asUIOneShot;

	[SerializeField]
	private AudioSource asSoundtrackSource;

	[SerializeField]
	private AudioSource asAmbienceSource;

	[SerializeField]
	private AudioSource asCoinCollectLoop;

	[SerializeField]
	private AudioSource asCoinSpawnLoop;

	[SerializeField]
	private AudioSource asTransitionSound;

	[SerializeField]
	private AudioSource asRainAmbience;

	[Header("Transition Sound")]
	[Range(0f, 1f)]
	[SerializeField]
	private float fTransitionSoundVolume = 0.5f;

	[Range(0f, 5f)]
	[SerializeField]
	private float fTransitionSoundFadeTime = 3f;

	[SerializeField]
	private AnimationCurve curveSoundtrackMuteCurve;

	[SerializeField]
	private float fSoundtrackMuteTime = 3f;

	[SerializeField]
	private float fPlaylistChangeFadeTime = 0.5f;

	[Header("Ambience Sounds")]
	[Range(0f, 1f)]
	[SerializeField]
	private float fRainVolume = 0.5f;

	[Range(0f, 10f)]
	[SerializeField]
	private float fAmbienceFadeTime = 3f;

	[Header("Coin Sounds")]
	[SerializeField]
	[Range(0f, 1f)]
	private float fCoinSpawnVolume = 0.5f;

	[SerializeField]
	[Range(0f, 1f)]
	private float fCoinCollectVolume = 0.5f;

	[Header(" ")]
	[Header("One Shots")]
	[SerializeField]
	private List<ClipData> cdOneShots = new List<ClipData>();

	[Header("Playlist")]
	[SerializeField]
	private CatalogConfig.ExtraPlaylist originalPlaylist;

	[Header("Joysticks SFX (PS4 only)")]
	[SerializeField]
	private AudioSource[] joysticksAudioSource = new AudioSource[4];

	[HideInInspector]
	public List<AudioContributer> audioContributorsInGame = new List<AudioContributer>();

	private const float gameOverFadeInTime = 0.1f;

	private const float gameOverFadeOutTime = 1.5f;

	private const float gameOverWaitTime = 4f;

	private const float updateInervalBuildingSounds = 0.1f;

	private const float updateIntervalSoundtrack = 4f;

	private float timerBuildingSounds;

	private float timerSoundTrack;

	private float noCoinsArrivedSince = 100f;

	private float noCoinsSpawnedSince = 100f;

	private bool transitionSoundFadedIn = true;

	private bool updateBuildingSounds = true;

	private Coroutine coroutLastTransitionSoundFade;

	private Coroutine coroutLastRainFade;

	private bool bPlaylistChanging;

	private float soundtrackBaseVolume;

	private AudioClip soundtrackOriginalClip;

	private AudioClip[] fanfareOriginalClips;

	private Coroutine cChangeMusicPlaylist;

	private List<AudioClip> lCurrentPlaylist = new List<AudioClip>();

	private int iCurrentPlaylistTrack = -1;

	private float[] m_ContributionsBuffer;

	private void Awake()
	{
		singleton = this;
		soundtrackBaseVolume = asSoundtrackSource.volume;
		soundtrackOriginalClip = asSoundtrackSource.clip;
		if (cdOneShots.Count > 5)
		{
			fanfareOriginalClips = cdOneShots[5].arClips;
		}
	}

	private void Start()
	{
		CoinManager.eventOnCoinGather.AddListener(CoinArriving);
		SaveLoadManager.singleton.eventOnTransitionStart.AddListener(FadeTransitionFadeIn);
		SaveLoadManager.singleton.eventOnTransitionMidEnd.AddListener(FadeTransitionFadeOut);
		SaveLoadManager.singleton.eventOnTransitionMidEnd.AddListener(UpdateAmbienceSound);
		CoinManager.eventOnPositiveCoinSpawn.AddListener(CoinSpawning);
		FeedbackManager.eventOnBuildingButtonSpawn.AddListener(PlayButtonSpawn);
		UiBuildingButtonManager.singleton.eventOnBuildingPlace.AddListener(PlayBuildingPlaced);
		UiBuildingButtonManager.singleton.eventOnBuildingButtonClick.AddListener(PlayButtonClick);
		UiBuildingButtonManager.singleton.eventOnBuildingCantPlace.AddListener(PlayErrorPrompt);
		UIBuildingChoice.Singleton.eventOnBuildingChoiceClick.AddListener(PlayButtonClick);
		LocalGameManager.singleton.eventOnNewBoosterPack.AddListener(PlayLevelUp);
		SaveLoadManager.singleton.eventOnTransitionMidEnd.AddListener(PlayNewIslandFanfare);
		SaveLoadManager.singleton.eventOnTransitionEnd.AddListener(PlayLevelUp);
		LocalGameManager.singleton.eventOnBuildingChoiceActivate.AddListener(PlayPositivePlop01);
		audioContributorsInGame = new List<AudioContributer>();
		transitionSoundFadedIn = false;
		DayNightCycle.Instance.AddListenerEvent(OnDayStart, OnNightStart);
	}

	private void OnDisable()
	{
		if (DayNightCycle.Instance != null)
		{
			DayNightCycle.Instance.RemoveListenerEvent(OnDayStart, OnNightStart);
		}
	}

	private void Update()
	{
		PlayCoinGatheringLoop();
		PlayCoinSpawningLoop();
		if (updateBuildingSounds)
		{
			UpdateBuildingSounds();
		}
		RunPlaylist();
	}

	private void CoinArriving()
	{
		noCoinsArrivedSince = 0f;
	}

	private void CoinSpawning()
	{
		noCoinsSpawnedSince = 0f;
	}

	private void PlayCoinGatheringLoop()
	{
		if (noCoinsArrivedSince <= CoinManager.Singleton.fCoinSpawnInterval + 0.02f)
		{
			if (asCoinCollectLoop.volume < fCoinCollectVolume)
			{
				asCoinCollectLoop.volume = fCoinCollectVolume;
			}
		}
		else
		{
			asCoinCollectLoop.volume -= 4f * Time.deltaTime;
			if (asCoinCollectLoop.volume <= 0f)
			{
				asCoinCollectLoop.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
			}
		}
		noCoinsArrivedSince += Time.deltaTime;
	}

	private void PlayCoinSpawningLoop()
	{
		if (noCoinsSpawnedSince <= CoinManager.Singleton.fCoinSpawnInterval + 0.4f)
		{
			if (asCoinSpawnLoop.volume < fCoinSpawnVolume)
			{
				asCoinSpawnLoop.volume += 1f * Time.deltaTime;
			}
		}
		else
		{
			asCoinSpawnLoop.volume = Mathf.Clamp01(asCoinSpawnLoop.volume - 0.5f * Time.deltaTime);
			if (asCoinSpawnLoop.volume <= 0f)
			{
				asCoinSpawnLoop.pitch = UnityEngine.Random.Range(0.95f, 1.1f);
			}
		}
		noCoinsSpawnedSince += Time.deltaTime;
	}

	private void UpdateBuildingSounds()
	{
		timerBuildingSounds += Time.deltaTime;
		if (m_ContributionsBuffer == null)
		{
			m_ContributionsBuffer = new float[buildingSoundSetups.Count];
		}
		if (m_ContributionsBuffer.Length != buildingSoundSetups.Count)
		{
			Array.Resize(ref m_ContributionsBuffer, buildingSoundSetups.Count);
		}
		for (int i = 0; i < m_ContributionsBuffer.Length; i++)
		{
			m_ContributionsBuffer[i] = 0f;
		}
		if (timerBuildingSounds >= 0.1f)
		{
			foreach (AudioContributer item in audioContributorsInGame)
			{
				Vector3 vector = tAudioListenerPosition.position - item.transform.position;
				float magnitude = new Vector2(vector.x, vector.z).magnitude;
				foreach (AudioContributer.BuildingSoundSetup soundSetup in item.soundSetups)
				{
					int num = EnumToIndex(soundSetup.buildingSound);
					float time = magnitude / buildingSoundSetups[num].fMaxDistance;
					float num2 = acContributionFalloff.Evaluate(time) * soundSetup.contributionFactor;
					m_ContributionsBuffer[num] += num2;
				}
			}
			for (int j = 0; j < m_ContributionsBuffer.Length; j++)
			{
				BuildingSoundSetup buildingSoundSetup = buildingSoundSetups[j];
				float time2 = (m_ContributionsBuffer[j] - buildingSoundSetup.v2MinMaxContribution.x) / (buildingSoundSetup.v2MinMaxContribution.y - buildingSoundSetup.v2MinMaxContribution.x);
				float num3 = Mathf.Clamp01(buildingSoundSetup.acVolumeOverContribution.Evaluate(time2));
				num3 = num3 * (buildingSoundSetup.v2MinMaxVolume.y - buildingSoundSetup.v2MinMaxVolume.x) + buildingSoundSetup.v2MinMaxVolume.x;
				buildingSoundSetup.fTargetVolume = num3;
			}
			timerBuildingSounds = 0f;
		}
		foreach (BuildingSoundSetup buildingSoundSetup2 in buildingSoundSetups)
		{
			buildingSoundSetup2.asAudioSource.volume = Mathf.SmoothDamp(buildingSoundSetup2.asAudioSource.volume, buildingSoundSetup2.fTargetVolume, ref buildingSoundSetup2.fSmoothRef, 0.1f);
		}
	}

	private void PlayBuildingPlaced()
	{
		PlayOneShotFromClipData(cdOneShots[0], asPitchableStandardOneShots);
	}

	private void PlayLevelUp()
	{
		PlayOneShotFromClipData(cdOneShots[1], asStandardOneshot);
	}

	private void PlayButtonSpawn()
	{
		PlayOneShotFromClipData(cdOneShots[2], asUIOneShot);
	}

	public void PlayButtonClick()
	{
		PlayOneShotFromClipData(cdOneShots[3], asUIOneShot);
	}

	public void PlayMenuClick()
	{
		PlayOneShotFromClipData(cdOneShots[8], asUIOneShot);
	}

	public void PlayErrorPrompt()
	{
		PlayOneShotFromClipData(cdOneShots[4], asStandardOneshot);
	}

	private void PlayNewIslandFanfare()
	{
		StartCoroutine(FadeAudioSourceVolume(0f, fSoundtrackMuteTime, asSoundtrackSource, 0f, curveSoundtrackMuteCurve));
		PlayOneShotFromClipData(cdOneShots[5], asStandardOneshot);
	}

	private void PlayPositivePlop01()
	{
		PlayOneShotFromClipData(cdOneShots[6], asStandardOneshot);
	}

	public void PlayGameOver()
	{
		StartCoroutine(GameOverFadeInOut());
	}

	public void PlayUndoSound()
	{
		PlayOneShotFromClipData(cdOneShots[10], asPitchableStandardOneShots);
	}

	public void PlayDemolishSound()
	{
		PlayOneShotFromClipData(cdOneShots[9], asPitchableStandardOneShots);
	}

	public void ChangeMusicPlaylist(List<AudioClip> playlistTracks)
	{
		if (asSoundtrackSource.loop)
		{
			asSoundtrackSource.loop = false;
		}
		lCurrentPlaylist = playlistTracks;
		iCurrentPlaylistTrack = -1;
		PlayNextMusicInQueue();
	}

	private void PlayNextMusicInQueue()
	{
		if (lCurrentPlaylist != null && lCurrentPlaylist.Count != 0)
		{
			iCurrentPlaylistTrack = (iCurrentPlaylistTrack + 1) % lCurrentPlaylist.Count;
			CleanPrevChangeMusicCoroutines();
			cChangeMusicPlaylist = StartCoroutine(RunChangeMusicPlaylist(lCurrentPlaylist[iCurrentPlaylistTrack]));
		}
	}

	private void RunPlaylist()
	{
		if (!asSoundtrackSource.loop && lCurrentPlaylist != null && lCurrentPlaylist.Count != 0 && !bPlaylistChanging && !asSoundtrackSource.isPlaying)
		{
			PlayNextMusicInQueue();
		}
	}

	public void RestoreFanfare()
	{
		ChangeFanfare(fanfareOriginalClips);
	}

	public void ChangeFanfare(AudioClip[] newFanfareClips)
	{
		if (cdOneShots.Count > 5)
		{
			ClipData value = cdOneShots[5];
			value.arClips = newFanfareClips;
			cdOneShots[5] = value;
		}
	}

	private void CleanPrevChangeMusicCoroutines()
	{
		if (cChangeMusicPlaylist != null)
		{
			if (bPlaylistChanging)
			{
				StopCoroutine(cChangeMusicPlaylist);
				bPlaylistChanging = false;
			}
			cChangeMusicPlaylist = null;
		}
	}

	private IEnumerator RunChangeMusicPlaylist(AudioClip newSoundtrackClip)
	{
		bPlaylistChanging = true;
		StartCoroutine(FadeAudioSourceVolume(0f, fPlaylistChangeFadeTime, asSoundtrackSource, 0f, null, checkChanges: false));
		yield return new WaitForSeconds(fPlaylistChangeFadeTime);
		asSoundtrackSource.Stop();
		asSoundtrackSource.clip = newSoundtrackClip;
		asSoundtrackSource.Play();
		StartCoroutine(FadeAudioSourceVolume(soundtrackBaseVolume, fPlaylistChangeFadeTime, asSoundtrackSource, 0f, null, checkChanges: false));
		yield return new WaitForSeconds(fPlaylistChangeFadeTime);
		bPlaylistChanging = false;
	}

	private bool IsAudioSourceChanging(AudioSource aSource)
	{
		if (bPlaylistChanging)
		{
			return aSource == asSoundtrackSource;
		}
		return false;
	}

	private IEnumerator GameOverFadeInOut()
	{
		float initialMusicVolume = asSoundtrackSource.volume;
		if (!IsAudioSourceChanging(asSoundtrackSource))
		{
			StartCoroutine(FadeAudioSourceVolume(0.1f, 0.1f, asSoundtrackSource));
		}
		yield return new WaitForSeconds(0.1f);
		PlayOneShotFromClipData(cdOneShots[7], asStandardOneshot);
		yield return new WaitForSeconds(4f);
		if (!IsAudioSourceChanging(asSoundtrackSource))
		{
			StartCoroutine(FadeAudioSourceVolume(initialMusicVolume, 1.5f, asSoundtrackSource));
		}
	}

	private void FadeTransitionFadeIn()
	{
		if (coroutLastTransitionSoundFade != null)
		{
			StopCoroutine(coroutLastTransitionSoundFade);
		}
		coroutLastTransitionSoundFade = StartCoroutine(FadeAudioSourceVolume(fTransitionSoundVolume, fTransitionSoundFadeTime, asTransitionSound));
	}

	private void FadeTransitionFadeOut()
	{
		if (coroutLastTransitionSoundFade != null)
		{
			StopCoroutine(coroutLastTransitionSoundFade);
		}
		coroutLastTransitionSoundFade = StartCoroutine(FadeAudioSourceVolume(0f, fTransitionSoundFadeTime, asTransitionSound));
	}

	private void PlayOneShotFromClipData(ClipData clipData, AudioSource asAudioSource)
	{
		AudioClip clip = clipData.arClips[UnityEngine.Random.Range(0, clipData.arClips.Length)];
		float pitch = UnityEngine.Random.Range(1f - clipData.fPitchRandomRange, 1f + clipData.fPitchRandomRange);
		asAudioSource.pitch = pitch;
		asAudioSource.PlayOneShot(clip, clipData.fVolume);
	}

	private void UpdateAmbienceSound()
	{
		if (ColorGenerator.singleton.activeColorSetup.enableRain)
		{
			if (coroutLastRainFade != null)
			{
				StopCoroutine(coroutLastRainFade);
			}
			coroutLastRainFade = StartCoroutine(FadeAudioSourceVolume(fRainVolume, fAmbienceFadeTime, asRainAmbience));
		}
		if (!ColorGenerator.singleton.activeColorSetup.enableRain)
		{
			if (coroutLastRainFade != null)
			{
				StopCoroutine(coroutLastRainFade);
			}
			coroutLastRainFade = StartCoroutine(FadeAudioSourceVolume(0f, fAmbienceFadeTime, asRainAmbience));
		}
	}

	private IEnumerator FadeAudioSourceVolume(float targetVolume, float fadeTime, AudioSource audioSource, float delay = 0f, AnimationCurve fadeCurve = null, bool checkChanges = true)
	{
		yield return new WaitForSeconds(delay);
		float fadeTimer = 0f;
		float startVolume = audioSource.volume;
		while (fadeTimer < fadeTime)
		{
			float num = fadeTimer / fadeTime;
			if (fadeCurve != null)
			{
				num = fadeCurve.Evaluate(num);
			}
			if (checkChanges && IsAudioSourceChanging(audioSource))
			{
				yield break;
			}
			audioSource.volume = Mathf.Lerp(startVolume, targetVolume, num);
			fadeTimer += Time.deltaTime;
			yield return null;
		}
		if (!checkChanges || !IsAudioSourceChanging(audioSource))
		{
			audioSource.volume = targetVolume;
			if (fadeCurve != null)
			{
				audioSource.volume = Mathf.Lerp(startVolume, targetVolume, fadeCurve.Evaluate(1f));
			}
		}
	}

	public int EnumToIndex(BuildingSounds enumInputBuildingSound)
	{
		for (int i = 0; i < buildingSoundSetups.Count; i++)
		{
			if (buildingSoundSetups[i].correspondingSoundType == enumInputBuildingSound)
			{
				return i;
			}
		}
		Debug.LogWarning("There is no matching building-sound in AudioManager.buildingSoundSetups for: " + enumInputBuildingSound.ToString() + ". Create one or ask Friedemann what the F**k is happening.");
		return 0;
	}

	[ContextMenu("Call Debug Method")]
	public void DebugMethod()
	{
	}

	private void OnValidate()
	{
		foreach (BuildingSoundSetup buildingSoundSetup in buildingSoundSetups)
		{
			buildingSoundSetup.sName = buildingSoundSetup.correspondingSoundType.ToString();
		}
	}

	private void Reset()
	{
		buildingSoundSetups = new List<BuildingSoundSetup>
		{
			new BuildingSoundSetup()
		};
	}

	private void OnNightStart()
	{
		updateBuildingSounds = false;
	}

	private void OnDayStart()
	{
		updateBuildingSounds = true;
	}

	public bool IsOriginalPlaylist(string playlistID)
	{
		if (string.IsNullOrEmpty(originalPlaylist.id) || string.IsNullOrEmpty(playlistID))
		{
			return false;
		}
		return originalPlaylist.id.Equals(playlistID);
	}

	public AudioClip[] GetOriginalPlaylistSoundtracks()
	{
		return originalPlaylist.soundtracks;
	}

	public string GetOriginalPlaylistName()
	{
		return originalPlaylist.localizedName;
	}

	public string GetOriginalPlaylistID()
	{
		string result = string.Empty;
		if (!string.IsNullOrEmpty(originalPlaylist.id))
		{
			result = originalPlaylist.id;
		}
		return result;
	}
}

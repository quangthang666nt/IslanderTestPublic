using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using I2.Loc;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

public class SettingsManager : MonoBehaviour
{
	[Serializable]
	public enum SettingsVersion
	{
		Default = 0,
		Content_Patch_1 = 1,
		Content_Patch_1_Bloom = 2,
		Content_Patch_3_Outline = 3
	}

	[Serializable]
	public class SettingsData
	{
		[Serializable]
		public class GameplayData
		{
			public string currentLanguage;

			public bool bShowTutorial;

			public bool enableTooltips = true;

			public bool dragCameraInBuildmode = true;

			public bool enableDayNightCycle = true;

			public bool bloom = true;

			public bool outline = true;

			public float CameraMovementSensibility;

			public float PointerMovementSensibility;
		}

		[Serializable]
		public class VideoData
		{
			[Serializable]
			public class SerializableResolution
			{
				public int height;

				public int width;

				public int refreshRate;

				public SerializableResolution(Resolution _resolution)
				{
					height = _resolution.height;
					width = _resolution.width;
					refreshRate = _resolution.refreshRate;
				}
			}

			public SerializableResolution resolution;

			public bool fullScreen;

			public int antiAliasing = 4;

			public bool ssao = true;

			public bool vSync = true;

			public int shadowResolution = 3;
		}

		[Serializable]
		public class AudioData
		{
			public float volumeMaster = 1f;

			public float volumeWorld = 1f;

			public float volumeScore = 1f;

			public float volumeMusic = 1f;

			public float volumeUI = 1f;
		}

		public GameplayData gameplayData = new GameplayData();

		public VideoData videoData = new VideoData();

		public AudioData audioData = new AudioData();

		public SettingsVersion version = SettingsVersion.Content_Patch_3_Outline;
	}

	private const string FILE_NAME = "User.settings";

	private const float audioMutePoint = -80f;

	private static SettingsManager singleton;

	[Help("This script manages the game settings (resolution, audio volume etc).", MessageType.Info)]
	[SerializeField]
	private AudioMixer audioMixer;

	[SerializeField]
	private AnimationCurve audioFalloffCurve;

	[SerializeField]
	private PostProcessProfile postProcessing;

	[SerializeField]
	private I2Manager i2Manager;

	private SettingsData currentData = new SettingsData();

	private string SaveFilePath => "User.settings";

	public static SettingsManager Singleton => singleton;

	public SettingsData CurrentData
	{
		get
		{
			return currentData;
		}
		set
		{
			currentData = value;
			ApplySettings();
		}
	}

	private void Awake()
	{
		if (singleton != null)
		{
			UnityEngine.Object.Destroy(singleton.gameObject);
		}
		singleton = this;
	}

	public void Save()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		using (MemoryStream memoryStream = new MemoryStream())
		{
			binaryFormatter.Serialize(memoryStream, currentData);
			byte[] data = memoryStream.ToArray();
			PlatformPlayerManagerSystem.Instance.SaveData(SaveFilePath, ref data, OnSaveCompleted);
		}
		ApplySettings();
	}

	private void OnSaveCompleted(string arg1, bool arg2)
	{
		Debug.Log("[SettingsManager] Settings saved!");
	}

	public static void LoadSettings()
	{
		PlatformPlayerManagerSystem.Instance.LoadData(Singleton.SaveFilePath, Singleton.OnLoadCompleted);
	}

	private void OnLoadCompleted(string fileName, LoadResult result, byte[] data)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		if (result == LoadResult.Success)
		{
			using MemoryStream serializationStream = new MemoryStream(data);
			object obj = binaryFormatter.Deserialize(serializationStream);
			currentData = (SettingsData)obj;
			if (currentData.version < SettingsVersion.Content_Patch_1)
			{
				currentData.gameplayData.enableDayNightCycle = true;
				currentData.version = SettingsVersion.Content_Patch_1;
			}
			if (currentData.version < SettingsVersion.Content_Patch_1_Bloom)
			{
				currentData.gameplayData.bloom = true;
				currentData.version = SettingsVersion.Content_Patch_1_Bloom;
			}
			if (currentData.version < SettingsVersion.Content_Patch_3_Outline)
			{
				currentData.gameplayData.outline = true;
				currentData.version = SettingsVersion.Content_Patch_3_Outline;
			}
		}
		else
		{
			currentData = new SettingsData();
			currentData.gameplayData.currentLanguage = PlatformPlayerManagerSystem.Instance.GetDefaultLanguage();
		}
		Debug.Log("Language: " + currentData.gameplayData.currentLanguage);
		if (!LocalizationManager.GetAllLanguages().Contains(currentData.gameplayData.currentLanguage))
		{
			currentData.gameplayData.currentLanguage = LocalizationManager.CurrentLanguage;
		}
		ApplySettings();
	}

	public void ApplySettings()
	{
		i2Manager.SetLanguage(currentData.gameplayData.currentLanguage);
		CameraController.singleton.fButtonTranslationSpeedValue = CameraController.singleton.fButtonTranslationSpeed + currentData.gameplayData.CameraMovementSensibility * CameraController.singleton.fButtonTranslationSpeed;
		InputManager.Singleton.pointerStartSpeedValue = InputManager.Singleton.fPointerStartSpeed + currentData.gameplayData.PointerMovementSensibility * InputManager.Singleton.fPointerStartSpeed;
		InputManager.Singleton.pointerSpeedupOnHoldValue = InputManager.Singleton.fPointerSpeedupOnHold + currentData.gameplayData.PointerMovementSensibility * InputManager.Singleton.fPointerSpeedupOnHold;
		InputManager.Singleton.maxPointerSpeedAdditionValue = InputManager.Singleton.fMaxPointerSpeedAddition + currentData.gameplayData.PointerMovementSensibility * InputManager.Singleton.fMaxPointerSpeedAddition;
		StructureOutline.activeOutline = currentData.gameplayData.outline;
		if (currentData.videoData.resolution != null)
		{
			SetResolutuionTheSafeWay(currentData.videoData.resolution.width, currentData.videoData.resolution.height, currentData.videoData.fullScreen, currentData.videoData.resolution.refreshRate);
		}
		QualitySettings.antiAliasing = currentData.videoData.antiAliasing;
		if (currentData.videoData.vSync)
		{
			QualitySettings.vSyncCount = 1;
		}
		else
		{
			QualitySettings.vSyncCount = 0;
		}
		postProcessing.TryGetSettings<AmbientOcclusion>(out var outSetting);
		if (currentData.videoData.ssao)
		{
			outSetting.active = true;
		}
		else
		{
			outSetting.active = false;
		}
		switch (currentData.videoData.shadowResolution)
		{
		case 0:
			QualitySettings.shadowResolution = ShadowResolution.Low;
			break;
		case 1:
			QualitySettings.shadowResolution = ShadowResolution.Medium;
			break;
		case 2:
			QualitySettings.shadowResolution = ShadowResolution.High;
			break;
		case 3:
			QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
			break;
		}
		float value = Mathf.Lerp(-80f, 0f, audioFalloffCurve.Evaluate(currentData.audioData.volumeMaster));
		float value2 = Mathf.Lerp(-80f, 0f, audioFalloffCurve.Evaluate(currentData.audioData.volumeWorld));
		float value3 = Mathf.Lerp(-80f, 0f, audioFalloffCurve.Evaluate(currentData.audioData.volumeScore));
		float value4 = Mathf.Lerp(-80f, 0f, audioFalloffCurve.Evaluate(currentData.audioData.volumeMusic));
		float value5 = Mathf.Lerp(-80f, 0f, audioFalloffCurve.Evaluate(currentData.audioData.volumeUI));
		audioMixer.SetFloat("VolumeMaster", value);
		audioMixer.SetFloat("VolumeBuildings", value2);
		audioMixer.SetFloat("VolumeIslandAmbient", value2);
		audioMixer.SetFloat("VolumeCoins", value3);
		audioMixer.SetFloat("VolumeMusic", value4);
		audioMixer.SetFloat("VolumeUI", value5);
		audioMixer.SetFloat("VolumeFeedback", value5);
	}

	public void SetResolutuionTheSafeWay(int _iWidth, int _iHeight, bool _bFullscreen, int _iRefreshRate)
	{
		if (SXUIVideoResolution.BIsResolutionSupported(_iWidth, _iHeight, _iRefreshRate))
		{
			Screen.SetResolution(_iWidth, _iHeight, _bFullscreen, _iRefreshRate);
			currentData.videoData.resolution.width = _iWidth;
			currentData.videoData.resolution.height = _iHeight;
			currentData.videoData.resolution.refreshRate = _iRefreshRate;
		}
		else if (SXUIVideoResolution.BIsResolutionSupported(_iWidth, _iHeight, 120))
		{
			Screen.SetResolution(_iWidth, _iHeight, _bFullscreen, 120);
			currentData.videoData.resolution.width = _iWidth;
			currentData.videoData.resolution.height = _iHeight;
			currentData.videoData.resolution.refreshRate = 120;
		}
		else if (SXUIVideoResolution.BIsResolutionSupported(_iWidth, _iHeight, 100))
		{
			Screen.SetResolution(_iWidth, _iHeight, _bFullscreen, 100);
			currentData.videoData.resolution.width = _iWidth;
			currentData.videoData.resolution.height = _iHeight;
			currentData.videoData.resolution.refreshRate = 100;
		}
		else if (SXUIVideoResolution.BIsResolutionSupported(_iWidth, _iHeight, 60))
		{
			Screen.SetResolution(_iWidth, _iHeight, _bFullscreen, 60);
			currentData.videoData.resolution.width = _iWidth;
			currentData.videoData.resolution.height = _iHeight;
			currentData.videoData.resolution.refreshRate = 60;
		}
		else if (SXUIVideoResolution.BIsResolutionSupported(_iWidth, _iHeight, 59))
		{
			Screen.SetResolution(_iWidth, _iHeight, _bFullscreen, 59);
			currentData.videoData.resolution.width = _iWidth;
			currentData.videoData.resolution.height = _iHeight;
			currentData.videoData.resolution.refreshRate = 59;
		}
		else
		{
			Resolution resolution = SXUIVideoResolution.ResolutionGetBestDefault();
			Screen.SetResolution(resolution.width, resolution.height, _bFullscreen, resolution.refreshRate);
			currentData.videoData.resolution.width = resolution.width;
			currentData.videoData.resolution.height = resolution.height;
			currentData.videoData.resolution.refreshRate = resolution.refreshRate;
		}
	}
}

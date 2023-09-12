using System;
using SCS.Gameplay;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class ColorGenerator : MonoBehaviour
{
	[Serializable]
	public class OverrideColorSetup
	{
		public ColorSetup colorSetup;

		[Range(0f, 10f)]
		public float fProbability;
	}

	public static ColorGenerator singleton;

	[Help("This script only changes material colors.That's it. It's responsible for generating nice looking color schemes.", MessageType.Info)]
	[SerializeField]
	[Tooltip("Assing the Rain-Particle System here.")]
	private GameObject rainParticleParent;

	[SerializeField]
	[Tooltip("Assing the Snow-Particle System here.")]
	private GameObject snowParticleParent;

	[SerializeField]
	private bool bChangeFogColor;

	[SerializeField]
	private bool bChangeAmbientColor;

	[SerializeField]
	public ColorSetup[] colorSetups;

	[SerializeField]
	private AnimationCurve hueOffsetCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	[Tooltip("Register lights from the scene here to change them with Color-Presets.")]
	private Light[] sceneLights;

	[SerializeField]
	private PostProcessVolume postProcess;

	private ColorSetup.KeyColorSetup[] keyColorSetups;

	[HideInInspector]
	public ColorSetup activeColorSetup;

	private Color fogColor;

	private float fogStartDistance;

	private float fogEndDistance;

	private float fogDensity;

	private Color ambientSkyBoxColor;

	private float ambientIntensity;

	private Color ambientLight;

	private Color ambientEquatorColor;

	private Color ambientGroundColor;

	private Color nightLightColor;

	private float nightCycleValue;

	private void Awake()
	{
		singleton = this;
	}

	private void Start()
	{
		singleton = this;
		StoreOriginalColors();
		fogColor = RenderSettings.fogColor;
		fogStartDistance = RenderSettings.fogStartDistance;
		fogEndDistance = RenderSettings.fogEndDistance;
		fogDensity = RenderSettings.fogDensity;
		ambientSkyBoxColor = RenderSettings.skybox.GetColor("_Tint");
		ambientIntensity = RenderSettings.ambientIntensity;
		ambientLight = RenderSettings.ambientLight;
		ambientEquatorColor = RenderSettings.ambientEquatorColor;
		ambientGroundColor = RenderSettings.ambientGroundColor;
		nightLightColor = Shader.GetGlobalColor("_NightLightColor");
		nightCycleValue = Shader.GetGlobalFloat("_NightCycleValue");
		SaveLoadManager.singleton.eventOnTransitionMidEnd.AddListener(delegate
		{
			if (!SettingsManager.Singleton.CurrentData.gameplayData.bloom)
			{
				EnableBloom(value: false);
			}
		});
	}

	[ContextMenu("Generate Color-Scheme")]
	public void GenerateColorScheme(ColorSetup _colorSetup = null)
	{
		singleton = this;
		activeColorSetup = _colorSetup;
		if (!activeColorSetup)
		{
			activeColorSetup = PickRandomColorSetup();
		}
		keyColorSetups = new ColorSetup.KeyColorSetup[4];
		keyColorSetups[0] = activeColorSetup.keyColorSetup1;
		keyColorSetups[1] = activeColorSetup.keyColorSetup2;
		keyColorSetups[2] = activeColorSetup.keyColorSetup3;
		keyColorSetups[3] = activeColorSetup.keyColorSetup4;
		ColorSetup.KeyColorSetup[] array = keyColorSetups;
		foreach (ColorSetup.KeyColorSetup keyColorSetup in array)
		{
			keyColorSetup.keyColor = GenerateKeyColor(keyColorSetup);
			keyColorSetup.keyColor = ApplyHSVOffsets(keyColorSetup.keyColor, keyColorSetup.hsvOffsetSettings);
			keyColorSetup.keyColor = ApplyHSVOVerwrites(keyColorSetup.keyColor, keyColorSetup.hsvOverwriteSettings);
		}
		foreach (ColorSetup.MaterialSetup materialSetup in activeColorSetup.materialSetups)
		{
			materialSetup.matColor = GenerateMixedColor(materialSetup.colorMixSettings);
			materialSetup.matColor = ApplyHSVOffsets(materialSetup.matColor, materialSetup.hsvOffsetSettings);
			materialSetup.matColor = ApplyHSVOVerwrites(materialSetup.matColor, materialSetup.hsvOverwriteSettings);
			materialSetup.targetMaterial.color = materialSetup.matColor;
			Material[] additionalTargets = materialSetup.additionalTargets;
			for (int i = 0; i < additionalTargets.Length; i++)
			{
				additionalTargets[i].color = materialSetup.matColor;
			}
		}
		foreach (ColorSetup.LightColorSetup lightColorSetup in activeColorSetup.lightColorSetups)
		{
			lightColorSetup.lightColor = GenerateMixedColor(lightColorSetup.colorMixSettings);
			lightColorSetup.lightColor = ApplyHSVOffsets(lightColorSetup.lightColor, lightColorSetup.hsvOffsetSettings);
			lightColorSetup.lightColor = ApplyHSVOVerwrites(lightColorSetup.lightColor, lightColorSetup.hsvOverwriteSettings);
			float cookieSize = UnityEngine.Random.Range(lightColorSetup.minMaxCookieSize.x, lightColorSetup.minMaxCookieSize.y);
			int[] targetLightIndeces = lightColorSetup.targetLightIndeces;
			foreach (int num in targetLightIndeces)
			{
				if ((bool)sceneLights[num])
				{
					sceneLights[num].color = lightColorSetup.lightColor;
					sceneLights[num].cookieSize = cookieSize;
				}
			}
		}
		if (bChangeFogColor)
		{
			GenerateFogColors();
			RenderSettings.fogStartDistance = UnityEngine.Random.Range(activeColorSetup.fogStartMinMax.x, activeColorSetup.fogStartMinMax.y);
			RenderSettings.fogEndDistance = UnityEngine.Random.Range(activeColorSetup.fogEndMinMax.x, activeColorSetup.fogEndMinMax.y);
			fogStartDistance = RenderSettings.fogStartDistance;
			fogEndDistance = RenderSettings.fogEndDistance;
		}
		if (bChangeAmbientColor)
		{
			Color inputColor = GenerateMixedColor(activeColorSetup.ambientColorSetup.colorMixSettings);
			inputColor = ApplyHSVOffsets(inputColor, activeColorSetup.ambientColorSetup.hsvOffsetSettings);
			inputColor = ApplyHSVOVerwrites(inputColor, activeColorSetup.ambientColorSetup.hsvOverwriteSettings);
			RenderSettings.ambientLight = inputColor;
			ambientLight = RenderSettings.ambientLight;
		}
		if ((bool)rainParticleParent)
		{
			activeColorSetup.UpdateRainParticleState(rainParticleParent);
		}
		if ((bool)snowParticleParent)
		{
			activeColorSetup.UpdateSnowParticleState(snowParticleParent);
		}
		if ((bool)activeColorSetup.DNCParameter)
		{
			DayNightCycle.Instance.ChangeParameters(activeColorSetup.DNCParameter);
		}
	}

	private void GenerateFogColors()
	{
		Color inputColor = GenerateMixedColor(activeColorSetup.fogColorSetup.colorMixSettings);
		inputColor = ApplyHSVOffsets(inputColor, activeColorSetup.fogColorSetup.hsvOffsetSettings);
		inputColor = (RenderSettings.fogColor = ApplyHSVOVerwrites(inputColor, activeColorSetup.fogColorSetup.hsvOverwriteSettings));
		fogColor = RenderSettings.fogColor;
		if ((bool)MainCamera.Cam)
		{
			MainCamera.Cam.backgroundColor = inputColor;
		}
	}

	[ContextMenu("Reset Material Colors")]
	public void ResetMaterialColors()
	{
		ColorSetup[] array = colorSetups;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (ColorSetup.MaterialSetup materialSetup in array[i].materialSetups)
			{
				if (materialSetup.targetMaterial != null)
				{
					materialSetup.matColor = materialSetup.originalColor;
					materialSetup.targetMaterial.color = materialSetup.matColor;
				}
				Material[] additionalTargets = materialSetup.additionalTargets;
				for (int j = 0; j < additionalTargets.Length; j++)
				{
					additionalTargets[j].color = materialSetup.originalColor;
				}
			}
		}
	}

	[ContextMenu("Store Material Colors")]
	public void StoreOriginalColors()
	{
		ColorSetup[] array = colorSetups;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (ColorSetup.MaterialSetup materialSetup in array[i].materialSetups)
			{
				if (materialSetup.targetMaterial != null)
				{
					materialSetup.originalColor = materialSetup.targetMaterial.color;
				}
				Material[] additionalTargets = materialSetup.additionalTargets;
				for (int j = 0; j < additionalTargets.Length; j++)
				{
					additionalTargets[j].color = materialSetup.originalColor;
				}
			}
		}
	}

	private void OnDestroy()
	{
		ResetMaterialColors();
	}

	public void ResetRenderSettings()
	{
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogStartDistance = fogStartDistance;
		RenderSettings.fogEndDistance = fogEndDistance;
		RenderSettings.fogDensity = fogDensity;
		if (RenderSettings.ambientMode == AmbientMode.Skybox)
		{
			RenderSettings.skybox.SetColor("_Tint", ambientSkyBoxColor);
			RenderSettings.ambientIntensity = ambientIntensity;
		}
		else
		{
			RenderSettings.ambientLight = ambientLight;
		}
		RenderSettings.ambientEquatorColor = ambientEquatorColor;
		RenderSettings.ambientGroundColor = ambientGroundColor;
		Shader.SetGlobalColor("_NightLightColor", nightLightColor);
		Shader.SetGlobalFloat("_NightCycleValue", nightCycleValue);
	}

	public void EnableBloom(bool value)
	{
		if (postProcess.profile.TryGetSettings<Bloom>(out var outSetting))
		{
			outSetting.active = value;
		}
	}

	public ColorSetup PickRandomColorSetup()
	{
		return colorSetups[UnityEngine.Random.Range(0, colorSetups.Length)];
	}

	private Color GenerateKeyColor(ColorSetup.KeyColorSetup keyColorSetup)
	{
		Color color = new Color(0f, 0f, 0f, 1f);
		return keyColorSetup.copyColorFrom switch
		{
			ColorSetup.KeyColorSelector.Random => UnityEngine.Random.ColorHSV(0f, 1f, 0f, 1f, 0f, 1f, 1f, 1f), 
			ColorSetup.KeyColorSelector.Fixed => keyColorSetup.fixedColor.fixedColor, 
			ColorSetup.KeyColorSelector.KeyColor1 => activeColorSetup.keyColorSetup1.keyColor, 
			ColorSetup.KeyColorSelector.KeyColor2 => activeColorSetup.keyColorSetup2.keyColor, 
			ColorSetup.KeyColorSelector.KeyColor3 => activeColorSetup.keyColorSetup3.keyColor, 
			ColorSetup.KeyColorSelector.KeyColor4 => activeColorSetup.keyColorSetup4.keyColor, 
			_ => color, 
		};
	}

	private Color GenerateMixedColor(ColorSetup.ColorMixSettings colorMixSettings)
	{
		Color color = new Color(0f, 0f, 0f, 1f);
		float num = colorMixSettings.amountKeyColor1 + colorMixSettings.amountKeyColor2 + colorMixSettings.amountKeyColor3 + colorMixSettings.amountKeyColor4 + colorMixSettings.amountFixedColor + colorMixSettings.amountCopyFromSource;
		if (num > 0f)
		{
			color += activeColorSetup.keyColorSetup1.keyColor * colorMixSettings.amountKeyColor1;
			color += activeColorSetup.keyColorSetup2.keyColor * colorMixSettings.amountKeyColor2;
			color += activeColorSetup.keyColorSetup3.keyColor * colorMixSettings.amountKeyColor3;
			color += activeColorSetup.keyColorSetup4.keyColor * colorMixSettings.amountKeyColor4;
			color += colorMixSettings.fixedColor * colorMixSettings.amountFixedColor;
			if ((bool)colorMixSettings.sourceMaterial)
			{
				color += colorMixSettings.sourceMaterial.color * colorMixSettings.amountCopyFromSource;
			}
			return color / num;
		}
		return color;
	}

	private Color ApplyHSVOffsets(Color inputColor, ColorSetup.HSVOffsetSetting offsetSettings)
	{
		Vector3 vector = RGBToHSV(inputColor);
		Vector2 rangeHueOffset = offsetSettings.rangeHueOffset;
		vector.x += UnityEngine.Random.Range(rangeHueOffset.x, rangeHueOffset.y);
		vector.x = Mathf.Repeat(vector.x, 1f);
		Vector2 rangeSaturationOffset = offsetSettings.rangeSaturationOffset;
		vector.y += UnityEngine.Random.Range(rangeSaturationOffset.x, rangeSaturationOffset.y);
		vector.y = Mathf.Clamp01(vector.y);
		Vector2 rangeValueOffset = offsetSettings.rangeValueOffset;
		vector.z += UnityEngine.Random.Range(rangeValueOffset.x, rangeValueOffset.y);
		vector.z = Mathf.Clamp01(vector.z);
		return Color.HSVToRGB(vector.x, vector.y, vector.z);
	}

	private Color ApplyHSVOVerwrites(Color inputColor, ColorSetup.HSVOverwriteSetting overwriteSettings)
	{
		Vector3 vector = RGBToHSV(inputColor);
		Vector2 rangeHueOverwrite = overwriteSettings.rangeHueOverwrite;
		if (rangeHueOverwrite.x != 0f || rangeHueOverwrite.y != 0f)
		{
			vector.x = UnityEngine.Random.Range(rangeHueOverwrite.x, rangeHueOverwrite.y);
			vector.x = Mathf.Repeat(vector.x, 1f);
		}
		Vector2 rangeSaturationOverwrite = overwriteSettings.rangeSaturationOverwrite;
		if (rangeSaturationOverwrite.x != 0f || rangeSaturationOverwrite.y != 0f)
		{
			vector.y = UnityEngine.Random.Range(rangeSaturationOverwrite.x, rangeSaturationOverwrite.y);
			vector.y = Mathf.Clamp01(vector.y);
		}
		Vector2 rangeValueOverwrite = overwriteSettings.rangeValueOverwrite;
		if (rangeValueOverwrite.x != 0f || rangeValueOverwrite.y != 0f)
		{
			vector.z = UnityEngine.Random.Range(rangeValueOverwrite.x, rangeValueOverwrite.y);
			vector.z = Mathf.Clamp01(vector.z);
		}
		return Color.HSVToRGB(vector.x, vector.y, vector.z);
	}

	private Vector3 RGBToHSV(Color rgbInputColor)
	{
		Color.RGBToHSV(rgbInputColor, out var H, out var S, out var V);
		return new Vector3(H, S, V);
	}

	private Color GenerateRandomColor()
	{
		float time = UnityEngine.Random.Range(0f, 1f);
		float s = UnityEngine.Random.Range(0f, 1f);
		float v = UnityEngine.Random.Range(0f, 1f);
		time = hueOffsetCurve.Evaluate(time);
		return Color.HSVToRGB(time, s, v);
	}
}

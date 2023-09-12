using System;
using System.Collections.Generic;
using SCS.Gameplay;
using UnityEngine;

[CreateAssetMenu(fileName = "Color Setup", menuName = "CUSTOM/ColorSetup", order = 2)]
public class ColorSetup : ScriptableObject
{
	[Serializable]
	public class HSVOverwriteSetting
	{
		[Header("HSV-Overwrites (0, 0 means no overwriting)")]
		public Vector2 rangeHueOverwrite;

		public Vector2 rangeSaturationOverwrite;

		public Vector2 rangeValueOverwrite;
	}

	[Serializable]
	public class HSVOffsetSetting
	{
		[Header("HSV-Offsets")]
		public Vector2 rangeHueOffset;

		public Vector2 rangeSaturationOffset;

		public Vector2 rangeValueOffset;
	}

	[Serializable]
	public class MaterialSetup
	{
		[HideInInspector]
		public string name;

		[HideInInspector]
		public Color matColor;

		public Material targetMaterial;

		public Material[] additionalTargets;

		[Header("Color Mix Settings")]
		public ColorMixSettings colorMixSettings;

		[Header("Color Offset Settings")]
		public HSVOffsetSetting hsvOffsetSettings;

		[Header("Color Overwrite Settings")]
		public HSVOverwriteSetting hsvOverwriteSettings;

		public CommentText comment;

		[HideInInspector]
		public Color originalColor;

		[HideInInspector]
		public Material lastTargetMat;
	}

	[Serializable]
	public class FixedColor
	{
		public Color fixedColor;
	}

	[Serializable]
	public class CommentText
	{
		[TextArea]
		public string comment;
	}

	[Serializable]
	public class KeyColorSetup
	{
		[HideInInspector]
		public string name;

		[HideInInspector]
		public Color keyColor;

		public FixedColor fixedColor;

		public KeyColorSelector copyColorFrom;

		public HSVOffsetSetting hsvOffsetSettings;

		[Header(" ")]
		public HSVOverwriteSetting hsvOverwriteSettings;

		public CommentText comment;
	}

	[Serializable]
	public class ColorMixSettings
	{
		[Range(0f, 1f)]
		public float amountKeyColor1;

		[Range(0f, 1f)]
		public float amountKeyColor2;

		[Range(0f, 1f)]
		public float amountKeyColor3;

		[Range(0f, 1f)]
		public float amountKeyColor4;

		[Range(0f, 1f)]
		public float amountFixedColor;

		[Range(0f, 1f)]
		public float amountCopyFromSource;

		public Color fixedColor;

		public Material sourceMaterial;
	}

	[Serializable]
	public class LightColorSetup
	{
		public string name;

		[HideInInspector]
		public Color lightColor;

		[Tooltip("These indeces should reference a Light in Color-Generetor's 'SceneLights'-List")]
		public int[] targetLightIndeces;

		[Header("Color Mix Settings")]
		public ColorMixSettings colorMixSettings;

		[Header("Color Offset Settings")]
		public HSVOffsetSetting hsvOffsetSettings;

		[Header("Color Overwrite Settings")]
		public HSVOverwriteSetting hsvOverwriteSettings;

		[Header("Light-Cookie Size")]
		public Vector2 minMaxCookieSize;

		public CommentText comment;
	}

	[Serializable]
	public class FogColorSetup
	{
		[Header("Color Mix Settings")]
		public ColorMixSettings colorMixSettings;

		[Header("Color Offset Settings")]
		public HSVOffsetSetting hsvOffsetSettings;

		[Header("Color Overwrite Settings")]
		public HSVOverwriteSetting hsvOverwriteSettings;

		public CommentText comment;
	}

	[Serializable]
	public class AmbientColorSetup
	{
		[Header("Color Mix Settings")]
		public ColorMixSettings colorMixSettings;

		[Header("Color Offset Settings")]
		public HSVOffsetSetting hsvOffsetSettings;

		[Header("Color Overwrite Settings")]
		public HSVOverwriteSetting hsvOverwriteSettings;

		public CommentText comment;
	}

	public enum KeyColorSelector
	{
		Random = 0,
		Fixed = 1,
		KeyColor1 = 2,
		KeyColor2 = 3,
		KeyColor3 = 4,
		KeyColor4 = 5
	}

	[Header("Key-Colors")]
	[Header("Key-Color 1")]
	public KeyColorSetup keyColorSetup1;

	[Header("Key-Color 2")]
	public KeyColorSetup keyColorSetup2;

	[Header("Key-Color 3")]
	public KeyColorSetup keyColorSetup3;

	[Header("Key-Color 4")]
	public KeyColorSetup keyColorSetup4;

	[Header("Material Setups")]
	public List<MaterialSetup> materialSetups;

	[Header("Water Foam Color")]
	[Tooltip("Assign a Material which gets the color of the waves here. The automatically generated water-edge will have its color.")]
	public Material waterEdgeReference;

	[Tooltip("Assign the material which creates the water-edge here.")]
	public Material waterPlaneMaterial;

	[Header("Light Color Setups")]
	public List<LightColorSetup> lightColorSetups;

	[Header("Fog Setup")]
	public FogColorSetup fogColorSetup;

	[Tooltip("Configure Fog Start & -End Settings here.")]
	public Vector2 fogStartMinMax;

	[Tooltip("Configure Fog Start & -End Settings here.")]
	public Vector2 fogEndMinMax;

	[Header("Ambient Light Color Setup")]
	public AmbientColorSetup ambientColorSetup;

	[Header("Weather Setup")]
	public bool enableRain;

	[Tooltip("How many Rain-Particles should the rain-effect spawn?")]
	public float rainIntensity;

	public bool enableSnow;

	[Range(0f, 1500f)]
	public float snowIntensity = 700f;

	[SerializeField]
	private DNCycleParameters dncycleParameter;

	public DNCycleParameters DNCParameter => dncycleParameter;

	public void OnValidate()
	{
		foreach (MaterialSetup materialSetup in materialSetups)
		{
			if (materialSetup.targetMaterial != null)
			{
				materialSetup.name = materialSetup.targetMaterial.name;
			}
			if ((bool)materialSetup.targetMaterial && (bool)materialSetup.lastTargetMat && materialSetup.targetMaterial != materialSetup.lastTargetMat)
			{
				materialSetup.originalColor = materialSetup.targetMaterial.color;
				materialSetup.lastTargetMat = materialSetup.targetMaterial;
			}
		}
	}

	public void UpdateSnowParticleState(GameObject parentSnow)
	{
		if (!enableSnow || enableRain)
		{
			parentSnow.SetActive(value: false);
			return;
		}
		parentSnow.SetActive(value: true);
		for (int i = 0; i < parentSnow.transform.childCount; i++)
		{
			UpdateSnowChildEmission(parentSnow, i);
		}
	}

	private void UpdateSnowChildEmission(GameObject parentSnow, int childIndex)
	{
		ParticleSystem.EmissionModule emission = parentSnow.transform.GetChild(childIndex).GetComponent<ParticleSystem>().emission;
		emission.rateOverTime = new ParticleSystem.MinMaxCurve(snowIntensity);
	}

	public void UpdateRainParticleState(GameObject rainParticleParent)
	{
		if (!enableRain)
		{
			rainParticleParent.SetActive(value: false);
			return;
		}
		rainParticleParent.SetActive(value: true);
		ParticleSystem component = rainParticleParent.transform.GetChild(1).GetComponent<ParticleSystem>();
		ParticleSystem.EmissionModule emission = component.emission;
		emission.rateOverTime = new ParticleSystem.MinMaxCurve(rainIntensity);
		rainParticleParent.transform.GetChild(0).GetComponent<ParticleSystem>();
		ParticleSystem.EmissionModule emission2 = component.emission;
		emission2.rateOverTime = new ParticleSystem.MinMaxCurve(rainIntensity / 10f);
	}
}

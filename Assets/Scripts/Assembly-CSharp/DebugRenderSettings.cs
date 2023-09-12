using UnityEngine;

public class DebugRenderSettings : MonoBehaviour
{
	private void Start()
	{
		DebugRender();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			DebugRender();
		}
	}

	private void DebugRender()
	{
		Debug.Log("----------- RENDER SETTINGS -------------");
		Debug.Log(RenderSettings.ambientEquatorColor);
		Debug.Log(RenderSettings.ambientGroundColor);
		Debug.Log(RenderSettings.ambientIntensity);
		Debug.Log("RenderSettings.ambientLight: " + RenderSettings.ambientLight.ToString());
		Debug.Log(RenderSettings.ambientMode);
		Debug.Log(RenderSettings.ambientProbe);
		Debug.Log("RenderSettings.ambientSkyColor: " + RenderSettings.ambientSkyColor.ToString());
		Debug.Log(RenderSettings.customReflection);
		Debug.Log(RenderSettings.defaultReflectionMode);
		Debug.Log(RenderSettings.defaultReflectionResolution);
		Debug.Log(RenderSettings.flareFadeSpeed);
		Debug.Log(RenderSettings.flareStrength);
		Debug.Log(RenderSettings.fog);
		Debug.Log("RenderSettings.fogColor: " + RenderSettings.fogColor.ToString());
		Debug.Log(RenderSettings.fogDensity);
		Debug.Log("RenderSettings.fogEndDistance: " + RenderSettings.fogEndDistance);
		Debug.Log(RenderSettings.fogMode);
		Debug.Log("RenderSettings.fogStartDistance: " + RenderSettings.fogStartDistance);
		Debug.Log(RenderSettings.haloStrength);
		Debug.Log(RenderSettings.reflectionBounces);
		Debug.Log(RenderSettings.reflectionIntensity);
		Debug.Log("RenderSettings.skybox: " + RenderSettings.skybox);
		Debug.Log(RenderSettings.subtractiveShadowColor);
		Debug.Log(RenderSettings.sun);
		Debug.Log("----------- RENDER SETTINGS -------------");
	}
}

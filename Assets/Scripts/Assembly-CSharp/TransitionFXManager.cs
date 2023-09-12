using System.Collections;
using UnityEngine;

public class TransitionFXManager : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem[] partclClouds;

	[SerializeField]
	private ParticleSystem[] partclFog;

	private void Start()
	{
	}

	private void StartTransitionFX()
	{
		ParticleSystem[] array = partclClouds;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		StartCoroutine(StopAfterSeconds(5f));
	}

	private IEnumerator StopAfterSeconds(float delay)
	{
		yield return new WaitForSeconds(delay);
		StopTransitionFX();
	}

	private void StopTransitionFX()
	{
		ParticleSystem[] array = partclClouds;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
	}
}

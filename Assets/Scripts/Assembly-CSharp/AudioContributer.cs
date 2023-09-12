using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioContributer : MonoBehaviour
{
	[Serializable]
	public class BuildingSoundSetup
	{
		[Tooltip("Which sound should this contribute to?")]
		public AudioManager.BuildingSounds buildingSound;

		public float contributionFactor = 1f;
	}

	public List<BuildingSoundSetup> soundSetups;

	private void OnEnable()
	{
		if ((bool)AudioManager.singleton)
		{
			AudioManager.singleton.audioContributorsInGame.Add(this);
		}
	}

	private void OnDisable()
	{
		if ((bool)AudioManager.singleton && AudioManager.singleton.audioContributorsInGame.Contains(this))
		{
			AudioManager.singleton.audioContributorsInGame.Remove(this);
		}
	}

	private void Reset()
	{
		soundSetups = new List<BuildingSoundSetup>
		{
			new BuildingSoundSetup()
		};
	}
}

using System;
using UnityEngine;

[Serializable]
public class LookAtReferenceData
{
	public Transform transformReference;

	public string identifierReference;

	public float YRotationOffset;

	public float YRotationVariation;

	public bool iUseIdentifier;
}

using System;
using UnityEngine;

[Serializable]
public class SeriVector3
{
	public float fX;

	public float fY;

	public float fZ;

	public Vector3 V3
	{
		get
		{
			return new Vector3(fX, fY, fZ);
		}
		set
		{
			fX = value.x;
			fY = value.y;
			fZ = value.z;
		}
	}

	public SeriVector3()
	{
	}

	public SeriVector3(Vector3 _v3)
	{
		V3 = _v3;
	}
}

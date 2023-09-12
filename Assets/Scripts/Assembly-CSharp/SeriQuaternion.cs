using System;
using UnityEngine;

[Serializable]
public class SeriQuaternion
{
	public float fX;

	public float fY;

	public float fZ;

	public float fW;

	public Quaternion Q
	{
		get
		{
			return new Quaternion(fX, fY, fZ, fW);
		}
		set
		{
			fX = value.x;
			fY = value.y;
			fZ = value.z;
			fW = value.w;
		}
	}

	public SeriQuaternion()
	{
	}

	public SeriQuaternion(Quaternion _q)
	{
		Q = _q;
	}
}

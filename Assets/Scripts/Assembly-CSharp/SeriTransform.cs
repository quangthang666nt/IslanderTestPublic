using System;
using UnityEngine;

[Serializable]
public class SeriTransform
{
	public SeriVector3 sv3Position;

	public SeriVector3 sv3LocalScale;

	public SeriQuaternion sqRotation;

	public int iVariation;

	public SeriTransform(float pX, float pY, float pZ, float sX, float sY, float sZ, float rX, float rY, float rZ, float rW, int variation)
	{
		sv3Position = new SeriVector3(new Vector3(pX, pY, pZ));
		sv3LocalScale = new SeriVector3(new Vector3(sX, sY, sZ));
		sqRotation = new SeriQuaternion(new Quaternion(rX, rY, rZ, rW));
		iVariation = variation;
	}

	public SeriTransform()
	{
		sv3Position = new SeriVector3();
		sv3Position = new SeriVector3();
		sqRotation = new SeriQuaternion();
		iVariation = 0;
	}

	public SeriTransform(Transform _trans, int _iVariation = 0)
	{
		sv3Position = new SeriVector3(_trans.position);
		sv3LocalScale = new SeriVector3(_trans.localScale);
		sqRotation = new SeriQuaternion(_trans.rotation);
		iVariation = _iVariation;
	}

	public SeriTransform(Transform _trans, Vector3 _v3Offset)
	{
		sv3Position = new SeriVector3(_trans.position + _v3Offset);
		sv3LocalScale = new SeriVector3(_trans.localScale);
		sqRotation = new SeriQuaternion(_trans.rotation);
		iVariation = 0;
	}

	public Vector3 V3GetPosition()
	{
		return new Vector3(sv3Position.fX, sv3Position.fY, sv3Position.fZ);
	}

	public Quaternion QGetRotation()
	{
		return new Quaternion(sqRotation.fX, sqRotation.fY, sqRotation.fZ, sqRotation.fW);
	}

	public Vector3 V3GetScale()
	{
		return new Vector3(sv3LocalScale.fX, sv3LocalScale.fY, sv3LocalScale.fZ);
	}
}

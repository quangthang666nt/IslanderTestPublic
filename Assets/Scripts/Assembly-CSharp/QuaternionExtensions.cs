using UnityEngine;

public static class QuaternionExtensions
{
	public static Vector3 Forward(this Quaternion rotation)
	{
		return rotation * Vector3.forward;
	}

	public static Vector3 Right(this Quaternion rotation)
	{
		return rotation * Vector3.right;
	}

	public static Vector3 Up(this Quaternion rotation)
	{
		return rotation * Vector3.up;
	}

	public static Quaternion Add(this Quaternion first, Quaternion second)
	{
		first.w += second.w;
		first.x += second.x;
		first.y += second.y;
		first.z += second.z;
		return first;
	}

	public static Quaternion Scale(this Quaternion first, float multiplier)
	{
		first.w *= multiplier;
		first.x *= multiplier;
		first.y *= multiplier;
		first.z *= multiplier;
		return first;
	}
}

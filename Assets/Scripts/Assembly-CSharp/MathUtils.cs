using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class MathUtils
{
	public static float SqrtRetainSign(float x)
	{
		return Mathf.Sqrt(Mathf.Abs(x)) * SignExcludingZero(x);
	}

	public static bool SolveQuadraticMax(float a, float b, float c, ref float res)
	{
		float num = b * b - 4f * a * c;
		if (a * a > 1E-06f)
		{
			if (num > 0f)
			{
				float num2 = Mathf.Sqrt(num);
				float num3 = 1f / (2f * a);
				float a2 = (0f - b + num2) * num3;
				float b2 = (0f - b - num2) * num3;
				res = Mathf.Max(a2, b2);
				return true;
			}
			if (Math.Abs(num) < 1E-05f)
			{
				res = (0f - b) / (2f * a);
				return true;
			}
		}
		else
		{
			if (b * b > 1E-06f)
			{
				res = (0f - c) / b;
				return true;
			}
			if (Math.Abs(c) < 1E-05f)
			{
				res = 0f;
				return true;
			}
		}
		return false;
	}

	public static bool SolveQuadraticMaxAboveZero(float a, float b, float c, ref float res)
	{
		float num = b * b - 4f * a * c;
		if (a * a > 1E-06f)
		{
			if (num > 0f)
			{
				float num2 = Mathf.Sqrt(num);
				float num3 = 1f / (2f * a);
				float a2 = (0f - b + num2) * num3;
				float b2 = (0f - b - num2) * num3;
				res = Mathf.Max(a2, b2);
				return res >= 0f;
			}
			if (Math.Abs(num) < 1E-05f)
			{
				res = (0f - b) / (2f * a);
				return res >= 0f;
			}
		}
		else
		{
			if (b * b > 1E-06f)
			{
				res = (0f - c) / b;
				return res >= 0f;
			}
			if (Math.Abs(c) < 1E-05f)
			{
				res = 0f;
				return true;
			}
		}
		return false;
	}

	public static bool SolveQuadraticMin(float a, float b, float c, ref float res)
	{
		float num = b * b - 4f * a * c;
		if (a * a > 1E-06f)
		{
			if (num > 0f)
			{
				float num2 = Mathf.Sqrt(num);
				float num3 = 1f / (2f * a);
				float a2 = (0f - b + num2) * num3;
				float b2 = (0f - b - num2) * num3;
				res = Mathf.Min(a2, b2);
				return true;
			}
			if (Math.Abs(num) < 1E-05f)
			{
				res = (0f - b) / (2f * a);
				return true;
			}
		}
		else
		{
			if (b * b > 1E-06f)
			{
				res = (0f - c) / b;
				return true;
			}
			if (Math.Abs(c) < 1E-05f)
			{
				res = 0f;
				return true;
			}
		}
		return false;
	}

	public static bool SolveQuadratic(float a, float b, float c, ref float resLow, ref float resHigh)
	{
		float num = b * b - 4f * a * c;
		if (a * a > 1E-06f)
		{
			if (num > 0f)
			{
				float num2 = Mathf.Sqrt(num);
				float num3 = 1f / (2f * a);
				float a2 = (0f - b + num2) * num3;
				float b2 = (0f - b - num2) * num3;
				resHigh = Mathf.Max(a2, b2);
				resLow = Mathf.Min(a2, b2);
				return true;
			}
			if (Math.Abs(num) < 1E-05f)
			{
				resHigh = (resLow = (0f - b) / (2f * a));
				return true;
			}
		}
		else
		{
			if (b * b > 1E-06f)
			{
				resHigh = (resLow = (0f - c) / b);
				return true;
			}
			if (Math.Abs(c) < 1E-05f)
			{
				resHigh = (resLow = 0f);
				return true;
			}
		}
		return false;
	}

	public static int SolveCubic(float a, float b, float c, float d, ref float res1, ref float res2, ref float res3)
	{
		if (a == 0f)
		{
			if (SolveQuadratic(b, c, d, ref res1, ref res2))
			{
				return 2;
			}
			return 0;
		}
		b /= a;
		c /= a;
		d /= a;
		float num = (3f * c - b * b) / 9f;
		float num2 = 0f - 27f * d + b * (9f * c - 2f * (b * b));
		num2 /= 54f;
		float num3 = num * num * num + num2 * num2;
		float num4 = b / 3f;
		res1 = 0f;
		if (num3 > 0f)
		{
			float num5 = num2 + Mathf.Sqrt(num3);
			num5 = ((num5 < 0f) ? (0f - Mathf.Pow(0f - num5, 1f / 3f)) : Mathf.Pow(num5, 1f / 3f));
			float num6 = num2 - Mathf.Sqrt(num3);
			num6 = ((num6 < 0f) ? (0f - Mathf.Pow(0f - num6, 1f / 3f)) : Mathf.Pow(num6, 1f / 3f));
			res1 = 0f - num4 + num5 + num6;
			return 1;
		}
		float num7;
		if (num3 == 0f)
		{
			num7 = ((num2 < 0f) ? (0f - Mathf.Pow(0f - num2, 1f / 3f)) : Mathf.Pow(num2, 1f / 3f));
			res1 = 0f - num4 + 2f * num7;
			res2 = 0f - (num7 + num4);
			res3 = res2;
			return 2;
		}
		num = 0f - num;
		float f = num * num * num;
		f = Mathf.Acos(num2 / Mathf.Sqrt(f));
		num7 = 2f * Mathf.Sqrt(num);
		res1 = 0f - num4 + num7 * Mathf.Cos(f / 3f);
		res2 = 0f - num4 + num7 * Mathf.Cos((f + MathF.PI * 2f) / 3f);
		res3 = 0f - num4 + num7 * Mathf.Cos((f + MathF.PI * 4f) / 3f);
		return 3;
	}

	public static bool SolveCubicMax(float a, float b, float c, float d, ref float result)
	{
		float res = 0f;
		float res2 = 0f;
		float res3 = 0f;
		switch (SolveCubic(a, b, c, d, ref res, ref res2, ref res3))
		{
		case 3:
			result = Mathf.Max(res, Mathf.Max(res2, res3));
			return true;
		case 2:
			result = Mathf.Max(res, res2);
			return true;
		case 1:
			result = res;
			return true;
		default:
			return false;
		}
	}

	public static bool SolveCubicMin(float a, float b, float c, float d, ref float result)
	{
		float res = 0f;
		float res2 = 0f;
		float res3 = 0f;
		switch (SolveCubic(a, b, c, d, ref res, ref res2, ref res3))
		{
		case 3:
			result = Mathf.Min(res, Mathf.Min(res2, res3));
			return true;
		case 2:
			result = Mathf.Min(res, res2);
			return true;
		case 1:
			result = res;
			return true;
		default:
			return false;
		}
	}

	public static bool SolveCubicMinAboveZero(float a, float b, float c, float d, ref float result)
	{
		float res = 0f;
		float res2 = 0f;
		float res3 = 0f;
		switch (SolveCubic(a, b, c, d, ref res, ref res2, ref res3))
		{
		case 3:
		{
			float num2 = Mathf.Max(res, Mathf.Max(res2, res3));
			if (num2 < 0f)
			{
				return false;
			}
			if (res < 0f)
			{
				res = num2;
			}
			if (res2 < 0f)
			{
				res2 = num2;
			}
			if (res3 < 0f)
			{
				res3 = num2;
			}
			result = Mathf.Min(res, Mathf.Min(res2, res3));
			return true;
		}
		case 2:
		{
			float num = Mathf.Max(res, res2);
			if (num < 0f)
			{
				return false;
			}
			if (res < 0f)
			{
				res = num;
			}
			if (res2 < 0f)
			{
				res2 = num;
			}
			result = Mathf.Min(res, res2);
			return true;
		}
		case 1:
			if (res < 0f)
			{
				return false;
			}
			result = res;
			return true;
		default:
			return false;
		}
	}

	public static void ForceMinimumLength(ref Vector3 vect, float minLength)
	{
		if (vect.sqrMagnitude < minLength * minLength)
		{
			vect = vect.normalized * minLength;
		}
	}

	public static void LimitLength(ref Vector3 vect, float maxLength)
	{
		if (vect.sqrMagnitude > maxLength * maxLength)
		{
			vect = vect.normalized * maxLength;
		}
	}

	public static void LimitLength(ref Vector2 vect, float maxLength)
	{
		if (vect.sqrMagnitude > maxLength * maxLength)
		{
			vect = vect.normalized * maxLength;
		}
	}

	public static Vector3 RetainSignAndSquare(Vector3 vect)
	{
		float sqrMagnitude = vect.sqrMagnitude;
		if (sqrMagnitude > 0f)
		{
			return vect.normalized * sqrMagnitude;
		}
		return Vector3.zero;
	}

	public static Vector3 RetainSignAndSquareRoot(Vector3 vect)
	{
		float magnitude = vect.magnitude;
		if (magnitude > 0f)
		{
			return vect.normalized * Mathf.Sqrt(magnitude);
		}
		return Vector3.zero;
	}

	public static Vector3 SafeNormalize(Vector3 vec, Vector3 fallback)
	{
		if (vec.sqrMagnitude > 0.0001f)
		{
			return vec.normalized;
		}
		return fallback;
	}

	public static Vector3 SafeNormalize(Vector3 vec)
	{
		if (vec.sqrMagnitude > 0.0001f)
		{
			return vec.normalized;
		}
		return Vector3.zero;
	}

	public static Quaternion SafeLookRotation(Vector3 fwd)
	{
		return SafeLookRotation(fwd, Vector3.up);
	}

	public static Quaternion SafeLookRotation(Vector3 fwd, Vector3 up)
	{
		Quaternion result = Quaternion.identity;
		if (up.sqrMagnitude > 0.0001f && fwd.sqrMagnitude > 0.0001f)
		{
			fwd = fwd.normalized;
			up = up.normalized;
			if (Vector3.Dot(up, fwd) < 0.9999f)
			{
				result = Quaternion.LookRotation(fwd, up);
			}
		}
		return result;
	}

	public static Quaternion SafeLookRotation(Vector3 fwd, Vector3 up, Quaternion fallback)
	{
		Quaternion result = fallback;
		if (up.sqrMagnitude > 0.0001f && fwd.sqrMagnitude > 0.0001f)
		{
			fwd = fwd.normalized;
			up = up.normalized;
			if (Vector3.Dot(up, fwd) < 0.9999f)
			{
				result = Quaternion.LookRotation(fwd, up);
			}
		}
		return result;
	}

	public static float AbsDifferenceOverSum(float a, float b)
	{
		return Mathf.Abs(a - b) / (a + b);
	}

	public static float AngleRangeRads(Vector3 v0, Vector3 v1, Vector3 v2)
	{
		float a = MathF.PI;
		Vector3 normalized = (v1 - v0).normalized;
		Vector3 normalized2 = (v2 - v1).normalized;
		Vector3 normalized3 = (v0 - v2).normalized;
		float b = Vector3.Angle(normalized2, normalized) * (MathF.PI / 180f);
		float b2 = Vector3.Angle(normalized3, normalized2) * (MathF.PI / 180f);
		float b3 = Vector3.Angle(normalized, normalized3) * (MathF.PI / 180f);
		a = Mathf.Min(a, b);
		a = Mathf.Min(a, b2);
		a = Mathf.Min(a, b3);
		return Mathf.Max(Mathf.Max(Mathf.Max(-MathF.PI, b), b2), b3) - a;
	}

	public static float FlatnessScore(Vector3 v0, Vector3 v1, Vector3 v2)
	{
		float a = MathF.PI;
		Vector3 normalized = (v1 - v0).normalized;
		Vector3 normalized2 = (v2 - v1).normalized;
		Vector3 normalized3 = (v0 - v2).normalized;
		float b = Vector3.Angle(normalized2, normalized) * (MathF.PI / 180f);
		float b2 = Vector3.Angle(normalized3, normalized2) * (MathF.PI / 180f);
		float b3 = Vector3.Angle(normalized, normalized3) * (MathF.PI / 180f);
		a = Mathf.Min(a, b);
		a = Mathf.Min(a, b2);
		a = Mathf.Min(a, b3);
		return (Mathf.Max(Mathf.Max(Mathf.Max(-MathF.PI, b), b2), b3) - a) / MathF.PI;
	}

	public static bool IsConvexQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector3 normal)
	{
		Vector3 vector = v1 - v0;
		Vector3 vector2 = v2 - v1;
		Vector3 vector3 = v3 - v2;
		Vector3 vector4 = v0 - v3;
		if (Vector3.Dot(-Vector3.Cross(vector2, vector), normal) < 0f)
		{
			return false;
		}
		if (Vector3.Dot(-Vector3.Cross(vector3, vector2), normal) < 0f)
		{
			return false;
		}
		if (Vector3.Dot(-Vector3.Cross(vector4, vector3), normal) < 0f)
		{
			return false;
		}
		if (Vector3.Dot(-Vector3.Cross(vector, vector4), normal) < 0f)
		{
			return false;
		}
		return true;
	}

	public static float ShortestEdgeLength(Vector3 v0, Vector3 v1, Vector3 v2)
	{
		float sqrMagnitude = (v1 - v0).sqrMagnitude;
		float sqrMagnitude2 = (v2 - v1).sqrMagnitude;
		float sqrMagnitude3 = (v0 - v2).sqrMagnitude;
		float num = ((sqrMagnitude < sqrMagnitude2) ? sqrMagnitude : sqrMagnitude2);
		num = ((num < sqrMagnitude3) ? num : sqrMagnitude3);
		return Mathf.Sqrt(num);
	}

	public static float TriangleArea(Vector3 v0, Vector3 v1, Vector3 v2)
	{
		Vector3 vector = v1 - v0;
		Vector3 vector2 = v1 - v2;
		Vector3 vector3 = Vector3.Cross(vector.normalized, vector2.normalized);
		return vector.magnitude * vector2.magnitude * 0.5f * vector3.magnitude;
	}

	public static Vector3 TriangleNormal(Vector3 v0, Vector3 v1, Vector3 v2)
	{
		Vector3 normalized = (v1 - v0).normalized;
		return Vector3.Cross((v1 - v2).normalized, normalized).normalized;
	}

	public static bool TriangleIntersection(Vector3 rayStart, Vector3 rayEnd, Vector3 v0, Vector3 v1, Vector3 v2, ref Vector3 result)
	{
		Vector3 vector = rayEnd - rayStart;
		Vector3 normalized = vector.normalized;
		Vector3 vector2 = v1 - v0;
		Vector3 vector3 = v2 - v0;
		Vector3 rhs = Vector3.Cross(normalized, vector3);
		float num = Vector3.Dot(vector2, rhs);
		if (num > 0f - Mathf.Epsilon && num < Mathf.Epsilon)
		{
			return false;
		}
		float num2 = 1f / num;
		Vector3 lhs = rayStart - v0;
		float num3 = Vector3.Dot(lhs, rhs) * num2;
		if (num3 < 0f || num3 > 1f)
		{
			return false;
		}
		Vector3 rhs2 = Vector3.Cross(lhs, vector2);
		float num4 = Vector3.Dot(normalized, rhs2) * num2;
		if (num4 < 0f || num3 + num4 > 1f)
		{
			return false;
		}
		if (Vector3.Dot(vector3, rhs2) * num2 > Mathf.Epsilon)
		{
			float num5 = 1f - (num3 + num4);
			result = v0 * num3 + v1 * num4 + v2 * num5;
			if ((result - rayStart).sqrMagnitude <= vector.sqrMagnitude)
			{
				return true;
			}
		}
		return false;
	}

	public static float GetAdjustmentByPlaneAlongAxis(Vector3 offset, Vector3 axis, Vector3 planeNormal)
	{
		float num = Vector3.Dot(axis, planeNormal);
		return ProjectOntoPlane(offset, planeNormal).magnitude / num;
	}

	public static Vector3 AdjustByPlaneAlongAxis(Vector3 offset, Vector3 axis, Vector3 planeNormal)
	{
		float num = Vector3.Dot(axis, planeNormal);
		float num2 = ProjectOntoPlane(offset, planeNormal).magnitude / num;
		return offset + axis * num2;
	}

	public static Vector3 ProjectOntoPlaneIfBelow(Vector3 line, Vector3 planeNormal)
	{
		if (Vector3.Dot(line, planeNormal) < 0f)
		{
			return line - planeNormal * Vector3.Dot(line, planeNormal);
		}
		return line;
	}

	public static Vector3 ProjectOntoPlaneIfAbove(Vector3 line, Vector3 planeNormal)
	{
		if (Vector3.Dot(line, planeNormal) > 0f)
		{
			return line - planeNormal * Vector3.Dot(line, planeNormal);
		}
		return line;
	}

	public static Vector3 ProjectOntoPlane(Vector3 line, Vector3 planeNormal)
	{
		return line - planeNormal * Vector3.Dot(line, planeNormal);
	}

	public static Vector3 ProjectOntoDirection(Vector3 input, Vector3 directionNormal)
	{
		return directionNormal * Vector3.Dot(input, directionNormal);
	}

	public static Vector3 NearpointOnLine(Vector3 p0, Vector3 p1, Vector3 test)
	{
		Vector3 vector = p1;
		test -= p0;
		vector -= p0;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = 0f;
		if (sqrMagnitude > 0f)
		{
			num = Mathf.Clamp01(Vector3.Dot(test, vector) / sqrMagnitude);
		}
		return vector * num + p0;
	}

	public static float NearpointOnLine(Vector3 p0, Vector3 p1, Vector3 test, out Vector3 res)
	{
		Vector3 vector = p1;
		test -= p0;
		vector -= p0;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = 0f;
		if (sqrMagnitude > 0f)
		{
			num = Mathf.Clamp01(Vector3.Dot(test, vector) / sqrMagnitude);
		}
		res = vector * num + p0;
		return num;
	}

	public static Vector3 Barycentric(Vector3 test, Vector3 p0, Vector3 p1, Vector3 p2)
	{
		Vector3 vector = p1 - p0;
		Vector3 vector2 = p2 - p0;
		Vector3 lhs = test - p0;
		float num = Vector3.Dot(vector, vector);
		float num2 = Vector3.Dot(vector, vector2);
		float num3 = Vector3.Dot(vector2, vector2);
		float num4 = Vector3.Dot(lhs, vector);
		float num5 = Vector3.Dot(lhs, vector2);
		float num6 = num * num3 - num2 * num2;
		float num7 = (num3 * num4 - num2 * num5) / num6;
		float num8 = (num * num5 - num2 * num4) / num6;
		return new Vector3(1f - num7 - num8, num7, num8);
	}

	public static float Frac(float value)
	{
		return value - Mathf.Floor(value);
	}

	public static float SolveMotionForDistance(float vel, float acc, float time)
	{
		float num = time * time;
		return vel * time + acc * num * 0.5f;
	}

	public static Vector3 SolveMotionForDistance(Vector3 vel, Vector3 acc, float time)
	{
		float num = time * time;
		return vel * time + acc * num * 0.5f;
	}

	public static Vector3 SolveMotionForDistance(Vector3 vel, Vector3 acc, Vector3 jolt, float time)
	{
		float num = time * time;
		float num2 = num * time;
		return vel * time + acc * num * 0.5f + jolt * num2 / 6f;
	}

	public static float SolveMotionForAcceleration(float vel, float distance, float time)
	{
		float num = time * time;
		return 2f * ((distance - vel * time) / num);
	}

	public static Vector3 SolveMotionForAcceleration(Vector3 vel, Vector3 distance, float time)
	{
		float num = time * time;
		return 2f * ((distance - vel * time) / num);
	}

	public static float SolveMotionForVelocity(float acceleration, float distance)
	{
		return Mathf.Sqrt(2f * Mathf.Abs(acceleration) * distance);
	}

	public static Vector3 SolveMotionForVelocity(Vector3 acceleration, Vector3 distance)
	{
		return SqrtVector(2f * MultiplyVectors(AbsVector(acceleration), distance));
	}

	public static float SolveMotionForVelocity(float currVelocity, float acceleration, float distance)
	{
		return Mathf.Sqrt(currVelocity * currVelocity + 2f * Mathf.Abs(acceleration) * distance);
	}

	public static Vector3 SolveMotionForVelocity(Vector3 currVelocity, Vector3 acceleration, Vector3 distance)
	{
		return SqrtVector(MultiplyVectors(currVelocity, currVelocity) + 2f * MultiplyVectors(AbsVector(acceleration), distance));
	}

	public static Vector3 MultiplyVectors(Vector3 vec1, Vector3 vec2)
	{
		return new Vector3(vec1.x * vec2.x, vec1.y * vec2.y, vec1.z * vec2.z);
	}

	public static Vector3 AbsVector(Vector3 vector)
	{
		return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
	}

	public static Vector3 SqrtVector(Vector3 vector)
	{
		return new Vector3(Mathf.Sqrt(vector.x), Mathf.Sqrt(vector.y), Mathf.Sqrt(vector.z));
	}

	public static Vector3 FixIfNaNOrInfinite(Vector3 v)
	{
		if (float.IsNaN(v.x) || float.IsInfinity(v.x))
		{
			v.x = 0f;
		}
		if (float.IsNaN(v.y) || float.IsInfinity(v.y))
		{
			v.y = 0f;
		}
		if (float.IsNaN(v.z) || float.IsInfinity(v.z))
		{
			v.z = 0f;
		}
		return v;
	}

	public static Vector3 TorqueToTurnTo(Transform subject, Transform target, Vector3 relAngVel, float extrapolation)
	{
		return TorqueToTurnTo(subject, target.rotation, relAngVel, extrapolation);
	}

	public static Vector3 TorqueToTurnTo(Transform subject, Quaternion target, Vector3 relAngVel, float extrapolation)
	{
		Quaternion quaternion = Quaternion.identity;
		Vector3 a = Vector3.one;
		if ((bool)subject.GetComponent<Rigidbody>())
		{
			quaternion = subject.GetComponent<Rigidbody>().inertiaTensorRotation;
			a = subject.GetComponent<Rigidbody>().inertiaTensor;
		}
		Quaternion quaternion2 = subject.transform.rotation * quaternion;
		Vector3 vector = target * Vector3.forward;
		Vector3 vector2 = target * Vector3.up;
		Vector3 vector3 = subject.forward + Vector3.Cross(relAngVel, subject.forward) * extrapolation;
		Vector3 vector4 = vector - Vector3.Cross(relAngVel, vector) * extrapolation;
		Vector3 vector5 = Vector3.Cross(vector3.normalized, vector4.normalized);
		float num = Mathf.Asin(vector5.magnitude);
		Vector3 vector6 = vector5.normalized * num / Time.fixedDeltaTime;
		Vector3 vector7 = quaternion2 * Vector3.Scale(a, Quaternion.Inverse(quaternion2) * vector6);
		vector3 = subject.up + Vector3.Cross(relAngVel, subject.up) * extrapolation;
		vector4 = vector2 - Vector3.Cross(relAngVel, vector2) * extrapolation;
		Vector3 vector8 = Vector3.Cross(vector3.normalized, vector4.normalized);
		num = Mathf.Asin(vector8.magnitude);
		vector6 = vector8.normalized * num / Time.fixedDeltaTime;
		return FixIfNaNOrInfinite(vector7 + quaternion2 * Vector3.Scale(a, Quaternion.Inverse(quaternion2) * vector6));
	}

	public static float CalculatePendulumPerdiod(float length, float oscillationAngle, float gravity)
	{
		float num = oscillationAngle * oscillationAngle;
		float num2 = num * num;
		float num3 = 0.0625f;
		float num4 = 0.0035807292f;
		return MathF.PI * 2f * Mathf.Sqrt(length / gravity) * (1f + num3 * num + num4 * num2);
	}

	public static float CalcMinAngleDif(float startAng, float endAng)
	{
		if (Mathf.Abs(endAng - startAng) > MathF.PI * 2f)
		{
			startAng %= MathF.PI * 2f;
			endAng %= MathF.PI * 2f;
			if (startAng > MathF.PI)
			{
				startAng -= MathF.PI * 2f;
			}
			if (endAng > MathF.PI)
			{
				endAng -= MathF.PI * 2f;
			}
			if (startAng < -MathF.PI)
			{
				startAng += MathF.PI * 2f;
			}
			if (endAng < -MathF.PI)
			{
				endAng += MathF.PI * 2f;
			}
		}
		float num;
		float num2;
		if (endAng > startAng)
		{
			num = endAng - startAng;
			num2 = endAng - MathF.PI * 2f - startAng;
		}
		else
		{
			num = endAng + MathF.PI * 2f - startAng;
			num2 = endAng - startAng;
		}
		if (num < 0f - num2)
		{
			return num;
		}
		return num2;
	}

	public static void CalcAnglesFromDir(Vector3 source, ref float angY, ref float angX)
	{
		Vector3 vector = new Vector3(source.x, 0f, source.z);
		Vector3 vector2 = source;
		vector = vector.normalized;
		vector2 = vector2.normalized;
		angY = Mathf.Acos(vector.z);
		angX = Mathf.Asin(vector2.y);
		if (vector.x < 0f)
		{
			angY = 0f - angY;
		}
	}

	public static void CalcDirFromAngles(ref Vector3 dest, float angY, float angX)
	{
		dest.y = Mathf.Sin(angX);
		float num = Mathf.Sqrt(1f - dest.y * dest.y);
		dest.x = Mathf.Sin(angY);
		dest.z = Mathf.Cos(angY);
		dest.x *= num;
		dest.z *= num;
		dest = dest.normalized;
	}

	public static Vector3 CalcDirFromAngles(float angY, float angX)
	{
		Vector3 zero = Vector3.zero;
		zero.y = Mathf.Sin(angX);
		float num = Mathf.Sqrt(1f - zero.y * zero.y);
		zero.x = Mathf.Sin(angY);
		zero.z = Mathf.Cos(angY);
		zero.x *= num;
		zero.z *= num;
		return zero.normalized;
	}

	public static Vector3 ClosestPointOnLine(Vector3 point1, Vector3 point2, Vector3 testPoint)
	{
		Vector3 lhs = testPoint - point1;
		Vector3 normalized = (point2 - point1).normalized;
		float max = Vector3.Distance(point1, point2);
		return point1 + Mathf.Clamp(Vector3.Dot(lhs, normalized), 0f, max) * normalized;
	}

	public static float GetFitness(Vector3 testPos, Vector3 sourceLocation, Vector3 sourceDirection, float limitAngle, float cosLimitAngle, float limitRange)
	{
		Vector3 vector = testPos - sourceLocation;
		float sqrMagnitude = vector.sqrMagnitude;
		float num = limitRange * limitRange;
		if (sqrMagnitude < num)
		{
			float num2 = Vector3.Dot(vector.normalized, sourceDirection);
			if (num2 >= cosLimitAngle)
			{
				float num3 = 1f - sqrMagnitude / num;
				float num4 = Mathf.Acos(num2);
				float num5 = Mathf.Min(1f, num4 / limitAngle);
				return Mathf.Sqrt(Mathf.Max(0f, 1f - num5 * num5)) * num3;
			}
		}
		return 0f;
	}

	public static float GetFitness(Vector3 testPos, Transform source, float limitAngle, float cosLimitAngle, float limitRange)
	{
		return GetFitness(testPos, source.position, source.forward, limitAngle, cosLimitAngle, limitRange);
	}

	public static float MaxByMagnitude(float x, float y)
	{
		if (Mathf.Abs(x) > Mathf.Abs(y))
		{
			return x;
		}
		return y;
	}

	public static float SignExcludingZero(float val)
	{
		if (val > 0f)
		{
			return 1f;
		}
		if (val < -0f)
		{
			return -1f;
		}
		return 0f;
	}

	public static Vector3 RandomVectorWithinAngleInDirection(Vector3 vector, float angleDegs)
	{
		uint num = 100u;
		float num2 = Mathf.Cos(angleDegs * (MathF.PI / 180f));
		while (num != 0)
		{
			num--;
			Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
			Vector3 normalized = Vector3.Cross(vector, insideUnitSphere).normalized;
			float value = UnityEngine.Random.value;
			float value2 = UnityEngine.Random.value;
			float num3 = Mathf.Cos(angleDegs * (MathF.PI / 180f));
			float f = MathF.PI * 2f * value;
			float num4 = num3 + (1f - num3) * value2;
			float num5 = Mathf.Sqrt(1f - num4 * num4);
			float num6 = Mathf.Cos(f) * num5;
			float num7 = Mathf.Sin(f) * num5;
			Vector3 result = insideUnitSphere * num6 + normalized * num7 + vector * num4;
			if (Vector3.Dot(result.normalized, vector.normalized) > num2)
			{
				return result;
			}
		}
		return Vector3.zero;
	}

	public static bool LineIntersect2D(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 res)
	{
		float x = p1.x;
		float x2 = p2.x;
		float x3 = p3.x;
		float x4 = p4.x;
		float y = p1.y;
		float y2 = p2.y;
		float y3 = p3.y;
		float y4 = p4.y;
		float num = (x - x2) * (y3 - y4) - (y - y2) * (x3 - x4);
		if (num == 0f)
		{
			return false;
		}
		float num2 = x * y2 - y * x2;
		float num3 = x3 * y4 - y3 * x4;
		float num4 = (num2 * (x3 - x4) - (x - x2) * num3) / num;
		float num5 = (num2 * (y3 - y4) - (y - y2) * num3) / num;
		if (num4 < Mathf.Min(x, x2) || num4 > Mathf.Max(x, x2) || num4 < Mathf.Min(x3, x4) || num4 > Mathf.Max(x3, x4))
		{
			return false;
		}
		if (num5 < Mathf.Min(y, y2) || num5 > Mathf.Max(y, y2) || num5 < Mathf.Min(y3, y4) || num5 > Mathf.Max(y3, y4))
		{
			return false;
		}
		res.x = num4;
		res.y = num5;
		return true;
	}

	public static float GetQuatLength(Quaternion q)
	{
		return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
	}

	public static Quaternion GetQuatConjugate(Quaternion q)
	{
		return new Quaternion(0f - q.x, 0f - q.y, 0f - q.z, q.w);
	}

	public static Quaternion GetQuatLog(Quaternion q)
	{
		Quaternion result = q;
		result.w = 0f;
		if (Mathf.Abs(q.w) < 1f)
		{
			float num = Mathf.Acos(q.w);
			float num2 = Mathf.Sin(num);
			if ((double)Mathf.Abs(num2) > 0.0001)
			{
				float num3 = num / num2;
				result.x = q.x * num3;
				result.y = q.y * num3;
				result.z = q.z * num3;
			}
		}
		return result;
	}

	public static Quaternion GetQuatExp(Quaternion q)
	{
		Quaternion result = q;
		float num = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z);
		float num2 = Mathf.Sin(num);
		result.w = Mathf.Cos(num);
		if ((double)Mathf.Abs(num2) > 0.0001)
		{
			float num3 = num2 / num;
			result.x = num3 * q.x;
			result.y = num3 * q.y;
			result.z = num3 * q.z;
		}
		return result;
	}

	public static float ExponentialEase(float t, float power)
	{
		if (t < 0.5f)
		{
			return Mathf.Pow(2f * t, power) * 0.5f;
		}
		return 1f - Mathf.Pow(2f * (1f - t), power) * 0.5f;
	}

	public static Quaternion GetQuatSquad(float t, Quaternion q0, Quaternion q1, Quaternion a0, Quaternion a1)
	{
		float t2 = 2f * t * (1f - t);
		Quaternion p = Slerp(q0, q1, t);
		Quaternion q2 = Slerp(a0, a1, t);
		return Slerp(p, q2, t2);
	}

	public static Quaternion GetSquadIntermediate(Quaternion q0, Quaternion q1, Quaternion q2)
	{
		Quaternion quatConjugate = GetQuatConjugate(q1);
		Quaternion quatLog = GetQuatLog(quatConjugate * q0);
		Quaternion quatLog2 = GetQuatLog(quatConjugate * q2);
		Quaternion q3 = new Quaternion(-0.25f * (quatLog.x + quatLog2.x), -0.25f * (quatLog.y + quatLog2.y), -0.25f * (quatLog.z + quatLog2.z), -0.25f * (quatLog.w + quatLog2.w));
		return q1 * GetQuatExp(q3);
	}

	public static float Ease(float t, float k1, float k2)
	{
		float num = k1 * 2f / MathF.PI + k2 - k1 + (1f - k2) * 2f / MathF.PI;
		float num2 = ((t < k1) ? (k1 * (2f / MathF.PI) * (Mathf.Sin(t / k1 * MathF.PI / 2f - MathF.PI / 2f) + 1f)) : ((!(t < k2)) ? (2f * k1 / MathF.PI + k2 - k1 + (1f - k2) * (2f / MathF.PI) * Mathf.Sin((t - k2) / (1f - k2) * MathF.PI / 2f)) : (2f * k1 / MathF.PI + t - k1)));
		return num2 / num;
	}

	public static Quaternion Slerp(Quaternion p, Quaternion q, float t)
	{
		float num = Quaternion.Dot(p, q);
		Quaternion result = default(Quaternion);
		if ((double)(1f + num) > 1E-05)
		{
			float num4;
			float num5;
			if ((double)(1f - num) > 1E-05)
			{
				float num2 = Mathf.Acos(num);
				float num3 = 1f / Mathf.Sin(num2);
				num4 = Mathf.Sin((1f - t) * num2) * num3;
				num5 = Mathf.Sin(t * num2) * num3;
			}
			else
			{
				num4 = 1f - t;
				num5 = t;
			}
			result.x = num4 * p.x + num5 * q.x;
			result.y = num4 * p.y + num5 * q.y;
			result.z = num4 * p.z + num5 * q.z;
			result.w = num4 * p.w + num5 * q.w;
		}
		else
		{
			float num6 = Mathf.Sin((1f - t) * MathF.PI * 0.5f);
			float num7 = Mathf.Sin(t * MathF.PI * 0.5f);
			result.x = num6 * p.x - num7 * p.y;
			result.y = num6 * p.y + num7 * p.x;
			result.z = num6 * p.z - num7 * p.w;
			result.w = p.z;
		}
		return result;
	}

	public static void BoundingSphereOfPoints(List<Vector3> points, out Vector3 boundingSphereCentre, out float boundingSphereRadius)
	{
		boundingSphereCentre = default(Vector3);
		boundingSphereRadius = 0f;
		foreach (Vector3 point in points)
		{
			if (boundingSphereCentre == Vector3.zero)
			{
				boundingSphereCentre = point;
				continue;
			}
			float magnitude = (boundingSphereCentre - point).magnitude;
			Vector3 normalized = (point - boundingSphereCentre).normalized;
			float num = (magnitude + boundingSphereRadius) / 2f;
			float num2 = Mathf.Max(0f, num - boundingSphereRadius);
			if (num > boundingSphereRadius)
			{
				boundingSphereRadius = num;
				boundingSphereCentre += normalized * num2;
			}
		}
	}

	public static void BoundingSphereOfPoints(Vector3[] points, out Vector3 boundingSphereCentre, out float boundingSphereRadius)
	{
		boundingSphereCentre = default(Vector3);
		boundingSphereRadius = 0f;
		foreach (Vector3 vector in points)
		{
			if (boundingSphereCentre == Vector3.zero)
			{
				boundingSphereCentre = vector;
				continue;
			}
			float magnitude = (boundingSphereCentre - vector).magnitude;
			Vector3 normalized = (vector - boundingSphereCentre).normalized;
			float num = (magnitude + boundingSphereRadius) / 2f;
			float num2 = Mathf.Max(0f, num - boundingSphereRadius);
			if (num > boundingSphereRadius)
			{
				boundingSphereRadius = num;
				boundingSphereCentre += normalized * num2;
			}
		}
	}

	public static float MaxAngle(Vector3 pointFrom, List<Vector3> points)
	{
		float largestAngle = 0f;
		Vector3 dir = default(Vector3);
		Vector3 dir2 = default(Vector3);
		int count = points.Count;
		for (int i = 0; i < count; i++)
		{
			Vector3 normalized = (pointFrom - points[i]).normalized;
			if (dir == Vector3.zero)
			{
				dir = normalized;
			}
			else if (dir2 == Vector3.zero)
			{
				dir2 = normalized;
				largestAngle = Vector3.Angle(dir, dir2);
			}
			else
			{
				NewAngle(normalized, ref largestAngle, ref dir, ref dir2);
			}
		}
		return largestAngle;
	}

	private static void NewAngle(Vector3 newDir, ref float largestAngle, ref Vector3 dir1, ref Vector3 dir2)
	{
		float num = Vector3.Angle(dir1, newDir);
		float num2 = Vector3.Angle(dir2, newDir);
		if (num > largestAngle || num2 > largestAngle)
		{
			if (num > num2)
			{
				dir2 = newDir;
				largestAngle = num;
			}
			else
			{
				dir1 = newDir;
				largestAngle = num2;
			}
		}
	}

	public static string GetMd5Hash(string input)
	{
		byte[] array = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	public static float RoundUp(float value, float roundUp)
	{
		if (value > 0f)
		{
			return Mathf.Ceil(value / roundUp) * roundUp;
		}
		if (value < 0f)
		{
			return Mathf.Floor(value / roundUp) * roundUp;
		}
		return roundUp;
	}

	public static float RoundDown(float value, float roundDown)
	{
		if (value < 0f)
		{
			return Mathf.Ceil(value / roundDown) * roundDown;
		}
		if (value > 0f)
		{
			return Mathf.Floor(value / roundDown) * roundDown;
		}
		return roundDown;
	}

	public static float MeanAngle(float[] angles)
	{
		float num = 0f;
		float num2 = 0f;
		int num3 = angles.Length;
		for (int i = 0; i < num3; i++)
		{
			num2 += Mathf.Cos(angles[i]);
			num += Mathf.Sin(angles[i]);
		}
		return Mathf.Atan2(num / (float)num3, num2 / (float)num3);
	}

	public static float WeightedAverageAngle(float[] angles, int[] weights)
	{
		float num = 0f;
		float num2 = 0f;
		int num3 = 0;
		int num4 = angles.Length;
		for (int i = 0; i < num4 && i <= weights.Length; i++)
		{
			num3 += weights[i];
			num2 += Mathf.Cos(angles[i]) * (float)weights[i];
			num += Mathf.Sin(angles[i]) * (float)weights[i];
		}
		return Mathf.Atan2(num / (float)num3, num2 / (float)num3);
	}

	public static Vector3 ExpectMatch(Vector3 value, Vector3 test, GameObject context)
	{
		if (Mathf.Abs(value.magnitude - test.magnitude) > 0.001f)
		{
			Debug.LogWarning("Vector LengthTest Failed : Magnitude", context);
		}
		if (Vector3.Dot(value, test) < 0.999f)
		{
			Debug.LogWarning("Vector LengthTest Failed : Direction", context);
		}
		return value;
	}

	public static float ExpectMatch(float value, float test, GameObject context)
	{
		if (Mathf.Abs(value - test) > 0.001f)
		{
			Debug.LogWarning("Float Match Failed : Magnitude", context);
		}
		return value;
	}

	public static Vector3 WeightedAverageVector(Vector3[] vectors, int[] weights)
	{
		Vector3 zero = Vector3.zero;
		int num = 0;
		int num2 = vectors.Length;
		for (int i = 0; i < num2 && i <= weights.Length; i++)
		{
			num += weights[i];
			zero += vectors[i] * weights[i];
		}
		return zero / num;
	}

	public static int iLog2(int v)
	{
		int num = (int)(((v > 65535) ? 1u : 0u) << 4);
		v >>= num;
		int num2 = (int)(((v > 255) ? 1u : 0u) << 3);
		v >>= num2;
		num |= num2;
		num2 = (int)(((v > 15) ? 1u : 0u) << 2);
		v >>= num2;
		num |= num2;
		num2 = (int)(((v > 3) ? 1u : 0u) << 1);
		v >>= num2;
		num |= num2;
		return num | (v >> 1);
	}

	public static int NextPowerOfTwo(int v)
	{
		if (v < 0)
		{
			return 1;
		}
		if (v == 0)
		{
			return 1;
		}
		if (v == (v & (~v + 1)))
		{
			return v;
		}
		v--;
		v |= v >> 1;
		v |= v >> 2;
		v |= v >> 4;
		v |= v >> 8;
		v |= v >> 16;
		v++;
		return v;
	}

	public static bool IsPowerOfTwo(int v)
	{
		if (v < 0)
		{
			return false;
		}
		return v == (v & (~v + 1));
	}

	public static bool IsPowerOfTwo(uint v)
	{
		return v == (v & (~v + 1));
	}

	public static uint BinaryToGray(uint v)
	{
		return (v >> 1) ^ v;
	}

	public static uint GrayToBinary(uint v)
	{
		v ^= v >> 16;
		v ^= v >> 8;
		v ^= v >> 4;
		v ^= v >> 2;
		v ^= v >> 1;
		return v;
	}

	public static int BinaryToGray(int v)
	{
		return (v >> 1) ^ v;
	}

	public static int GrayToBinary(int v)
	{
		v ^= v >> 16;
		v ^= v >> 8;
		v ^= v >> 4;
		v ^= v >> 2;
		v ^= v >> 1;
		return v;
	}

	public static int IntPow(int x, int pow)
	{
		int num = 1;
		while (pow != 0)
		{
			if ((pow & 1) == 1)
			{
				num *= x;
			}
			x *= x;
			pow >>= 1;
		}
		return num;
	}

	public static Vector3 GetPositionOnBezier(float t, Vector3 p0, Vector3 anchor0, Vector3 anchor1, Vector3 p1)
	{
		float num = 1f - t;
		float num2 = t * t;
		float num3 = num * num;
		float num4 = num3 * num;
		float num5 = num2 * t;
		return num4 * p0 + 3f * num3 * t * anchor0 + 3f * num * num2 * anchor1 + num5 * p1;
	}

	public static Quaternion GetLookRotationOnBezierAsQuaternion(float t, Vector3 p0, Vector3 anchor0, Vector3 anchor1, Vector3 p1, float precision, Vector3 up)
	{
		Vector3 positionOnBezier = GetPositionOnBezier(t, p0, anchor0, anchor1, p1);
		return Quaternion.LookRotation((GetPositionOnBezier(t + precision, p0, anchor0, anchor1, p1) - positionOnBezier).normalized, up);
	}

	public static Vector3 GetLookDirectionOnBezier(float t, Vector3 p0, Vector3 anchor0, Vector3 anchor1, Vector3 p1, float precision, Vector3 up)
	{
		Vector3 positionOnBezier = GetPositionOnBezier(t, p0, anchor0, anchor1, p1);
		return (GetPositionOnBezier(t + precision, p0, anchor0, anchor1, p1) - positionOnBezier).normalized;
	}

	public static float ProjectPointOnBezier(Vector3 point, float startInterval, float endInterval, int steps, Vector3 p0, Vector3 anchor0, Vector3 anchor1, Vector3 p1, out float distance)
	{
		startInterval = Mathf.Clamp01(startInterval);
		endInterval = Mathf.Clamp01(endInterval);
		float num = (endInterval - startInterval) / (float)steps;
		float result = 0f;
		distance = float.MaxValue;
		for (int i = 0; i < steps; i++)
		{
			float num2 = startInterval + num * (float)i;
			float sqrMagnitude = (GetPositionOnBezier(num2, p0, anchor0, anchor1, p1) - point).sqrMagnitude;
			if (sqrMagnitude < distance)
			{
				distance = sqrMagnitude;
				result = num2;
			}
		}
		return result;
	}

	public static float ProjectPointOnBezier(Vector3 point, float startInterval, float endInterval, int steps, Vector3 p0, Vector3 anchor0, Vector3 anchor1, Vector3 p1)
	{
		float distance = 0f;
		return ProjectPointOnBezier(point, startInterval, endInterval, steps, p0, anchor0, anchor1, p1, out distance);
	}

	public static float ProjectPointOnBezierBinarySearch(Vector3 point, Vector3 p0, Vector3 anchor0, Vector3 anchor1, Vector3 p1)
	{
		float outDistance = 0f;
		return ProjectPointOnBezierBinarySearch(point, p0, anchor0, anchor1, p1, out outDistance);
	}

	public static float ProjectPointOnBezierBinarySearch(Vector3 point, Vector3 p0, Vector3 anchor0, Vector3 anchor1, Vector3 p1, out float outDistance)
	{
		float num = ProjectPointOnBezier(point, 0f, 1f, 10, p0, anchor0, anchor1, p1);
		float num2 = 0.1f;
		float distance = float.MaxValue;
		for (int i = 0; i < 4; i++)
		{
			num = ProjectPointOnBezier(point, num - num2, num + num2, 10, p0, anchor0, anchor1, p1, out distance);
			num2 /= 9f;
		}
		outDistance = distance;
		return num;
	}

	public static Quaternion Uppify(Quaternion q, Vector3 up)
	{
		return Quaternion.FromToRotation(q * Vector3.up, up) * q;
	}

	public static float Remap(float value, float fromLow, float fromHigh, float toLow, float toHigh)
	{
		return toLow + (value - fromLow) * (toHigh - toLow) / (fromHigh - fromLow);
	}

	public static float RemapRandom(System.Random random, float from, float to)
	{
		return Remap((float)random.NextDouble(), 0f, 1f, from, to);
	}

	public static bool RotatedAABBContains(Bounds aabb, Vector3 point, Matrix4x4 inverseTransformMatrix)
	{
		Vector3 point2 = inverseTransformMatrix.MultiplyPoint(point);
		return aabb.Contains(point2);
	}
}

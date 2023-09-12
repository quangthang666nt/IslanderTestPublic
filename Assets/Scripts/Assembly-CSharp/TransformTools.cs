using UnityEngine;

public class TransformTools
{
	public static void BoxColliderToWorldSpace(BoxCollider _bc, ref Vector3 _v3OutPosition, ref Vector3 _v3OutScale, ref Quaternion _qOutRotation)
	{
		_v3OutPosition = _bc.transform.TransformPoint(_bc.center);
		_v3OutScale = Vector3.Scale(_bc.size, _bc.transform.lossyScale);
		_qOutRotation = _bc.transform.rotation;
	}

	public static void BoxColliderAtFakeTransformToWorldSpace(BoxCollider _bc, Transform _transFake, ref Vector3 _v3OutPosition, ref Vector3 _v3OutScale, ref Quaternion _qOutRotation)
	{
		_v3OutPosition = _transFake.TransformPoint(_bc.center);
		_v3OutScale = Vector3.Scale(_bc.size, _transFake.lossyScale);
		_qOutRotation = _transFake.rotation;
	}

	public static void SphereColliderToWorldSpace(SphereCollider _sc, ref Vector3 _v3OutPosition, ref float _fOutRadius)
	{
		_v3OutPosition = _sc.transform.TransformPoint(_sc.center);
		_fOutRadius = _sc.radius * _sc.gameObject.transform.lossyScale.y;
	}
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class BasicAnimation : MonoBehaviour
{
	[Serializable]
	private class TimedEvent
	{
		[Range(0f, 1f)]
		public float fTime;

		public UnityEvent eventEvent;

		[HideInInspector]
		public bool wasInvokedThisCycle;
	}

	[Header("Animation")]
	[SerializeField]
	private bool animatedInEditMode = true;

	[SerializeField]
	private Vector2 v2AnimationLengthMinMax = Vector2.one;

	[Range(0f, 1f)]
	[SerializeField]
	private float fOffset;

	[SerializeField]
	private bool bPlayOnStart = true;

	[SerializeField]
	private bool bLoop = true;

	[SerializeField]
	private bool bRandomSpeedEveryLoop;

	[SerializeField]
	[Header("Optional")]
	[Tooltip("If this is null, the transform of this gameobject will be animated.")]
	private Transform tAnimateThisTransform;

	[Header("Scale")]
	[SerializeField]
	private bool bAnimateScale;

	[SerializeField]
	private Vector2 v2MinMaxScale = Vector2.one;

	[SerializeField]
	private AnimationCurve curveScaleAnimationX = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	[SerializeField]
	private AnimationCurve curveScaleAnimationY = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	[SerializeField]
	private AnimationCurve curveScaleAnimationZ = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	[Header("Rotation")]
	[SerializeField]
	private bool bAnimateRotation;

	[SerializeField]
	private Vector3 v3MaxRotationXYZ = Vector3.zero;

	[SerializeField]
	private AnimationCurve curveRotationX = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	[SerializeField]
	private AnimationCurve curveRotationY = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	[SerializeField]
	private AnimationCurve curveRotationZ = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	[Header("Position")]
	[SerializeField]
	private bool bAnimatePosition;

	[SerializeField]
	private Vector3 v3MaxOffset = Vector3.zero;

	[SerializeField]
	private AnimationCurve curveOffsetX = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	[SerializeField]
	private AnimationCurve curveOffsetY = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	[SerializeField]
	private AnimationCurve curveOffsetZ = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	[Header("Events")]
	public UnityEvent eventOnAnimationStart = new UnityEvent();

	public UnityEvent eventOnAnimationEnd = new UnityEvent();

	[SerializeField]
	private TimedEvent[] arTimedEvents;

	private Coroutine coroutineCurrentAnimation;

	private Vector3 v3OriginalPosition;

	private Vector3 v3OriginalScale;

	private Quaternion qOriginalRotation;

	private float fAnimationLength;

	[ContextMenu("Start")]
	private void Start()
	{
		fAnimationLength = UnityEngine.Random.Range(v2AnimationLengthMinMax.x, v2AnimationLengthMinMax.y);
		if (bPlayOnStart)
		{
			StartAnimation();
		}
	}

	[ContextMenu("Start Animation")]
	public void StartAnimation()
	{
		if (!Application.isPlaying && !animatedInEditMode)
		{
			return;
		}
		if (arTimedEvents != null)
		{
			TimedEvent[] array = arTimedEvents;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].wasInvokedThisCycle = false;
			}
		}
		if (bRandomSpeedEveryLoop)
		{
			fAnimationLength = UnityEngine.Random.Range(v2AnimationLengthMinMax.x, v2AnimationLengthMinMax.y);
		}
		if ((bool)tAnimateThisTransform)
		{
			StoreOriginalTransformInfo(tAnimateThisTransform);
		}
		else
		{
			StoreOriginalTransformInfo(base.transform);
		}
		if (coroutineCurrentAnimation != null)
		{
			StopCoroutine(coroutineCurrentAnimation);
		}
		eventOnAnimationStart.Invoke();
		coroutineCurrentAnimation = StartCoroutine(Animate());
	}

	[ContextMenu("Stop Animation")]
	public void StopAnimation()
	{
		if (coroutineCurrentAnimation != null)
		{
			StopCoroutine(coroutineCurrentAnimation);
		}
		if ((bool)tAnimateThisTransform)
		{
			RestoreOriginalTransformation(tAnimateThisTransform);
		}
		else
		{
			RestoreOriginalTransformation(base.transform);
		}
	}

	private IEnumerator Animate()
	{
		if (!Application.isPlaying && !animatedInEditMode)
		{
			yield break;
		}
		Transform tAnimationTarget = ((!tAnimateThisTransform) ? base.transform : tAnimateThisTransform);
		float fTimer = 0f;
		while (fTimer <= fAnimationLength)
		{
			float num = fTimer / fAnimationLength;
			num = Mathf.Repeat(num + fOffset, 1f);
			if (bAnimateScale)
			{
				tAnimationTarget.localScale = V3EvaluateScaleCurves(num);
			}
			if (bAnimateRotation)
			{
				tAnimationTarget.localRotation = QuatEvaluateRotationCurves(num);
			}
			if (bAnimatePosition)
			{
				tAnimationTarget.localPosition = V3EvaluateTranslationCurves(num, v3OriginalPosition);
			}
			if (arTimedEvents != null)
			{
				TimedEvent[] array = arTimedEvents;
				foreach (TimedEvent timedEvent in array)
				{
					if (num >= timedEvent.fTime && !timedEvent.wasInvokedThisCycle)
					{
						timedEvent.eventEvent.Invoke();
						timedEvent.wasInvokedThisCycle = true;
					}
				}
			}
			fTimer += Time.deltaTime;
			yield return null;
		}
		if (bAnimateScale)
		{
			tAnimationTarget.localScale = V3EvaluateScaleCurves(1f);
		}
		if (bAnimateRotation)
		{
			tAnimationTarget.localRotation = QuatEvaluateRotationCurves(1f);
		}
		if (bAnimatePosition)
		{
			tAnimationTarget.localPosition = V3EvaluateTranslationCurves(1f, v3OriginalPosition);
		}
		eventOnAnimationEnd.Invoke();
		if (bLoop)
		{
			StartAnimation();
		}
	}

	private Vector3 V3EvaluateScaleCurves(float t)
	{
		Vector3 one = Vector3.one;
		one.x = v3OriginalScale.x * Mathf.Lerp(v2MinMaxScale.x, v2MinMaxScale.y, curveScaleAnimationX.Evaluate(t));
		one.y = v3OriginalScale.y * Mathf.Lerp(v2MinMaxScale.x, v2MinMaxScale.y, curveScaleAnimationY.Evaluate(t));
		one.z = v3OriginalScale.z * Mathf.Lerp(v2MinMaxScale.x, v2MinMaxScale.y, curveScaleAnimationZ.Evaluate(t));
		return one;
	}

	private Quaternion QuatEvaluateRotationCurves(float t)
	{
		_ = Quaternion.identity;
		Vector3 zero = Vector3.zero;
		zero.x = qOriginalRotation.eulerAngles.x + Mathf.Lerp(0f, v3MaxRotationXYZ.x, curveRotationX.Evaluate(t));
		zero.y = qOriginalRotation.eulerAngles.y + Mathf.Lerp(0f, v3MaxRotationXYZ.y, curveRotationY.Evaluate(t));
		zero.z = qOriginalRotation.eulerAngles.z + Mathf.Lerp(0f, v3MaxRotationXYZ.z, curveRotationZ.Evaluate(t));
		return Quaternion.Euler(zero.x, zero.y, zero.z);
	}

	private Vector3 V3EvaluateTranslationCurves(float t, Vector3 originalPos)
	{
		Vector3 zero = Vector3.zero;
		zero.x = Mathf.Lerp(originalPos.x, originalPos.x + v3MaxOffset.x, curveOffsetX.Evaluate(t));
		zero.y = Mathf.Lerp(originalPos.y, originalPos.y + v3MaxOffset.y, curveOffsetY.Evaluate(t));
		zero.z = Mathf.Lerp(originalPos.z, originalPos.z + v3MaxOffset.z, curveOffsetZ.Evaluate(t));
		return zero;
	}

	private void StoreOriginalTransformInfo(Transform t)
	{
		v3OriginalPosition = t.localPosition;
		v3OriginalScale = t.localScale;
		qOriginalRotation = t.localRotation;
	}

	private void RestoreOriginalTransformation(Transform t)
	{
		t.localPosition = v3OriginalPosition;
		t.localScale = v3OriginalScale;
		t.localRotation = qOriginalRotation;
	}

	private void OnDisable()
	{
		if (coroutineCurrentAnimation != null)
		{
			StopAnimation();
		}
	}
}

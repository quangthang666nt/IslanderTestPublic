using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationGroup : MonoBehaviour
{
	[Serializable]
	public class AnimationSet
	{
		public AnimationCurve x;

		public AnimationCurve y;

		public AnimationCurve z;
	}

	public enum AnimationDirection
	{
		TopDown = 0,
		BottomUp = 1
	}

	public bool bAnimateOnEnable;

	[Header("Objects To Animate (Single)")]
	public List<Transform> transObjsToAnimate;

	[Header("Objects To Animate (Children)")]
	public Transform transTarget;

	[Header("Setup")]
	public AnimationDirection animationDirection;

	public float fAnimationDelayIn;

	public float fAnimationTimeIn = 1f;

	public float fTimeSpacingIn;

	public float fAnimationTimeOut = 1f;

	public float fTimeSpacingOut;

	public bool overrideDelayOut;

	public float fAnimationDelayOut;

	[Header("Scale")]
	public AnimationSet scaleIn;

	public AnimationSet scaleOut;

	private bool bAnimationModifier;

	private void OnEnable()
	{
		if (bAnimateOnEnable)
		{
			AnimationIn();
		}
	}

	[ContextMenu("Animation In")]
	public void AnimationIn()
	{
		bAnimationModifier = false;
		StopAllCoroutines();
		StartCoroutine(PlayAnimation(fAnimationTimeIn, fTimeSpacingIn, scaleIn, fAnimationDelayIn));
	}

	[ContextMenu("Animation Out")]
	public void AnimationOut()
	{
		bAnimationModifier = true;
		StopAllCoroutines();
		StartCoroutine(PlayAnimation(fAnimationTimeOut, fTimeSpacingOut, scaleOut, overrideDelayOut ? fAnimationDelayOut : fAnimationDelayIn));
	}

	private IEnumerator PlayAnimation(float elementLength, float spacing, AnimationSet scale, float animationDelay)
	{
		WaitForSeconds wfsSpacer = new WaitForSeconds(spacing);
		List<Transform> liTransChildren = new List<Transform>();
		liTransChildren.AddRange(transObjsToAnimate);
		if (transTarget != null)
		{
			foreach (Transform item in transTarget)
			{
				liTransChildren.Add(item);
			}
		}
		Vector3 localScale = new Vector3(scale.x.Evaluate(0f), scale.y.Evaluate(0f), scale.z.Evaluate(0f));
		for (int num = liTransChildren.Count - 1; num >= 0; num--)
		{
			liTransChildren[num].localScale = localScale;
		}
		if (animationDirection == AnimationDirection.BottomUp)
		{
			liTransChildren.Reverse();
		}
		yield return new WaitForSeconds(animationDelay);
		if (bAnimationModifier)
		{
			for (int j = liTransChildren.Count - 1; j >= 0; j--)
			{
				StartCoroutine(AnimationElement(elementLength, liTransChildren[j], scale));
				if (spacing > 0f && liTransChildren[j].gameObject.activeSelf)
				{
					yield return wfsSpacer;
				}
			}
			yield break;
		}
		for (int j = 0; j < liTransChildren.Count; j++)
		{
			StartCoroutine(AnimationElement(elementLength, liTransChildren[j], scale));
			if (spacing > 0f && liTransChildren[j].gameObject.activeSelf)
			{
				yield return wfsSpacer;
			}
		}
	}

	private IEnumerator AnimationElement(float length, Transform _transTarget, AnimationSet scale)
	{
		float _fTimer = 0f;
		Vector3 localScale = default(Vector3);
		while (_fTimer <= length)
		{
			_fTimer += Time.deltaTime;
			float time = Mathf.InverseLerp(0f, length, _fTimer);
			localScale.x = scale.x.Evaluate(time);
			localScale.y = scale.y.Evaluate(time);
			localScale.z = scale.z.Evaluate(time);
			_transTarget.localScale = localScale;
			yield return null;
		}
		yield return null;
	}
}

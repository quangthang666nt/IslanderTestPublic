using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateChildsRandom : MonoBehaviour
{
	public float timeToActivated = 0.5f;

	public Transform target;

	private Coroutine currentActivationCoroutine;

	private float[] activationTimeOffset;

	private List<GameObject> childs = new List<GameObject>();

	private List<GameObject> childsStaticList = new List<GameObject>();

	private void OnEnable()
	{
		if (currentActivationCoroutine != null)
		{
			CoroutineManagerSingleton.Instance.StopCoroutine(currentActivationCoroutine);
		}
		currentActivationCoroutine = CoroutineManagerSingleton.Instance.StartCoroutine(SetActiveChilds(active: true));
	}

	private void OnDisable()
	{
		if (currentActivationCoroutine != null)
		{
			CoroutineManagerSingleton.Instance.StopCoroutine(currentActivationCoroutine);
		}
		currentActivationCoroutine = CoroutineManagerSingleton.Instance.StartCoroutine(SetActiveChilds(active: false));
	}

	private IEnumerator SetActiveChilds(bool active)
	{
		childs.Clear();
		if (target == null)
		{
			yield return null;
		}
		activationTimeOffset = new float[target.childCount];
		float num = 0f;
		float maxInclusive = 1f / (float)target.childCount;
		for (int j = 0; j < target.childCount; j++)
		{
			GameObject gameObject = target.GetChild(j).gameObject;
			gameObject.SetActive(!active);
			childs.Add(gameObject);
			activationTimeOffset[j] = num + Random.Range(0f, maxInclusive);
			num = activationTimeOffset[j];
		}
		for (int i = 0; i < target.childCount; i++)
		{
			yield return new WaitForSeconds(activationTimeOffset[i] * timeToActivated);
			GameObject gameObject2 = childs[Random.Range(0, childs.Count)];
			gameObject2.SetActive(active);
			childs.Remove(gameObject2);
		}
	}
}

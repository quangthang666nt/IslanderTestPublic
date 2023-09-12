using UnityEngine;

public class OnStartSetRandomScale : MonoBehaviour
{
	[SerializeField]
	private Vector2 v2MinMaxScaleMultiplier = new Vector2(0.9f, 1.1f);

	private void Start()
	{
		base.transform.localScale *= Random.Range(v2MinMaxScaleMultiplier.x, v2MinMaxScaleMultiplier.y);
	}
}

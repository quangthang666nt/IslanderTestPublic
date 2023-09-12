using UnityEngine;

public class LogErrorOnStart : MonoBehaviour
{
	private void Start()
	{
		Debug.LogError("LogErrorOnStart");
	}

	private void Update()
	{
	}
}

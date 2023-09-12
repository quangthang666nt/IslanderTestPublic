using System;
using UnityEngine;

public class SCSDialogsManager : MonoBehaviour
{
	public static SCSDialogsManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
	}

	public void OpenInputDialog(string title, int textLenght, string startText, Action<string> onSuccess, Action onError)
	{
	}
}

using UnityEngine;

public class CommandLineDispatcherGame : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		CommandLineHandler.Initialize();
		CommandLineHandler.ApplicationName = Application.productName;
	}

	private void Update()
	{
		lock (CommandLineHandler.m_MainThreadCommandLines)
		{
			while (CommandLineHandler.m_MainThreadCommandLines.Count > 0)
			{
				CommandLineHandler.CallMethod(CommandLineHandler.m_MainThreadCommandLines.Dequeue());
			}
		}
	}
}

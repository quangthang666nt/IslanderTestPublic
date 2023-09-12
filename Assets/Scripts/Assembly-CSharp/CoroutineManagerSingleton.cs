using UnityEngine;

public class CoroutineManagerSingleton : MonoBehaviour
{
	private static CoroutineManagerSingleton instance_;

	public static CoroutineManagerSingleton Instance
	{
		get
		{
			if (instance_ == null)
			{
				GameObject obj = new GameObject("CoroutineManager");
				instance_ = obj.AddComponent<CoroutineManagerSingleton>();
				Object.DontDestroyOnLoad(obj);
			}
			return instance_;
		}
	}
}

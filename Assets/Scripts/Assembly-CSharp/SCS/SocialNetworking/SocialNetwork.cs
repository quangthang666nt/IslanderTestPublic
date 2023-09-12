using System;
using SCS.UserManagement;
using UnityEngine;

namespace SCS.SocialNetworking
{
	[Obsolete("This class is obsolete. For rich presence, use SCSStatsManager; For the online functions, use SCSUserManager")]
	public class SocialNetwork : MonoBehaviour
	{
		public static SocialNetwork Instance;

		public bool HasOnlinePrivileges => true;

		private void Awake()
		{
			if (Application.isPlaying)
			{
				if (Instance != null)
				{
					UnityEngine.Object.DestroyImmediate(this);
					return;
				}
				Instance = this;
				UnityEngine.Object.DontDestroyOnLoad(this);
			}
		}

		public void SetRichPresence(string presenceID, UserData user, params string[] s)
		{
		}
	}
}

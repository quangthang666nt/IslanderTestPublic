using System.Collections.Generic;
using UnityEngine;

namespace SCS.UserManagement
{
	public class UserData
	{
		public class OnlineProfile
		{
		}

		public Texture2D picture;

		public bool isPictureReady;

		public int internalID;

		public string internalSID;

		public string Name { get; private set; }

		public List<int> Controllers { get; private set; }

		public UserData(string name, Texture2D picture = null)
		{
			Name = name;
			Controllers = new List<int>();
			this.picture = picture;
			isPictureReady = false;
		}
	}
}

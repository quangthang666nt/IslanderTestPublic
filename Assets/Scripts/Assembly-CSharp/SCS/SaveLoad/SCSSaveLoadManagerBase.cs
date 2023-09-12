using System;
using System.Collections;
using SCS.UserManagement;
using UnityEngine;

namespace SCS.SaveLoad
{
	public abstract class SCSSaveLoadManagerBase : MonoBehaviour
	{
		protected abstract void Init();

		protected abstract void OnDestroy();

		protected abstract IEnumerator _Save(SCSSaveData saveData, SaveLocation saveLocation, UserData user, Action successCallback, Action<SaveLoadErrorResult> errorCallback);

		protected abstract IEnumerator _Load(SaveLocation saveLocation, UserData user, SCSSaveData saveData, Action<SCSSaveData> successCallback, Action<SaveLoadErrorResult> errorCallback, string blopName = "");

		protected abstract bool _Exists(SaveLocation saveLocation, UserData user);

		protected abstract IEnumerator _Delete(SaveLocation saveLocation, UserData user, Action onDelete = null, Action onCancel = null, bool displayDialog = true);
	}
}

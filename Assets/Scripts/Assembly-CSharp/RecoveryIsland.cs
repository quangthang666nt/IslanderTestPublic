using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class RecoveryIsland
{
	public static string GetGrizzlyBackupPath()
	{
		string[] array = Application.persistentDataPath.Split("/");
		string text = string.Empty;
		for (int i = 0; i < array.Length - 2; i++)
		{
			text = text + array[i] + "/";
		}
		return text + "Grizzly Games/ISLANDERS/TheSaveV06_Backup.island";
	}

	public static IEnumerator LoadGrizzlyFile(Action OnSuccess, Action OnFailedRecovery)
	{
		return LoadGrizzlyFile(GetGrizzlyBackupPath(), OnSuccess, OnFailedRecovery);
	}

	public static IEnumerator LoadGrizzlyFile(string currentPath, Action OnSuccess, Action OnFailedRecovery)
	{
		if (!File.Exists(currentPath))
		{
			OnFailedRecovery?.Invoke();
			yield break;
		}
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream serializationStream = new FileStream(currentPath, FileMode.Open);
		SaveGame saveGame = (SaveGame)binaryFormatter.Deserialize(serializationStream);
		if (saveGame.statsMatch != null)
		{
			if (saveGame.statsMatch.liIMilisecondsPerBuilding == null)
			{
				saveGame.statsMatch.liIMilisecondsPerBuilding = new List<int>();
			}
			if (saveGame.statsMatch.liIWentToNextIslandAtMiliseconds == null)
			{
				saveGame.statsMatch.liIWentToNextIslandAtMiliseconds = new List<int>();
			}
		}
		SaveLoadManager.saveGameCurrent = saveGame;
		yield return CoroutineManagerSingleton.Instance.StartCoroutine(saveGame.ApplyFile());
		SaveLoadManager.PerformAutosave(force: true);
		OnSuccess?.Invoke();
	}
}

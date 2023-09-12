using System;
using System.Collections;
using System.Collections.Generic;
using FlatBuffers;
using Islanders;
using UnityEngine;

[Serializable]
public class SaveGame
{
	public enum ELoadMode
	{
		Normal = 0,
		NewGame = 1,
		NextIsland = 2,
		GameLaunch = 3
	}

	public List<int> liIStructIds = new List<int>();

	public List<SeriTransform> liSeriTransformsOfStructIds = new List<SeriTransform>();

	public List<int> liIPlaceableIds = new List<int>();

	public List<SeriTransform> liSeriTransOfPlaceables = new List<SeriTransform>();

	public int iDebugLinearIndexIslandGenerator;

	public UnityEngine.Random.State randomStateIslandCreation;

	public List<int> liIIslandsInThisRun = new List<int>();

	public int iCurrendIslandGenID = -1;

	public UnityEngine.Random.State randomStateBeforeColorGen;

	public int iGameVersion;

	public int iScore;

	public List<int> liIBuildingInventory = new List<int>();

	public int iRequiredScoreForNextPack;

	public int iRequiredScoreForLastPack;

	public int iUnlockedBoosterPacks;

	public int iRequiredScoreForNextIsland = -1;

	public int iScoreWhenEnteredThisIsland;

	public int iCurrentActiveIsland;

	public int iLastGottenBuildingAmountSave;

	public List<int> liIPlusBuildingButtonsIncludingBuildingCounts = new List<int>();

	public int iGameMode;

	public bool bViewingArchivedIsland;

	public bool bBBrainShuffled;

	public List<int> liIBBPackUnlockableRemaining = new List<int>();

	public List<int> liGoUnlockedBuildings = new List<int>();

	public List<int> liGoRemaining = new List<int>();

	public List<int> liGoGuaranteedNext = new List<int>();

	public List<int> liIPlacedBuildings = new List<int>();

	public List<int> liIPlacedBuildingsAmount = new List<int>();

	public UnityEngine.Random.State randomStateNextBuildingChoice;

	public List<int> liIBuildingButtonBuildings = new List<int>();

	public List<int> liIBuildingButtonVariations = new List<int>();

	public List<int> liIBuildingButtonVariationsNext = new List<int>();

	public List<int> liIBBPacksCurrentChoice = new List<int>();

	public Stats statsMatch = new Stats();

	private bool bApplyFileDone;

	public bool BApplyFileDone => bApplyFileDone;

	public void PopulateFile()
	{
		liIStructIds.Clear();
		liSeriTransformsOfStructIds.Clear();
		IslandManager singleton = IslandManager.singleton;
		iDebugLinearIndexIslandGenerator = singleton.iCurrentIslandIndex - 1;
		randomStateIslandCreation = singleton.randomStateCreation;
		liIIslandsInThisRun = singleton.liIIslandsInThisRun;
		List<StructureID> liStructIDRegister = SaveLoadManager.liStructIDRegister;
		List<GameObject> liGoStructIDRegister = SaveLoadManager.liGoStructIDRegister;
		_ = IslandManager.singleton.transform.position;
		for (int i = 0; i < liStructIDRegister.Count; i++)
		{
			StructureID structureID = liStructIDRegister[i];
			Building component = liGoStructIDRegister[i].GetComponent<Building>();
			liIStructIds.Add(structureID.iID);
			if ((bool)component)
			{
				liSeriTransformsOfStructIds.Add(new SeriTransform(structureID.transform, component.iVariation));
			}
			else
			{
				liSeriTransformsOfStructIds.Add(new SeriTransform(structureID.transform));
			}
		}
		LocalGameManager singleton2 = LocalGameManager.singleton;
		iGameVersion = singleton2.iGameVersionCurrentlyPlayingOn;
		iScore = singleton2.IScore;
		iCurrentActiveIsland = singleton2.ICurrentActiveIsland;
		iRequiredScoreForNextPack = singleton2.IRequiredScoreForNextPack;
		iRequiredScoreForLastPack = singleton2.IRequiredScoreForLastPack;
		iUnlockedBoosterPacks = singleton2.IUnlockedBoosterPacks;
		iRequiredScoreForNextIsland = singleton2.IRequiredScoreForNextIsland;
		iScoreWhenEnteredThisIsland = singleton2.IScoreWhenEnteredThisIsland;
		iLastGottenBuildingAmountSave = singleton2.iLastGottenBuildingAmountSave;
		liIPlusBuildingButtonsIncludingBuildingCounts = singleton2.liIPlusBuildingButtonsIncludingBuildingCounts;
		iGameMode = (int)singleton2.GameMode;
		bViewingArchivedIsland = singleton2.bViewingArchivedIsland;
		List<GameObject> liGoResorcesInTransition = FeedbackManager.Singleton.liGoResorcesInTransition;
		foreach (GameObject item in liGoResorcesInTransition)
		{
			singleton2.AddBuildingToInventory(item, 1);
		}
		liIBuildingInventory.Clear();
		foreach (GameObject key in singleton2.dicBuildingInventory.Keys)
		{
			liIBuildingInventory.Add(key.GetComponent<StructureID>().iID);
			liIBuildingInventory.Add(singleton2.dicBuildingInventory[key]);
		}
		foreach (GameObject item2 in liGoResorcesInTransition)
		{
			singleton2.AddBuildingToInventory(item2, -1);
		}
		BuildorderBrainB singleton3 = BuildorderBrainB.singleton;
		bBBrainShuffled = singleton3.bShuffled;
		liIBBPackUnlockableRemaining = singleton3.LiIBBPacksToIndecies(singleton3.liBBPackUnlockableRemaining);
		PopulateGoList(ref liGoUnlockedBuildings, singleton3.liGoUnlockedBuildings);
		PopulateGoList(ref liGoRemaining, singleton3.liGoRemaining);
		PopulateGoList(ref liGoGuaranteedNext, singleton3.liGoGuaranteedNext);
		PopulateGoList(ref liIPlacedBuildings, singleton3.inventoryReceivedBuildings.liT);
		liIPlacedBuildingsAmount = LiCopy(singleton3.inventoryReceivedBuildings.liIAmount);
		randomStateNextBuildingChoice = singleton3.randomStateForNextChoice;
		PopulateGoList(ref liIBuildingButtonBuildings, UiBuildingButtonManager.singleton.LiGoGetBuildingsOfExistingButtons());
		liIBuildingButtonVariations.Clear();
		if (liIBuildingButtonVariationsNext == null)
		{
			liIBuildingButtonVariationsNext = new List<int>();
		}
		liIBuildingButtonVariationsNext.Clear();
		foreach (UiBuildingButton item3 in UiBuildingButtonManager.singleton.LiBuildingButtonsExisting)
		{
			liIBuildingButtonVariations.Add(item3.iBuildingSeed);
			liIBuildingButtonVariationsNext.Add(item3.iBuildingSeed);
		}
		liIBBPacksCurrentChoice = singleton3.LiIBBPacksToIndecies(UIBuildingChoice.Singleton.LiBBPacksSave);
		statsMatch = StatsManager.statsMatch;
	}

	private void PopulateGoList(ref List<int> _liIFill, List<GameObject> _liGoRead)
	{
		_liIFill.Clear();
		foreach (GameObject item in _liGoRead)
		{
			_liIFill.Add(item.GetComponent<StructureID>().iID);
		}
	}

	private List<T> LiCopy<T>(List<T> _liI)
	{
		List<T> list = new List<T>();
		for (int i = 0; i < _liI.Count; i++)
		{
			list.Add(_liI[i]);
		}
		return list;
	}

	public void ToFlatBuffer(FlatBufferBuilder builder)
	{
		VectorOffset? vectorOffset = WriteIntVectorToFlatBuffer(builder, liIStructIds, SaveData.StartIStructIdsVector);
		VectorOffset? vectorOffset2 = null;
		if (liSeriTransformsOfStructIds != null && liSeriTransformsOfStructIds.Count > 0)
		{
			SaveData.StartSeriTransformsOfStructIdsVector(builder, liSeriTransformsOfStructIds.Count);
			for (int num = liSeriTransformsOfStructIds.Count - 1; num >= 0; num--)
			{
				SeriTransform seriTransform = liSeriTransformsOfStructIds[num];
				Islanders.Transform.CreateTransform(builder, seriTransform.sv3Position.fX, seriTransform.sv3Position.fY, seriTransform.sv3Position.fZ, seriTransform.sqRotation.fX, seriTransform.sqRotation.fY, seriTransform.sqRotation.fZ, seriTransform.sqRotation.fW, seriTransform.sv3LocalScale.fX, seriTransform.sv3LocalScale.fY, seriTransform.sv3LocalScale.fZ, seriTransform.iVariation);
			}
			vectorOffset2 = builder.EndVector();
		}
		VectorOffset? vectorOffset3 = WriteIntVectorToFlatBuffer(builder, liIPlaceableIds, SaveData.StartIPlaceableIdsVector);
		VectorOffset? vectorOffset4 = null;
		if (liSeriTransOfPlaceables != null && liSeriTransOfPlaceables.Count > 0)
		{
			SaveData.StartSeriTransOfPlaceablesVector(builder, liSeriTransOfPlaceables.Count);
			for (int num2 = liSeriTransOfPlaceables.Count - 1; num2 >= 0; num2--)
			{
				SeriTransform seriTransform2 = liSeriTransOfPlaceables[num2];
				Islanders.Transform.CreateTransform(builder, seriTransform2.sv3Position.fX, seriTransform2.sv3Position.fY, seriTransform2.sv3Position.fZ, seriTransform2.sqRotation.fX, seriTransform2.sqRotation.fY, seriTransform2.sqRotation.fZ, seriTransform2.sqRotation.fW, seriTransform2.sv3LocalScale.fX, seriTransform2.sv3LocalScale.fY, seriTransform2.sv3LocalScale.fZ, seriTransform2.iVariation);
			}
			vectorOffset4 = builder.EndVector();
		}
		VectorOffset? vectorOffset5 = WriteIntVectorToFlatBuffer(builder, liIIslandsInThisRun, SaveData.StartIIslandsInThisRunVector);
		StringOffset randomStateIslandCreationOffset = builder.CreateString(JsonUtility.ToJson(randomStateIslandCreation, prettyPrint: false));
		StringOffset randomStateBeforeColorGenOffset = builder.CreateString(JsonUtility.ToJson(randomStateBeforeColorGen, prettyPrint: false));
		StringOffset randomStateNextBuildingChoiceOffset = builder.CreateString(JsonUtility.ToJson(randomStateNextBuildingChoice));
		VectorOffset? vectorOffset6 = WriteIntVectorToFlatBuffer(builder, liIBuildingInventory, SaveData.StartIBuildingInventoryVector);
		VectorOffset? vectorOffset7 = WriteIntVectorToFlatBuffer(builder, liIPlusBuildingButtonsIncludingBuildingCounts, SaveData.StartIPlusBuildingButtonsIncludingBuildingCountsVector);
		VectorOffset? vectorOffset8 = WriteIntVectorToFlatBuffer(builder, liIBBPackUnlockableRemaining, SaveData.StartIBBPackUnlockableRemainingVector);
		VectorOffset? vectorOffset9 = WriteIntVectorToFlatBuffer(builder, liGoUnlockedBuildings, SaveData.StartGoUnlockedBuildingsVector);
		VectorOffset? vectorOffset10 = WriteIntVectorToFlatBuffer(builder, liGoRemaining, SaveData.StartGoRemainingVector);
		VectorOffset? vectorOffset11 = WriteIntVectorToFlatBuffer(builder, liGoGuaranteedNext, SaveData.StartGoGuaranteedNextVector);
		VectorOffset? vectorOffset12 = WriteIntVectorToFlatBuffer(builder, liIPlacedBuildings, SaveData.StartIPlacedBuildingsVector);
		VectorOffset? vectorOffset13 = WriteIntVectorToFlatBuffer(builder, liIPlacedBuildingsAmount, SaveData.StartIPlacedBuildingsAmountVector);
		VectorOffset? vectorOffset14 = WriteIntVectorToFlatBuffer(builder, liIBuildingButtonBuildings, SaveData.StartIBuildingButtonBuildingsVector);
		VectorOffset? vectorOffset15 = WriteIntVectorToFlatBuffer(builder, liIBuildingButtonVariations, SaveData.StartIBuildingButtonVariationsVector);
		VectorOffset? vectorOffset16 = WriteIntVectorToFlatBuffer(builder, liIBuildingButtonVariationsNext, SaveData.StartIBuildingButtonVariationsNextVector);
		VectorOffset? vectorOffset17 = WriteIntVectorToFlatBuffer(builder, liIBBPacksCurrentChoice, SaveData.StartIBBPacksCurrentChoiceVector);
		Offset<Islanders.Stats> statsMatchOffset = statsMatch.ToFlatBuffer(builder);
		SaveData.StartSaveData(builder);
		if (vectorOffset.HasValue)
		{
			SaveData.AddIStructIds(builder, vectorOffset.Value);
		}
		if (vectorOffset2.HasValue)
		{
			SaveData.AddSeriTransformsOfStructIds(builder, vectorOffset2.Value);
		}
		if (vectorOffset3.HasValue)
		{
			SaveData.AddIPlaceableIds(builder, vectorOffset3.Value);
		}
		if (vectorOffset4.HasValue)
		{
			SaveData.AddSeriTransOfPlaceables(builder, vectorOffset4.Value);
		}
		SaveData.AddDebugLinearIndexIslandGenerator(builder, iDebugLinearIndexIslandGenerator);
		SaveData.AddRandomStateIslandCreation(builder, randomStateIslandCreationOffset);
		if (vectorOffset5.HasValue)
		{
			SaveData.AddIIslandsInThisRun(builder, vectorOffset5.Value);
		}
		SaveData.AddCurrentIslandGenID(builder, iCurrentActiveIsland);
		SaveData.AddRandomStateBeforeColorGen(builder, randomStateBeforeColorGenOffset);
		SaveData.AddGameVersion(builder, iGameVersion);
		SaveData.AddScore(builder, iScore);
		if (vectorOffset6.HasValue)
		{
			SaveData.AddIBuildingInventory(builder, vectorOffset6.Value);
		}
		SaveData.AddRequiredScoreForNextPack(builder, iRequiredScoreForNextPack);
		SaveData.AddRequiredScoreForLastPack(builder, iRequiredScoreForLastPack);
		SaveData.AddUnlockedBoosterPacks(builder, iUnlockedBoosterPacks);
		SaveData.AddRequiredScoreForNextIsland(builder, iRequiredScoreForNextIsland);
		SaveData.AddScoreWhenEnteredThisIsland(builder, iScoreWhenEnteredThisIsland);
		SaveData.AddCurrentActiveIsland(builder, iCurrentActiveIsland);
		SaveData.AddLastGottenBuildingAmountSave(builder, iLastGottenBuildingAmountSave);
		if (vectorOffset7.HasValue)
		{
			SaveData.AddIPlusBuildingButtonsIncludingBuildingCounts(builder, vectorOffset7.Value);
		}
		SaveData.AddGameMode(builder, iGameMode);
		SaveData.AddViewingArchivedIsland(builder, bViewingArchivedIsland);
		SaveData.AddBBrainShuffled(builder, bBBrainShuffled);
		if (vectorOffset8.HasValue)
		{
			SaveData.AddIBBPackUnlockableRemaining(builder, vectorOffset8.Value);
		}
		if (vectorOffset9.HasValue)
		{
			SaveData.AddGoUnlockedBuildings(builder, vectorOffset9.Value);
		}
		if (vectorOffset10.HasValue)
		{
			SaveData.AddGoRemaining(builder, vectorOffset10.Value);
		}
		if (vectorOffset11.HasValue)
		{
			SaveData.AddGoGuaranteedNext(builder, vectorOffset11.Value);
		}
		if (vectorOffset12.HasValue)
		{
			SaveData.AddIPlacedBuildings(builder, vectorOffset12.Value);
		}
		if (vectorOffset13.HasValue)
		{
			SaveData.AddIPlacedBuildingsAmount(builder, vectorOffset13.Value);
		}
		SaveData.AddRandomStateNextBuildingChoice(builder, randomStateNextBuildingChoiceOffset);
		if (vectorOffset14.HasValue)
		{
			SaveData.AddIBuildingButtonBuildings(builder, vectorOffset14.Value);
		}
		if (vectorOffset15.HasValue)
		{
			SaveData.AddIBuildingButtonVariations(builder, vectorOffset15.Value);
		}
		if (vectorOffset16.HasValue)
		{
			SaveData.AddIBuildingButtonVariationsNext(builder, vectorOffset16.Value);
		}
		if (vectorOffset17.HasValue)
		{
			SaveData.AddIBBPacksCurrentChoice(builder, vectorOffset17.Value);
		}
		SaveData.AddStatsMatch(builder, statsMatchOffset);
		Offset<SaveData> offset = SaveData.EndSaveData(builder);
		SaveData.FinishSaveDataBuffer(builder, offset);
	}

	public void FromFlatBuffer(SaveData saveDataBuffer)
	{
		ReadIntVectorFromFlatBuffer(ref liIStructIds, saveDataBuffer.IStructIdsLength, ((SaveData)saveDataBuffer).IStructIds);
		ReadTransformVectorFromFlatBuffer(ref liSeriTransformsOfStructIds, saveDataBuffer.SeriTransformsOfStructIdsLength, ((SaveData)saveDataBuffer).SeriTransformsOfStructIds);
		ReadIntVectorFromFlatBuffer(ref liIPlaceableIds, saveDataBuffer.IPlaceableIdsLength, ((SaveData)saveDataBuffer).IPlaceableIds);
		ReadTransformVectorFromFlatBuffer(ref liSeriTransOfPlaceables, saveDataBuffer.SeriTransOfPlaceablesLength, ((SaveData)saveDataBuffer).SeriTransOfPlaceables);
		iDebugLinearIndexIslandGenerator = saveDataBuffer.DebugLinearIndexIslandGenerator;
		randomStateIslandCreation = JsonUtility.FromJson<UnityEngine.Random.State>(saveDataBuffer.RandomStateIslandCreation);
		ReadIntVectorFromFlatBuffer(ref liIIslandsInThisRun, saveDataBuffer.IIslandsInThisRunLength, ((SaveData)saveDataBuffer).IIslandsInThisRun);
		iCurrendIslandGenID = saveDataBuffer.CurrentIslandGenID;
		randomStateBeforeColorGen = JsonUtility.FromJson<UnityEngine.Random.State>(saveDataBuffer.RandomStateBeforeColorGen);
		iGameVersion = saveDataBuffer.GameVersion;
		iScore = saveDataBuffer.Score;
		ReadIntVectorFromFlatBuffer(ref liIBuildingInventory, saveDataBuffer.IBuildingInventoryLength, ((SaveData)saveDataBuffer).IBuildingInventory);
		iRequiredScoreForNextPack = saveDataBuffer.RequiredScoreForNextPack;
		iRequiredScoreForLastPack = saveDataBuffer.RequiredScoreForLastPack;
		iUnlockedBoosterPacks = saveDataBuffer.UnlockedBoosterPacks;
		iRequiredScoreForNextIsland = saveDataBuffer.RequiredScoreForNextIsland;
		iScoreWhenEnteredThisIsland = saveDataBuffer.ScoreWhenEnteredThisIsland;
		iCurrentActiveIsland = saveDataBuffer.CurrentActiveIsland;
		iLastGottenBuildingAmountSave = saveDataBuffer.LastGottenBuildingAmountSave;
		ReadIntVectorFromFlatBuffer(ref liIPlusBuildingButtonsIncludingBuildingCounts, saveDataBuffer.IPlusBuildingButtonsIncludingBuildingCountsLength, ((SaveData)saveDataBuffer).IPlusBuildingButtonsIncludingBuildingCounts);
		iGameMode = saveDataBuffer.GameMode;
		bViewingArchivedIsland = saveDataBuffer.ViewingArchivedIsland;
		bBBrainShuffled = saveDataBuffer.BBrainShuffled;
		ReadIntVectorFromFlatBuffer(ref liIBBPackUnlockableRemaining, saveDataBuffer.IBBPackUnlockableRemainingLength, ((SaveData)saveDataBuffer).IBBPackUnlockableRemaining);
		ReadIntVectorFromFlatBuffer(ref liGoUnlockedBuildings, saveDataBuffer.GoUnlockedBuildingsLength, ((SaveData)saveDataBuffer).GoUnlockedBuildings);
		ReadIntVectorFromFlatBuffer(ref liGoRemaining, saveDataBuffer.GoRemainingLength, ((SaveData)saveDataBuffer).GoRemaining);
		ReadIntVectorFromFlatBuffer(ref liGoGuaranteedNext, saveDataBuffer.GoGuaranteedNextLength, ((SaveData)saveDataBuffer).GoGuaranteedNext);
		ReadIntVectorFromFlatBuffer(ref liIPlacedBuildings, saveDataBuffer.IPlacedBuildingsLength, ((SaveData)saveDataBuffer).IPlacedBuildings);
		ReadIntVectorFromFlatBuffer(ref liIPlacedBuildingsAmount, saveDataBuffer.IPlacedBuildingsAmountLength, ((SaveData)saveDataBuffer).IPlacedBuildingsAmount);
		randomStateNextBuildingChoice = JsonUtility.FromJson<UnityEngine.Random.State>(saveDataBuffer.RandomStateNextBuildingChoice);
		ReadIntVectorFromFlatBuffer(ref liIBuildingButtonBuildings, saveDataBuffer.IBuildingButtonBuildingsLength, ((SaveData)saveDataBuffer).IBuildingButtonBuildings);
		ReadIntVectorFromFlatBuffer(ref liIBuildingButtonVariations, saveDataBuffer.IBuildingButtonVariationsLength, ((SaveData)saveDataBuffer).IBuildingButtonVariations);
		ReadIntVectorFromFlatBuffer(ref liIBuildingButtonVariationsNext, saveDataBuffer.IBuildingButtonVariationsNextLength, ((SaveData)saveDataBuffer).IBuildingButtonVariationsNext);
		ReadIntVectorFromFlatBuffer(ref liIBBPacksCurrentChoice, saveDataBuffer.IBBPacksCurrentChoiceLength, ((SaveData)saveDataBuffer).IBBPacksCurrentChoice);
		if (statsMatch == null)
		{
			statsMatch = new Stats();
		}
		if (saveDataBuffer.StatsMatch.HasValue)
		{
			statsMatch.FromFlatBuffer(saveDataBuffer.StatsMatch.Value);
		}
	}

	public static VectorOffset? WriteIntVectorToFlatBuffer(FlatBufferBuilder builder, List<int> intVector, Action<FlatBufferBuilder, int> startFunction)
	{
		if (intVector != null && intVector.Count > 0)
		{
			startFunction(builder, intVector.Count);
			for (int num = intVector.Count - 1; num >= 0; num--)
			{
				builder.AddInt(intVector[num]);
			}
			return builder.EndVector();
		}
		return null;
	}

	public static void ReadIntVectorFromFlatBuffer(ref List<int> intVector, int size, Func<int, int> getElementFunc)
	{
		if (intVector == null)
		{
			intVector = new List<int>(size);
		}
		intVector.Clear();
		for (int i = 0; i < size; i++)
		{
			intVector.Add(getElementFunc(i));
		}
	}

	public static void ReadTransformVectorFromFlatBuffer(ref List<SeriTransform> transformVector, int size, Func<int, Islanders.Transform?> getElementFunc)
	{
		if (transformVector == null)
		{
			transformVector = new List<SeriTransform>(size);
		}
		transformVector.Clear();
		for (int i = 0; i < size; i++)
		{
			Islanders.Transform? transform = getElementFunc(i);
			if (transform.HasValue)
			{
				transformVector.Add(new SeriTransform(transform.Value.Position.X, transform.Value.Position.Y, transform.Value.Position.Z, transform.Value.LocalScale.X, transform.Value.LocalScale.Y, transform.Value.LocalScale.Z, transform.Value.Rotation.X, transform.Value.Rotation.Y, transform.Value.Rotation.Z, transform.Value.Rotation.W, transform.Value.Variation));
			}
		}
	}

	public IEnumerator ApplyFile(ELoadMode _eLoadMode = ELoadMode.Normal)
	{
		bApplyFileDone = false;
		LocalGameManager localGameManager = LocalGameManager.singleton;
		IslandManager islandManager = IslandManager.singleton;
		if (liIPlaceableIds == null)
		{
			liIPlaceableIds = new List<int>();
			liSeriTransOfPlaceables = new List<SeriTransform>();
		}
		if (_eLoadMode == ELoadMode.NewGame || _eLoadMode == ELoadMode.NextIsland)
		{
			liIPlaceableIds.Clear();
			liSeriTransOfPlaceables.Clear();
			randomStateBeforeColorGen = default(UnityEngine.Random.State);
		}
		if (_eLoadMode == ELoadMode.Normal)
		{
			islandManager.liIIslandsInThisRun = liIIslandsInThisRun;
		}
		if (_eLoadMode == ELoadMode.NewGame)
		{
			islandManager.liIIslandsInThisRun.Clear();
		}
		UiBuildingButtonManager.singleton.GoSelectedButton = null;
		if (iRequiredScoreForNextIsland < 0)
		{
			iRequiredScoreForNextIsland = islandManager.LiIslandsAndGoals[0].iScoreGoal;
		}
		if (_eLoadMode == ELoadMode.Normal)
		{
			localGameManager.iGameVersionCurrentlyPlayingOn = iGameVersion;
		}
		else
		{
			localGameManager.iGameVersionCurrentlyPlayingOn = 1;
		}
		if (_eLoadMode == ELoadMode.Normal)
		{
			StatsManager.statsMatch = statsMatch;
		}
		if (_eLoadMode == ELoadMode.NewGame)
		{
			statsMatch = new Stats();
			StatsManager.statsMatch = statsMatch;
		}
		islandManager.iCurrentIslandIndex = iDebugLinearIndexIslandGenerator;
		UnityEngine.Random.state = randomStateIslandCreation;
		LocalGameManager.EGameMode gameMode = ((_eLoadMode != 0) ? LocalGameManager.singleton.GameMode : ((LocalGameManager.EGameMode)iGameMode));
		islandManager.StartCoroutine(islandManager.CreateNewIsland(gameMode, _eLoadMode));
		while (!islandManager.BCreateNewIslandDone)
		{
			yield return null;
		}
		if (_eLoadMode == ELoadMode.Normal)
		{
			List<StructureID> liStructIDRegister = SaveLoadManager.liStructIDRegister;
			for (int num = liStructIDRegister.Count - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(liStructIDRegister[num].gameObject);
			}
			yield return null;
			List<CosmeticHandler> cosmeticsHandlers = new List<CosmeticHandler>();
			_ = IslandManager.singleton.transform.position;
			for (int i = 0; i < liIStructIds.Count; i++)
			{
				int iTranslateID = liIStructIds[i];
				SeriTransform seriTransform = liSeriTransformsOfStructIds[i];
				GameObject gameObject = UnityEngine.Object.Instantiate(StructureIDTranslator.GoGetGameObjectFromID(iTranslateID), seriTransform.sv3Position.V3, seriTransform.sqRotation.Q);
				gameObject.transform.localScale = seriTransform.sv3LocalScale.V3;
				Building component = gameObject.GetComponent<Building>();
				if ((bool)component)
				{
					component.SelectVariation(seriTransform.iVariation);
					component.PlaceQuick();
				}
				CosmeticHandler cosmetic = gameObject.GetComponentInChildren<CosmeticHandler>();
				if ((bool)cosmetic && cosmetic.HasCosmeticActivated())
				{
					cosmeticsHandlers.Add(cosmetic);
					CosmeticHandler cosmeticHandler = cosmetic;
					cosmeticHandler.OnComesticApplied = (Action)Delegate.Combine(cosmeticHandler.OnComesticApplied, (Action)delegate
					{
						cosmeticsHandlers.Remove(cosmetic);
					});
					cosmetic.CheckAndApplyCosmetics();
				}
			}
			yield return null;
			while (cosmeticsHandlers.Count != 0)
			{
				yield return null;
			}
		}
		localGameManager.StopAllCoroutines();
		if (_eLoadMode == ELoadMode.Normal)
		{
			localGameManager.liIPlusBuildingButtonsIncludingBuildingCounts = new List<int>(liIPlusBuildingButtonsIncludingBuildingCounts);
			localGameManager.bViewingArchivedIsland = bViewingArchivedIsland;
			if (bViewingArchivedIsland)
			{
				iScore = 0;
				iRequiredScoreForNextPack = 20;
				iRequiredScoreForLastPack = 0;
				liIBuildingInventory.Clear();
				iUnlockedBoosterPacks = 0;
				localGameManager.liIPlusBuildingButtonsIncludingBuildingCounts = new List<int>();
				iGameMode = 1;
			}
			localGameManager.SetGameMode((LocalGameManager.EGameMode)iGameMode);
		}
		else
		{
			localGameManager.liIPlusBuildingButtonsIncludingBuildingCounts = new List<int>();
			localGameManager.bViewingArchivedIsland = false;
		}
		if (_eLoadMode != ELoadMode.NextIsland)
		{
			localGameManager.iScore = iScore;
			localGameManager.ICurrentActiveIsland = iCurrentActiveIsland;
			localGameManager.IRequiredScoreForNextPack = iRequiredScoreForNextPack;
			localGameManager.IRequiredScoreForLastPack = iRequiredScoreForLastPack;
			localGameManager.IRequiredScoreForNextIsland = iRequiredScoreForNextIsland;
			localGameManager.IScoreWhenEnteredThisIsland = iScoreWhenEnteredThisIsland;
		}
		localGameManager.dicBuildingInventory.Clear();
		for (int j = 0; j < liIBuildingInventory.Count; j += 2)
		{
			GameObject gameObject2 = StructureIDTranslator.GoGetGameObjectFromID(liIBuildingInventory[j]);
			int value = liIBuildingInventory[j + 1];
			localGameManager.dicBuildingInventory.Add(gameObject2, value);
			localGameManager.EnsureBuildingButtonIsPresent(gameObject2);
		}
		localGameManager.IUnlockedBoosterPacks = iUnlockedBoosterPacks;
		localGameManager.CalculateBoosterPackProgress();
		localGameManager.CheckForNewIsland(_bOverrideAvailable: true);
		UIPlusBuildingsButton.singleton.UpdateButton();
		yield return null;
		BuildorderBrainB singleton = BuildorderBrainB.singleton;
		singleton.bShuffled = bBBrainShuffled;
		singleton.liBBPackUnlockableRemaining = singleton.LiBBPackFromIndecies(liIBBPackUnlockableRemaining);
		ApplyGoList(ref singleton.liGoUnlockedBuildings, liGoUnlockedBuildings);
		ApplyGoList(ref singleton.liGoRemaining, liGoRemaining);
		ApplyGoList(ref singleton.liGoGuaranteedNext, liGoGuaranteedNext);
		ApplyGoList(ref singleton.inventoryReceivedBuildings.liT, liIPlacedBuildings);
		singleton.inventoryReceivedBuildings.liIAmount = LiCopy(liIPlacedBuildingsAmount);
		if (_eLoadMode == ELoadMode.Normal)
		{
			singleton.randomStateForNextChoice = randomStateNextBuildingChoice;
		}
		else
		{
			singleton.randomStateForNextChoice = UnityEngine.Random.state;
		}
		yield return null;
		List<GameObject> _liGoFill = new List<GameObject>();
		ApplyGoList(ref _liGoFill, liIBuildingButtonBuildings);
		for (int k = 0; k < _liGoFill.Count; k++)
		{
			UiBuildingButton uiBuildingButton = localGameManager.EnsureBuildingButtonIsPresent(_liGoFill[k]);
			uiBuildingButton.iBuildingSeed = liIBuildingButtonVariations[k];
			if (liIBuildingButtonVariationsNext == null)
			{
				liIBuildingButtonVariationsNext = new List<int>();
			}
			if (liIBuildingButtonVariationsNext.Count > 0)
			{
				uiBuildingButton.iBuildingSeedNext = liIBuildingButtonVariationsNext[k];
			}
		}
		yield return null;
		UiProgressBar.singleton.Jump();
		UiScore.Singleton.ForceUpdateImmediate();
		localGameManager.CheckForNewBoosterPack();
		yield return null;
		AssetFolder.UnpackAll();
		bApplyFileDone = true;
		yield return null;
	}

	private void ApplyGoList(ref List<GameObject> _liGoFill, List<int> _liIRead)
	{
		_liGoFill.Clear();
		foreach (int item in _liIRead)
		{
			_liGoFill.Add(StructureIDTranslator.GoGetGameObjectFromID(item));
		}
	}
}

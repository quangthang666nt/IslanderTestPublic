using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoinManager : MonoBehaviour
{
	private static CoinManager singleton;

	public static UnityEvent eventOnCoinGather;

	public static UnityEvent eventOnCoinSpend;

	public static UnityEvent eventOnPositiveCoinSpawn;

	[Help("This script manages the coin animations.", MessageType.Info)]
	[SerializeField]
	private GameObject goCoinParticlePrefab;

	[SerializeField]
	private GameObject goCNegativeCoinParticlePrefab;

	[SerializeField]
	private Transform transCoinTargetPosInUI;

	[SerializeField]
	private float fCoinTargetYOffet;

	[SerializeField]
	private float fCoinToBuildingYOffset = 0.5f;

	[SerializeField]
	public float fCoinToBarTime;

	[SerializeField]
	public float fCointToBuildingTime;

	[SerializeField]
	public float fCoinSpawnInterval;

	[SerializeField]
	private AnimationCurve acCoinToBarXZ = new AnimationCurve();

	[SerializeField]
	private AnimationCurve acCoinToBarY = new AnimationCurve();

	[SerializeField]
	private AnimationCurve acCoinToBuilding = new AnimationCurve();

	[SerializeField]
	private AnimationCurve acCoinToBuildingY = new AnimationCurve();

	[SerializeField]
	private AnimationCurve acCoinFromBarY = new AnimationCurve();

	private Vector3 v3CoinTargetPosition = Vector3.zero;

	private int iCoinToBarFeedbacksRunning;

	private int iCoinsInPositiveBarTransition;

	private int iCoinsInNegativeBarTransition;

	private int iCoinsInTransitionBalance;

	private bool bInPositiveCoinFeedback;

	private UiBuildingButtonManager UiBBM;

	private List<GameObject> liGoCurrentCointPrefabs = new List<GameObject>();

	public static CoinManager Singleton => singleton;

	public bool BCoinBarFeedbackRunning => iCoinToBarFeedbacksRunning > 0;

	public bool BInPositiveCoinFeedback => bInPositiveCoinFeedback;

	private void Awake()
	{
		if (singleton != null)
		{
			Object.Destroy(this);
			return;
		}
		singleton = this;
		eventOnCoinGather = new UnityEvent();
		eventOnCoinSpend = new UnityEvent();
		eventOnPositiveCoinSpawn = new UnityEvent();
	}

	private void Start()
	{
		UiBBM = UiBuildingButtonManager.singleton;
		UiBBM.eventOnBuildingPlace.AddListener(SpawnCoinsOnBuildingPlace);
	}

	private void Update()
	{
		v3CoinTargetPosition = UICameraSpaceHelper.ScreenPointToCanvasPointUnscaledZ(UICameraSpaceHelper.CanvasPointToScreenPoint(transCoinTargetPosInUI.position) + Vector3.forward * fCoinTargetYOffet);
	}

	public int IGetCoinTransitionBalance()
	{
		return iCoinsInTransitionBalance;
	}

	public void InstantlyFinishOffAllCoinTransitions()
	{
		LocalGameManager.singleton.IScore += IGetCoinTransitionBalance();
		for (int num = liGoCurrentCointPrefabs.Count - 1; num >= 0; num--)
		{
			Object.Destroy(liGoCurrentCointPrefabs[num]);
		}
		liGoCurrentCointPrefabs.Clear();
		StopAllCoroutines();
		iCoinsInPositiveBarTransition = 0;
		iCoinsInNegativeBarTransition = 0;
		iCoinToBarFeedbacksRunning = 0;
		bInPositiveCoinFeedback = false;
		iCoinsInTransitionBalance = 0;
	}

	private void SpawnCoinsOnBuildingPlace()
	{
		if (LocalGameManager.singleton.GameMode != LocalGameManager.EGameMode.Sandbox)
		{
			StartCoroutine(SpawnCoinFeedback(UiBBM.IScorePreview, UiBBM.GoBuildingPreview.transform, UiBBM.LiScoreContributorsPreview));
		}
	}

	private IEnumerator SpawnCoinFeedback(int _iCoinAmount, Transform _transPlacedBuilding, List<ScoreContributorData> _liContributors)
	{
		iCoinsInTransitionBalance += _iCoinAmount;
		iCoinToBarFeedbacksRunning++;
		List<ScoreContributorData> liContributors = new List<ScoreContributorData>(_liContributors);
		WaitForSeconds wfsSpawnIntervall = new WaitForSeconds(fCoinSpawnInterval);
		WaitForSeconds wfsCoinToBuilding = new WaitForSeconds(fCointToBuildingTime);
		WaitForSeconds wfsCoinToBar = new WaitForSeconds(fCoinToBarTime);
		bool bAContributorExists = false;
		int iSpawnedCoins3 = 0;
		bool bACoinSpawned2;
		do
		{
			bACoinSpawned2 = false;
			foreach (ScoreContributorData item in liContributors)
			{
				if (iSpawnedCoins3 < item.iScoreContribution && (bool)item.trans)
				{
					GameObject gameObject = Object.Instantiate(goCoinParticlePrefab, item.trans.position, Quaternion.identity);
					liGoCurrentCointPrefabs.Add(gameObject);
					StartCoroutine(MoveCoin(gameObject.transform, _transPlacedBuilding, fCointToBuildingTime, acCoinToBuilding, acCoinToBuilding, acCoinToBuilding, bIncreaseScore: false, bBarTransition: false, acCoinToBuildingY));
					bACoinSpawned2 = true;
					bAContributorExists = true;
				}
			}
			eventOnPositiveCoinSpawn.Invoke();
			iSpawnedCoins3++;
			yield return wfsSpawnIntervall;
		}
		while (bACoinSpawned2);
		if (bAContributorExists)
		{
			yield return wfsCoinToBuilding;
		}
		if (_iCoinAmount > 0)
		{
			iSpawnedCoins3 = 0;
			while (iSpawnedCoins3 < _iCoinAmount)
			{
				eventOnPositiveCoinSpawn.Invoke();
				if (!_transPlacedBuilding)
				{
					break;
				}
				GameObject gameObject2 = Object.Instantiate(goCoinParticlePrefab, _transPlacedBuilding.transform.position, Quaternion.identity);
				liGoCurrentCointPrefabs.Add(gameObject2);
				StartCoroutine(MoveCoin(gameObject2.transform, transCoinTargetPosInUI, fCoinToBarTime, acCoinToBarXZ, acCoinToBarY, acCoinToBarXZ, bIncreaseScore: true, bBarTransition: true));
				iCoinsInPositiveBarTransition++;
				iSpawnedCoins3++;
				yield return wfsSpawnIntervall;
			}
		}
		if (_iCoinAmount < 0)
		{
			for (int u = 0; u > _iCoinAmount; u--)
			{
				iCoinsInTransitionBalance++;
				LocalGameManager.singleton.IScore--;
				eventOnCoinSpend.Invoke();
				GameObject gameObject3 = Object.Instantiate(goCNegativeCoinParticlePrefab, v3CoinTargetPosition, Quaternion.identity);
				liGoCurrentCointPrefabs.Add(gameObject3);
				StartCoroutine(MoveCoin(gameObject3.transform, _transPlacedBuilding, fCoinToBarTime, acCoinToBarXZ, acCoinFromBarY, acCoinToBarXZ, bIncreaseScore: false, bBarTransition: true));
				iCoinsInNegativeBarTransition++;
				yield return wfsSpawnIntervall;
			}
		}
		iCoinToBarFeedbacksRunning--;
		yield return wfsCoinToBar;
		iSpawnedCoins3 = 0;
		do
		{
			bACoinSpawned2 = false;
			foreach (ScoreContributorData item2 in liContributors)
			{
				if (item2.iScoreContribution < iSpawnedCoins3 && (bool)item2.trans && (bool)_transPlacedBuilding)
				{
					GameObject gameObject4 = Object.Instantiate(goCNegativeCoinParticlePrefab, _transPlacedBuilding.position, Quaternion.identity);
					liGoCurrentCointPrefabs.Add(gameObject4);
					StartCoroutine(MoveCoin(gameObject4.transform, item2.trans, fCointToBuildingTime, acCoinToBuilding, acCoinToBuilding, acCoinToBuilding, bIncreaseScore: false, bBarTransition: false, acCoinToBuildingY));
					bACoinSpawned2 = true;
				}
			}
			iSpawnedCoins3--;
			yield return wfsSpawnIntervall;
		}
		while (bACoinSpawned2);
	}

	private IEnumerator MoveCoin(Transform _transCoin, Transform _transTarget, float _fToTargetTime, AnimationCurve _acX, AnimationCurve _acY, AnimationCurve _acZ, bool bIncreaseScore = false, bool bBarTransition = false, AnimationCurve coinYModifierCurve = null)
	{
		Vector3 v3OriginPosition = _transCoin.position;
		Vector3 zero = Vector3.zero;
		float fTimer = 0f;
		while (fTimer < fCoinToBarTime)
		{
			fTimer += Time.deltaTime;
			float time = Mathf.InverseLerp(0f, _fToTargetTime, fTimer);
			if (!_transTarget)
			{
				break;
			}
			if (_transTarget == transCoinTargetPosInUI)
			{
				zero.x = Mathf.Lerp(v3OriginPosition.x, v3CoinTargetPosition.x, _acX.Evaluate(time));
				zero.z = Mathf.Lerp(v3OriginPosition.z, v3CoinTargetPosition.z, _acZ.Evaluate(time));
				zero.y = Mathf.Lerp(v3OriginPosition.y, v3CoinTargetPosition.y, _acY.Evaluate(time));
				if (coinYModifierCurve != null)
				{
					zero.y += coinYModifierCurve.Evaluate(time) * fCoinToBuildingYOffset;
				}
			}
			else
			{
				zero.x = Mathf.Lerp(v3OriginPosition.x, _transTarget.position.x, _acX.Evaluate(time));
				zero.z = Mathf.Lerp(v3OriginPosition.z, _transTarget.position.z, _acZ.Evaluate(time));
				zero.y = Mathf.Lerp(v3OriginPosition.y, _transTarget.position.y, _acY.Evaluate(time));
				if (coinYModifierCurve != null)
				{
					zero.y += coinYModifierCurve.Evaluate(time) * fCoinToBuildingYOffset;
				}
			}
			_transCoin.position = zero;
			yield return null;
		}
		if (bIncreaseScore)
		{
			iCoinsInTransitionBalance--;
			LocalGameManager.singleton.IScore++;
			eventOnCoinGather.Invoke();
		}
		liGoCurrentCointPrefabs.Remove(_transCoin.gameObject);
		Object.Destroy(_transCoin.gameObject);
		if (!bBarTransition)
		{
			yield break;
		}
		if (bIncreaseScore)
		{
			iCoinsInPositiveBarTransition--;
			if (iCoinsInPositiveBarTransition > 0)
			{
				bInPositiveCoinFeedback = true;
			}
			else
			{
				bInPositiveCoinFeedback = false;
			}
		}
		else
		{
			iCoinsInNegativeBarTransition--;
		}
	}
}

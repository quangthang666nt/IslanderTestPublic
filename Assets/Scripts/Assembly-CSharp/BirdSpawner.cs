using System.Collections;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
	[Header("Spawn Volume Setup")]
	[SerializeField]
	private Vector3 v3SpawnBounds;

	[SerializeField]
	private bool bShowSpawnBoundGizmo;

	[Header("")]
	[Header("Spawn Interval")]
	[SerializeField]
	[Tooltip("Each time a flock of birds is spawned, the spawnInterval is set to a new number between the min and max you specifiy here.")]
	private Vector2 fSpawnIntervalMinMax;

	[SerializeField]
	private int iMaxSpawnAttemptsPerFrame = 20;

	[SerializeField]
	private GameObject[] arBirdGroupPrefabs;

	[Header("")]
	[Header("Debug")]
	[SerializeField]
	private bool bDebugShowFlightPaths;

	private float fTimer;

	private float fCurrSpawnInterval;

	private void Start()
	{
		fCurrSpawnInterval = Random.Range(fSpawnIntervalMinMax.x, fSpawnIntervalMinMax.y);
	}

	private void Update()
	{
		if (fTimer >= fCurrSpawnInterval)
		{
			StartCoroutine(TryToSpawn());
			fTimer = 0f;
			fCurrSpawnInterval = Random.Range(fSpawnIntervalMinMax.x, fSpawnIntervalMinMax.y);
		}
		fTimer += Time.deltaTime;
	}

	private bool BEvaluateSpawnPos(Vector3 v3Pos, GameObject goBirdGroup)
	{
		Vector3 direction = goBirdGroup.transform.rotation * Vector3.forward;
		Ray ray = new Ray(v3Pos, direction);
		RaycastHit hitInfo = new RaycastHit
		{
			distance = 50f
		};
		bool result = !Physics.SphereCast(ray, 0.7f, out hitInfo, 50f);
		if (bDebugShowFlightPaths)
		{
			Debug.DrawRay(v3Pos, direction.normalized * hitInfo.distance, Color.white, 10f);
		}
		return result;
	}

	private IEnumerator TryToSpawn()
	{
		int iSafetyCounter = 0;
		bool bHasSpawned = false;
		while (iSafetyCounter < 100000 && !bHasSpawned)
		{
			for (int i = 0; i < iMaxSpawnAttemptsPerFrame; i++)
			{
				Vector3 vector = V3GetPositionInBounds();
				GameObject gameObject = arBirdGroupPrefabs[Random.Range(0, arBirdGroupPrefabs.Length)];
				if (bDebugShowFlightPaths && BEvaluateSpawnPos(vector, gameObject))
				{
					Object.Instantiate(gameObject, vector, gameObject.transform.rotation);
					bHasSpawned = true;
					break;
				}
			}
			iSafetyCounter++;
			yield return null;
		}
	}

	private Vector3 V3GetPositionInBounds()
	{
		Vector3 position = base.transform.position;
		Vector2 vector = new Vector2(position.x - v3SpawnBounds.x / 2f, position.x + v3SpawnBounds.x / 2f);
		Vector2 vector2 = new Vector2(position.y - v3SpawnBounds.y / 2f, position.y + v3SpawnBounds.y / 2f);
		Vector2 vector3 = new Vector2(position.z - v3SpawnBounds.z / 2f, position.z + v3SpawnBounds.z / 2f);
		float x = Random.Range(vector.x, vector.y);
		float y = Random.Range(vector2.x, vector2.y);
		float z = Random.Range(vector3.x, vector3.y);
		return new Vector3(x, y, z);
	}

	private void OnDrawGizmos()
	{
		if (bShowSpawnBoundGizmo)
		{
			Color yellow = Color.yellow;
			yellow.a = 0.5f;
			Gizmos.color = yellow;
			Gizmos.DrawCube(base.transform.position, v3SpawnBounds);
		}
	}
}

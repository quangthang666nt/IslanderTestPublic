using UnityEngine;

public class ScreenshakeManager : MonoBehaviour
{
	public static ScreenshakeManager singleton;

	private Vector3 v3BasePos;

	private Transform trans;

	private float fShake;

	[Range(0f, 1f)]
	public float fShakeDecayDuration = 0.5f;

	public Vector3 v3MaxPositionShake;

	public Vector3 v3MaxRotationShake;

	public Vector3 v3InstantPosition;

	public Vector3 v3InstantRotation;

	public float fMaxUpdatesPerSec = 60f;

	private Vector3 v3CurrentRotation;

	private Vector3 v3CurrentOffset;

	private float fCooldown;

	private float fX;

	private float fY;

	private float fZ;

	private void Awake()
	{
		trans = base.transform;
		singleton = this;
	}

	private void Start()
	{
		v3BasePos = trans.localPosition;
	}

	private void Update()
	{
		fShake *= Mathf.Pow(fShakeDecayDuration, Time.deltaTime * 10f);
		fCooldown -= Time.deltaTime;
		v3CurrentOffset = Vector3.zero;
		v3CurrentRotation = Vector3.zero;
		if (fCooldown <= 0f)
		{
			fCooldown += 1f / fMaxUpdatesPerSec;
			fCooldown = Mathf.Clamp(fCooldown, -1f, 1f);
			if (fShake > 0.01f)
			{
				fX = v3MaxPositionShake.x * (Random.value - 0.5f) * 2f;
				fY = v3MaxPositionShake.y * (Random.value - 0.5f) * 2f;
				fZ = v3MaxPositionShake.z * (Random.value - 0.5f) * 2f;
				v3CurrentOffset = new Vector3(fX, fY, fZ) * fShake;
				fX = v3MaxRotationShake.x * (Random.value - 0.5f) * 2f;
				fY = v3MaxRotationShake.y * (Random.value - 0.5f) * 2f;
				fZ = v3MaxRotationShake.z * (Random.value - 0.5f) * 2f;
				v3CurrentRotation = new Vector3(fX, fY, fZ) * fShake;
			}
		}
		v3CurrentRotation += v3InstantRotation * fShake;
		trans.localRotation = Quaternion.Euler(v3CurrentRotation);
		v3CurrentOffset += v3InstantPosition * fShake;
		trans.localPosition = v3BasePos + v3CurrentOffset;
	}

	public static void Shake(float _fStrength)
	{
		singleton.fShake = Mathf.Max(singleton.fShake, _fStrength);
	}
}

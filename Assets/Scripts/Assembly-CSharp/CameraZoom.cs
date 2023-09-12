using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
	public static CameraZoom singleton;

	[SerializeField]
	private Camera camUICam;

	[SerializeField]
	private bool bZoomWithPosition = true;

	[SerializeField]
	private Vector3 v3OriginalLocalPos = Vector3.zero;

	[SerializeField]
	private float fMaxZoomOffset = 2f;

	[SerializeField]
	private float fZoomStep = 0.1f;

	[SerializeField]
	private float fZoomInterpolation = 5f;

	[SerializeField]
	private InputManager inputManager;

	[SerializeField]
	private bool bZoomWithFOV;

	[SerializeField]
	private float fStartFOV = 50f;

	[SerializeField]
	private float fMaxFOVOffset = 30f;

	[SerializeField]
	private SaveLoadManager saveLoadManager;

	[SerializeField]
	private float fResetTime = 1f;

	public float m_DefaultZoom = 0.5f;

	private float fCurrentZoom01 = 0.5f;

	private float fTargetZoom;

	private bool bZoomingToDefault;

	private Transform uiCamTransform;

	private Camera cam;

	public float FCurrentZoom01 => fCurrentZoom01;

	private void Awake()
	{
		singleton = this;
	}

	private void Start()
	{
		base.transform.localPosition = v3OriginalLocalPos;
		uiCamTransform = camUICam.transform;
		cam = Camera.main;
		fCurrentZoom01 = m_DefaultZoom;
		fTargetZoom = fCurrentZoom01;
		bZoomingToDefault = false;
		saveLoadManager = SaveLoadManager.singleton;
		inputManager = InputManager.Singleton;
		saveLoadManager.eventOnTransitionStart.AddListener(ResetZoom);
	}

	private void Update()
	{
		if (UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.Leaderboard && !SandboxGenerationView.IsOpened() && UiCanvasManager.Singleton.UIState != UiCanvasManager.EUIState.Settings && saveLoadManager.ETransitionStateCurrent == SaveLoadManager.EtransitionState.NotInTransition && !bZoomingToDefault && !UiCanvasManager.Singleton.IsArchiveIslandOpen())
		{
			InputManager.InputData inputDataCurrent = inputManager.InputDataCurrent;
			fTargetZoom = Mathf.Clamp01(fTargetZoom + fZoomStep * inputDataCurrent.fCameraZoom);
			float num = fCurrentZoom01;
			fCurrentZoom01 = Mathf.Lerp(fCurrentZoom01, fTargetZoom, Time.deltaTime * fZoomInterpolation);
			num = fCurrentZoom01 - num;
			CameraController.singleton.ZoomWithMouse += Mathf.Abs(num);
			UpdateZoom();
		}
	}

	public void SetZoom(float duration, float value)
	{
		StartCoroutine(ZoomToValue(duration, value));
	}

	private void ResetZoom()
	{
		StopAllCoroutines();
		StartCoroutine(ZoomToValue(fResetTime, m_DefaultZoom));
	}

	private void UpdateZoom()
	{
		if (bZoomWithPosition)
		{
			uiCamTransform.localPosition = v3OriginalLocalPos + Vector3.forward * fMaxZoomOffset * fCurrentZoom01;
			base.transform.localPosition = v3OriginalLocalPos + Vector3.forward * fMaxZoomOffset * fCurrentZoom01;
		}
		if (bZoomWithFOV)
		{
			camUICam.fieldOfView = fStartFOV - fMaxFOVOffset * fCurrentZoom01;
			cam.fieldOfView = fStartFOV - fMaxFOVOffset * fCurrentZoom01;
		}
	}

	private IEnumerator ZoomToValue(float duration, float value)
	{
		bZoomingToDefault = true;
		float timer = 0f;
		float startValue = fCurrentZoom01;
		while (timer <= duration)
		{
			float t = Mathf.SmoothStep(0f, 1f, timer / duration);
			fCurrentZoom01 = Mathf.Lerp(startValue, value, t);
			fTargetZoom = fCurrentZoom01;
			UpdateZoom();
			timer += Time.deltaTime;
			yield return null;
		}
		fCurrentZoom01 = value;
		fTargetZoom = fCurrentZoom01;
		UpdateZoom();
		bZoomingToDefault = false;
	}
}

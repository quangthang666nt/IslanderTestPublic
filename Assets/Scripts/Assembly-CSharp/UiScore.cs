using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UiScore : MonoBehaviour
{
	private static UiScore singleton;

	private TextMeshProUGUI tmpScoreDisplay;

	[SerializeField]
	private float fFadebackTime = 0.5f;

	[SerializeField]
	private Color colPositive = Color.white;

	[SerializeField]
	private Color colNegative = Color.white;

	private float fCurrentValue;

	private float fTargetValue;

	private float fFadebackClock;

	private Color colDefault;

	private Color colCurrent;

	private Color colFadeOrigin;

	private float m_LastCurrentValue;

	private Color m_LastColor;

	private int m_LastRequiredScore;

	public static UiScore Singleton => singleton;

	private void Awake()
	{
		if (singleton != null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			singleton = this;
		}
		tmpScoreDisplay = GetComponent<TextMeshProUGUI>();
		colDefault = tmpScoreDisplay.color;
		colCurrent = colDefault;
	}

	private void Update()
	{
		fTargetValue = LocalGameManager.singleton.IScore;
		if (fCurrentValue < fTargetValue)
		{
			colCurrent = colPositive;
			colFadeOrigin = colCurrent;
			fFadebackClock = fFadebackTime;
		}
		else if (fCurrentValue > fTargetValue)
		{
			colCurrent = colNegative;
			colFadeOrigin = colCurrent;
			fFadebackClock = fFadebackTime;
		}
		if (fFadebackClock > 0f)
		{
			fFadebackClock -= Time.deltaTime;
			colCurrent = Color.Lerp(colFadeOrigin, colDefault, Mathf.InverseLerp(fFadebackTime, 0f, fFadebackClock));
		}
		fCurrentValue = fTargetValue;
		if (fCurrentValue != m_LastCurrentValue || colCurrent != m_LastColor || LocalGameManager.singleton.IRequiredScoreForNextPack != m_LastRequiredScore)
		{
			UpdateScoreDisplay();
		}
	}

	private void UpdateScoreDisplay()
	{
		string text = "<color=#" + ColorUtility.ToHtmlStringRGB(colCurrent) + ">" + fCurrentValue + "</color><font=" + LegacyLocalizationManager.LightFont + ">/" + LocalGameManager.singleton.IRequiredScoreForNextPack;
		tmpScoreDisplay.text = text;
		m_LastCurrentValue = fCurrentValue;
		m_LastColor = colCurrent;
		m_LastRequiredScore = LocalGameManager.singleton.IRequiredScoreForNextPack;
	}

	public void ForceUpdateImmediate()
	{
		fCurrentValue = fTargetValue;
	}
}

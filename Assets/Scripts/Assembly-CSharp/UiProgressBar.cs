using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UiProgressBar : MonoBehaviour
{
	public static UiProgressBar singleton;

	private LocalGameManager localGameManager;

	private UiBuildingButtonManager UiBBM;

	private Image image;

	[SerializeField]
	protected Color colValueRising;

	[SerializeField]
	protected Color colValueSinking;

	[SerializeField]
	protected float fFadebackTime = 0.5f;

	private float fCurrentValue;

	private float fFadebackClock;

	private Color colDefault;

	private Color colCurrent;

	private Color colFadeOrigin;

	public void Jump()
	{
		float num = Mathf.InverseLerp(localGameManager.IRequiredScoreForLastPack, localGameManager.IRequiredScoreForNextPack, localGameManager.IScore);
		fCurrentValue = num;
		image.fillAmount = fCurrentValue;
	}

	private void Awake()
	{
		singleton = this;
	}

	private void Start()
	{
		localGameManager = LocalGameManager.singleton;
		UiBBM = UiBuildingButtonManager.singleton;
		image = GetComponent<Image>();
		colCurrent = (colDefault = image.color);
	}

	private void Update()
	{
		float num = Mathf.InverseLerp(localGameManager.IRequiredScoreForLastPack, localGameManager.IRequiredScoreForNextPack, localGameManager.IScore);
		if (num != fCurrentValue)
		{
			if (num > fCurrentValue)
			{
				colCurrent = colValueRising;
				colFadeOrigin = colCurrent;
				fFadebackClock = fFadebackTime;
			}
			else
			{
				colCurrent = colValueSinking;
				colFadeOrigin = colCurrent;
				fFadebackClock = fFadebackTime;
			}
			fCurrentValue = num;
			image.fillAmount = fCurrentValue;
		}
		if (fFadebackClock > 0f)
		{
			fFadebackClock -= Time.deltaTime;
			colCurrent = Color.Lerp(colFadeOrigin, colDefault, Mathf.InverseLerp(fFadebackTime, 0f, fFadebackClock));
			image.color = colCurrent;
		}
		else if (UiBBM.GoBuildingPreview != null && UiBBM.IScorePreview < 0)
		{
			image.color = colDefault;
			float target = Mathf.InverseLerp(localGameManager.IRequiredScoreForLastPack, localGameManager.IRequiredScoreForNextPack, localGameManager.IScore + UiBBM.IScorePreview);
			float fillAmount = Mathf.MoveTowards(image.fillAmount, target, 3.5f * Time.deltaTime);
			image.fillAmount = fillAmount;
		}
		else if (fCurrentValue != image.fillAmount)
		{
			image.fillAmount = fCurrentValue;
		}
	}
}

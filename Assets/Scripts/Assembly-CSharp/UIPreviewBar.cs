using UnityEngine;
using UnityEngine.UI;

public class UIPreviewBar : MonoBehaviour
{
	[SerializeField]
	private Color negativePreviewColor;

	[SerializeField]
	private Image imgTarget;

	private Color defaultColor;

	private UiBuildingButtonManager UiBBM;

	private LocalGameManager LGM;

	private void Start()
	{
		LGM = LocalGameManager.singleton;
		UiBBM = UiBuildingButtonManager.singleton;
		defaultColor = imgTarget.color;
	}

	private void Update()
	{
		float num = 0f;
		bool flag = true;
		if (UiBBM.GoBuildingPreview != null)
		{
			if (UiBBM.IScorePreview >= 0)
			{
				imgTarget.color = defaultColor;
				num = Mathf.InverseLerp(LGM.IRequiredScoreForLastPack, LGM.IRequiredScoreForNextPack, LGM.IScore + UiBBM.IScorePreview);
			}
			else
			{
				imgTarget.color = negativePreviewColor;
				num = Mathf.InverseLerp(LGM.IRequiredScoreForLastPack, LGM.IRequiredScoreForNextPack, LGM.IScore);
				flag = false;
			}
		}
		float fillAmount = ((!flag) ? num : Mathf.MoveTowards(imgTarget.fillAmount, num, 3.5f * Time.deltaTime));
		imgTarget.fillAmount = fillAmount;
	}
}

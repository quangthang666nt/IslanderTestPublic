using UnityEngine;
using UnityEngine.UI;

public class UiScoreChangePreview : MonoBehaviour
{
	private UiBuildingButtonManager uiBuildingButtonManager;

	private Text text;

	[SerializeField]
	private Color colPositive;

	[SerializeField]
	private Color colNegative;

	private void Start()
	{
		uiBuildingButtonManager = UiBuildingButtonManager.singleton;
		text = GetComponent<Text>();
	}

	private void Update()
	{
		if ((bool)uiBuildingButtonManager.GoBuildingPreview)
		{
			if (uiBuildingButtonManager.GoBuildingPreview.activeSelf)
			{
				int iScorePreview = uiBuildingButtonManager.IScorePreview;
				text.text = iScorePreview.ToString();
				if (iScorePreview > 0)
				{
					text.text = "+" + text.text;
				}
				text.color = ((iScorePreview >= 0) ? colPositive : colNegative);
			}
			else
			{
				text.text = "";
			}
		}
		else
		{
			text.text = "";
		}
	}
}

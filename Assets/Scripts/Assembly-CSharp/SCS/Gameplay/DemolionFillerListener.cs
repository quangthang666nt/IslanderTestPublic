using UnityEngine;
using UnityEngine.UI;

namespace SCS.Gameplay
{
	public class DemolionFillerListener : MonoBehaviour
	{
		[SerializeField]
		private FloatEventHandler onFillerEvent;

		[Header("UI")]
		[SerializeField]
		private Image background;

		[SerializeField]
		private Image fillmid;

		[SerializeField]
		private Image fillbackground;

		private void OnEnable()
		{
			onFillerEvent?.RegisterEvent(OnFillCallback);
			background.gameObject.SetActive(value: false);
		}

		private void OnDisable()
		{
			onFillerEvent?.UnRegisterEvent(OnFillCallback);
		}

		private void OnFillCallback(float delta)
		{
			background.gameObject.SetActive(delta > DemolitionController.START_FILL_PERCENT);
			if (delta <= DemolitionController.START_FILL_PERCENT)
			{
				Image image = fillbackground;
				float fillAmount = (fillmid.fillAmount = 0f);
				image.fillAmount = fillAmount;
			}
			else if (delta >= DemolitionController.REDUCE_SPEED_LIMIT)
			{
				fillmid.fillAmount = delta;
				fillbackground.fillAmount = DemolitionController.REDUCE_SPEED_LIMIT;
			}
			else
			{
				fillbackground.fillAmount = delta;
				fillmid.fillAmount = 0f;
			}
		}
	}
}

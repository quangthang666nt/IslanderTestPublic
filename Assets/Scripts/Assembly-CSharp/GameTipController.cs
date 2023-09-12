using I2.Loc;
using TMPro;
using UnityEngine;

public class GameTipController : MonoBehaviour
{
	private const string LOADING_TIP_LABEL = "Loading Screen Tips (Enhanced)/LoadingTips/";

	[SerializeField]
	private TextMeshProUGUI textDisplayer;

	[SerializeField]
	private int total = 11;

	[SerializeField]
	private float displayTime = 2f;

	private int tipID = 1;

	private float timer;

	private void OnEnable()
	{
		timer = 0f;
		textDisplayer.text = GetTip(tipID);
	}

	private void OnDisable()
	{
		if (timer > displayTime / 2f)
		{
			tipID = AddOneClamped();
			textDisplayer.text = GetTip(tipID);
		}
	}

	private string GetTip(int id)
	{
		return new LocalizedString("Loading Screen Tips (Enhanced)/LoadingTips/" + id);
	}

	private void Update()
	{
		timer += Time.deltaTime;
		if (timer >= displayTime)
		{
			tipID = AddOneClamped();
			textDisplayer.text = GetTip(tipID);
			timer = 0f;
		}
	}

	private int AddOneClamped()
	{
		return Mathf.Clamp(++tipID % (total + 1), 1, total);
	}
}

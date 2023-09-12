using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UiFloatToTextUpdater : MonoBehaviour
{
	[SerializeField]
	protected Color colTextValueRising;

	[SerializeField]
	protected Color colTextValueSinking;

	[SerializeField]
	protected float fSizeRising = 1.2f;

	[SerializeField]
	protected float fSizeSinking = 0.8f;

	[SerializeField]
	protected float fEffectBlendRange = 5f;

	[SerializeField]
	protected float fChangeSpeed = 50f;

	protected Text text;

	private Color colTextNormal;

	protected float fTargetValue;

	protected float fCurrentValue;

	protected void Start()
	{
		text = GetComponent<Text>();
		fTargetValue = float.Parse(text.text);
		fCurrentValue = fTargetValue;
		colTextNormal = text.color;
	}

	protected void UpdateText()
	{
		Color color;
		if (fCurrentValue > fTargetValue)
		{
			float value = Mathf.InverseLerp(fTargetValue, fTargetValue + fEffectBlendRange, fCurrentValue);
			value = Mathf.Clamp(value, 0f, 1f);
			color = Color.Lerp(colTextNormal, colTextValueSinking, value);
			text.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(fSizeSinking, fSizeSinking, 1f), value);
		}
		else
		{
			float value2 = Mathf.InverseLerp(fTargetValue, fTargetValue - fEffectBlendRange, fCurrentValue);
			value2 = Mathf.Clamp(value2, 0f, 1f);
			color = Color.Lerp(colTextNormal, colTextValueRising, value2);
			text.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(fSizeRising, fSizeRising, 1f), value2);
		}
		text.color = color;
		float num = Mathf.Clamp(Mathf.Sign(fTargetValue - fCurrentValue) * Time.deltaTime * fChangeSpeed, 0f - Mathf.Abs(fTargetValue - fCurrentValue), Mathf.Abs(fTargetValue - fCurrentValue));
		fCurrentValue += num;
		text.text = Mathf.Round(fCurrentValue).ToString();
	}
}

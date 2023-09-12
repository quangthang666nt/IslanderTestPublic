using UnityEngine;
using UnityEngine.UI;

public class UIContributorScorePreview : MonoBehaviour
{
	[SerializeField]
	private Text scoreText;

	[SerializeField]
	private Outline outline;

	public string Text
	{
		get
		{
			return scoreText.text;
		}
		set
		{
			scoreText.text = value;
		}
	}

	public Color Color
	{
		get
		{
			return scoreText.color;
		}
		set
		{
			scoreText.color = value;
		}
	}
	public Color OutlineColor
	{
		get
		{
			return outline.effectColor;
		}
		set
		{
			SetOutlineWidth();
			outline.effectColor = value;
		}
	}

	private void SetOutlineWidth()
	{
		outline.effectDistance = new Vector2(FeedbackManager.Singleton.fScorePreviewOutlineWidth, FeedbackManager.Singleton.fScorePreviewOutlineWidth);
	}
}

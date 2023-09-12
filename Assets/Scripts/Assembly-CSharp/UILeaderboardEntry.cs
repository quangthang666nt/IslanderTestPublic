using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILeaderboardEntry : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI rankUI;

	[SerializeField]
	private TextMeshProUGUI scoreUI;

	[SerializeField]
	private Text usernameUI;

	[SerializeField]
	private Image background;

	[SerializeField]
	private Color bgNormal;

	[SerializeField]
	private Color bgHighlighted;

	public void UpdateDisplay(string rank, string username, string score, bool isHighlighted = false)
	{
		rankUI.text = "#" + rank;
		usernameUI.text = username;
		scoreUI.text = score;
		if (isHighlighted)
		{
			background.color = bgHighlighted;
		}
		else
		{
			background.color = bgNormal;
		}
	}
}

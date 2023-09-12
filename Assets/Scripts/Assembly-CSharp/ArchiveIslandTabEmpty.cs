using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ArchiveIslandTabEmpty : MonoBehaviour
{
	[SerializeField]
	private TMP_Text label;

	private ArchiveLoadPrompt loadPrompt;

	private bool bReady;

	private Image image;

	private BasicNavigationItem item;

	private void Start()
	{
		item = GetComponent<BasicNavigationItem>();
	}

	public void SetPrompt(ArchiveLoadPrompt loadPrompt)
	{
		this.loadPrompt = loadPrompt;
		bReady = true;
	}

	public void InteractFromClick()
	{
		if (!item)
		{
			Debug.LogError("BasicNavigationItem reference is null");
		}
		else
		{
			loadPrompt.SelectItem(item);
		}
	}

	public void Interact()
	{
		if (bReady)
		{
			loadPrompt.SelectEmpty();
		}
	}

	public void SetSelectedColor()
	{
		if (!image)
		{
			image = GetComponent<Image>();
		}
		image.color = loadPrompt.NavSelectedColor;
		label.color = loadPrompt.NavUnselectedColor;
	}

	public void SetUnselectedColor()
	{
		if (!image)
		{
			image = GetComponent<Image>();
		}
		image.color = loadPrompt.NavUnselectedColor;
		label.color = loadPrompt.NavSelectedColor;
	}
}

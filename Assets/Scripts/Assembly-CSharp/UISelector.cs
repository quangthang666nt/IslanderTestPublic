using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UISelector : MonoBehaviour
{
	public List<string> options;

	public UnityEvent eventOnSelectionChange = new UnityEvent();

	private TextMeshProUGUI display;

	private int index;

	public int Index => index;

	private void Start()
	{
		display = GetComponent<TextMeshProUGUI>();
		DisplayIndex();
	}

	private void DisplayIndex()
	{
		if (!(display == null))
		{
			if (index >= options.Count)
			{
				index = 0;
			}
			else if (index < 0)
			{
				index = options.Count - 1;
			}
			display.text = options[index];
		}
	}

	public void SetIndex(int i)
	{
		index = i;
		DisplayIndex();
	}

	public void IndexUp()
	{
		index++;
		DisplayIndex();
		eventOnSelectionChange.Invoke();
	}

	public void IndexDown()
	{
		index--;
		DisplayIndex();
		eventOnSelectionChange.Invoke();
	}
}

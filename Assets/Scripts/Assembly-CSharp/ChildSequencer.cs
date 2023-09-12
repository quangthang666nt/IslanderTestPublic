using TMPro;
using UnityEngine;

public class ChildSequencer : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private EventObject stringActionObject;

	[SerializeField]
	private EventObject gameObjectActionObject;

	[SerializeField]
	private bool allowNoneSelected = true;

	private int childIndex;

	public int ChildIndex
	{
		get
		{
			if (childIndex == 0 && !allowNoneSelected)
			{
				return 1;
			}
			return childIndex;
		}
	}

	private void Start()
	{
		Clear();
	}

	public void Next()
	{
		childIndex = (childIndex + 1) % (base.transform.childCount + 1);
		if (!allowNoneSelected && childIndex == 0)
		{
			childIndex = 1;
		}
		SelectCurrentChild();
	}

	public void Previous()
	{
		childIndex--;
		if (childIndex < 0)
		{
			childIndex = base.transform.childCount;
		}
		if (!allowNoneSelected && childIndex == 0)
		{
			childIndex = base.transform.childCount;
		}
		SelectCurrentChild();
	}

	public void Clear()
	{
		if (!allowNoneSelected)
		{
			childIndex = 1;
		}
		else
		{
			childIndex = 0;
		}
		SelectCurrentChild();
	}

	public bool SetChildIndex(int index)
	{
		if (index < 0 || index >= base.transform.childCount)
		{
			return false;
		}
		childIndex = index;
		if (childIndex == 0 && !allowNoneSelected)
		{
			childIndex = 1;
		}
		SelectCurrentChild();
		return true;
	}

	private void SelectCurrentChild()
	{
		if (childIndex != 0)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				base.transform.GetChild(i).gameObject.SetActive(childIndex - 1 == i);
			}
			if ((bool)text)
			{
				text.text = base.transform.GetChild(childIndex - 1).name;
			}
			if (stringActionObject != null)
			{
				stringActionObject.objectEvent?.Invoke(base.transform.GetChild(childIndex - 1).name);
			}
			if (gameObjectActionObject != null)
			{
				gameObjectActionObject.objectEvent?.Invoke(base.transform.GetChild(childIndex - 1).gameObject);
			}
		}
		else
		{
			for (int j = 0; j < base.transform.childCount; j++)
			{
				base.transform.GetChild(j).gameObject.SetActive(value: false);
			}
			if ((bool)text)
			{
				text.text = "None";
			}
			if (stringActionObject != null)
			{
				stringActionObject.objectEvent?.Invoke("None");
			}
			if (gameObjectActionObject != null)
			{
				gameObjectActionObject.objectEvent?.Invoke(null);
			}
		}
	}
}

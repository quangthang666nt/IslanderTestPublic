using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContentSizeFitterHelper : UIBehaviour
{
	[SerializeField]
	private bool forcesRebuildParentLayout = true;

	[SerializeField]
	private RectTransform parentLayout;

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		if (forcesRebuildParentLayout && parentLayout != null)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(parentLayout);
		}
	}
}

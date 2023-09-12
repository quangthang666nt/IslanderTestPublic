using System.Collections.Generic;
using UnityEngine;

public class StructureOutline : MonoBehaviour
{
	[SerializeField]
	private Material positiveMaterial;

	[SerializeField]
	private Material negativeMaterial;

	[SerializeField]
	private Material maskMaterial;

	[SerializeField]
	private bool findOutlinesAtStart = true;

	[SerializeField]
	private List<MeshRenderer> outlinePositiveRenderers = new List<MeshRenderer>();

	[SerializeField]
	private List<MeshRenderer> outlineNegativeRenderers = new List<MeshRenderer>();

	private Material runtimePositiveMaterial;

	private Material runtimeNegativeMaterial;

	private Material runtimeMaskMaterial;

	private Material[] positiveMaterials;

	private Material[] negativeMaterials;

	public static bool activeOutline = true;

	private void Start()
	{
		runtimePositiveMaterial = new Material(positiveMaterial);
		runtimeNegativeMaterial = new Material(negativeMaterial);
		runtimeMaskMaterial = new Material(maskMaterial);
		positiveMaterials = new Material[2];
		positiveMaterials[0] = runtimeMaskMaterial;
		positiveMaterials[1] = runtimePositiveMaterial;
		negativeMaterials = new Material[2];
		negativeMaterials[0] = runtimeMaskMaterial;
		negativeMaterials[1] = runtimeNegativeMaterial;
		if (findOutlinesAtStart)
		{
			FindOutlines();
		}
		HideOutline();
	}

	public void FindOutlines()
	{
		List<Transform> childs = new List<Transform>();
		FindChildRecursive(base.transform, "Outline", ref childs);
		foreach (Transform item in childs)
		{
			for (int i = 0; i < item.childCount; i++)
			{
				Transform child = item.GetChild(i);
				MeshRenderer component = child.GetComponent<MeshRenderer>();
				if (component != null && !outlinePositiveRenderers.Contains(component))
				{
					outlinePositiveRenderers.Add(component);
					GameObject obj = Object.Instantiate(child.gameObject, child);
					obj.name = child.gameObject.name + "_NegativeOutline";
					obj.transform.rotation = child.transform.rotation;
					obj.transform.position = child.transform.position;
					MeshRenderer component2 = obj.GetComponent<MeshRenderer>();
					outlineNegativeRenderers.Add(component2);
					component.materials = positiveMaterials;
					component2.materials = negativeMaterials;
				}
			}
		}
	}

	private void FindChildRecursive(Transform parent, string name, ref List<Transform> childs)
	{
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			if (name.Equals(child.gameObject.name))
			{
				childs.Add(child);
			}
			else
			{
				FindChildRecursive(child, name, ref childs);
			}
		}
	}

	public void ShowPositiveOutline()
	{
		if (activeOutline)
		{
			for (int i = 0; i < outlinePositiveRenderers.Count; i++)
			{
				outlinePositiveRenderers[i].enabled = true;
				outlineNegativeRenderers[i].enabled = false;
			}
		}
	}

	public void ShowNegativeOutline()
	{
		if (activeOutline)
		{
			for (int i = 0; i < outlinePositiveRenderers.Count; i++)
			{
				outlinePositiveRenderers[i].enabled = false;
				outlineNegativeRenderers[i].enabled = true;
			}
		}
	}

	public void HideOutline()
	{
		for (int i = 0; i < outlinePositiveRenderers.Count; i++)
		{
			outlinePositiveRenderers[i].enabled = false;
			outlineNegativeRenderers[i].enabled = false;
		}
	}

	private void OnDestroy()
	{
		if (runtimeMaskMaterial != null)
		{
			Object.DestroyImmediate(runtimeMaskMaterial);
		}
		if (runtimePositiveMaterial != null)
		{
			Object.DestroyImmediate(runtimePositiveMaterial);
		}
		if (runtimeNegativeMaterial != null)
		{
			Object.DestroyImmediate(runtimeNegativeMaterial);
		}
	}
}

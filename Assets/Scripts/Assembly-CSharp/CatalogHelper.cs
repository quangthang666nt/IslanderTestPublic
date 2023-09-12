using UnityEngine;

public class CatalogHelper : MonoBehaviour
{
	private static string LABEL_TABLE = "labelTable";

	private static string LABEL_NAMES = "labelNames";

	public static string PACKAGE_SEPARATOR = "_";

	public static string TIMESTAMP_SEPARATOR = "#";

	private void Start()
	{
	}

	private void Update()
	{
	}

	public static string GetElementId(string package, string elementId)
	{
		return package + PACKAGE_SEPARATOR + elementId;
	}
}

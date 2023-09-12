using System.Collections.Generic;
using UnityEngine;

public class TO_HandlerIdentifier : MonoBehaviour
{
	[SerializeField]
	private string identifier;

	private int hashCode;

	private static Dictionary<int, List<TO_HandlerIdentifier>> internalIdentifierDic = new Dictionary<int, List<TO_HandlerIdentifier>>();

	public int ID => hashCode;

	public void SetIdentifier(string identifier)
	{
		this.identifier = identifier;
		hashCode = identifier.GetHashCode();
	}

	public static void ClearHandlersCache()
	{
		internalIdentifierDic.Clear();
	}

	public static Dictionary<int, List<TO_HandlerIdentifier>> GetAllHandlerIdentifiers()
	{
		if (internalIdentifierDic.Count < 0)
		{
			return internalIdentifierDic;
		}
		TO_HandlerIdentifier[] array = Object.FindObjectsOfType<TO_HandlerIdentifier>();
		foreach (TO_HandlerIdentifier tO_HandlerIdentifier in array)
		{
			if (internalIdentifierDic.ContainsKey(tO_HandlerIdentifier.ID))
			{
				internalIdentifierDic[tO_HandlerIdentifier.ID].Add(tO_HandlerIdentifier);
				continue;
			}
			List<TO_HandlerIdentifier> list = new List<TO_HandlerIdentifier>();
			list.Add(tO_HandlerIdentifier);
			internalIdentifierDic.Add(tO_HandlerIdentifier.ID, list);
		}
		return internalIdentifierDic;
	}
}

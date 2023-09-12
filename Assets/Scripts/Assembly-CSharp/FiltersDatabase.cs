using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(fileName = "Filters Database", menuName = "SCS/Filters Database")]
public class FiltersDatabase : ScriptableObject
{
	[Serializable]
	public struct Filter
	{
		public string filterName;

		public PostProcessProfile profile;
	}

	public List<Filter> filters;
}

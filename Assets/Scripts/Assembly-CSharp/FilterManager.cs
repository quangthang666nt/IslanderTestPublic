using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class FilterManager : MonoBehaviour
{
	[SerializeField]
	private PostProcessVolume volume;

	[SerializeField]
	private FiltersDatabase filtersDatabase;

	[SerializeField]
	private PostProcessProfile defaultProfile;

	[SerializeField]
	private EventObject filterNameActionObject;

	private int icurrentProfile;

	private void Awake()
	{
		if (volume == null)
		{
			volume = Object.FindObjectOfType<PostProcessVolume>();
		}
	}

	public void NextFilter()
	{
		icurrentProfile++;
		if (icurrentProfile - 1 < filtersDatabase.filters.Count)
		{
			volume.profile = filtersDatabase.filters[icurrentProfile - 1].profile;
			filterNameActionObject.objectEvent(filtersDatabase.filters[icurrentProfile - 1].filterName);
		}
		else
		{
			icurrentProfile = 0;
			ReturnToNormal();
		}
	}

	public void PrevFilter()
	{
		icurrentProfile--;
		if (icurrentProfile == 0)
		{
			ReturnToNormal();
			return;
		}
		if (icurrentProfile - 1 < 0)
		{
			icurrentProfile = filtersDatabase.filters.Count;
		}
		volume.profile = filtersDatabase.filters[icurrentProfile - 1].profile;
		filterNameActionObject.objectEvent(filtersDatabase.filters[icurrentProfile - 1].filterName);
	}

	private void ReturnToNormal()
	{
		volume.profile = defaultProfile;
		filterNameActionObject.objectEvent("None");
	}

	public void ResetFilters()
	{
		ReturnToNormal();
		icurrentProfile = 0;
	}
}

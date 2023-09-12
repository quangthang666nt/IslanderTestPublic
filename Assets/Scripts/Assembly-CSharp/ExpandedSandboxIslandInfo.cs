using UnityEngine;

public class ExpandedSandboxIslandInfo : MonoBehaviour
{
	[Tooltip("Max numbers of tree groups for this island")]
	public int maxTreesGroupsValue = 20;

	[Tooltip("Max numbers of flowers groups for this island")]
	public int maxFlowersGroupsValue = 20;

	[Header("Internal")]
	[Tooltip("Reference to modify resources in expanded sandbox mode")]
	public GameObject resourcesChildRef;

	[Tooltip("Reference to modify ice floes in expanded sandbox mode")]
	public GameObject iceFloesRef;
}

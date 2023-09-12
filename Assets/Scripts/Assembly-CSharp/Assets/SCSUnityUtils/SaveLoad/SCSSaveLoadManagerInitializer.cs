using SCS.SaveLoad;
using UnityEngine;

namespace Assets.SCSUnityUtils.SaveLoad
{
	public class SCSSaveLoadManagerInitializer : MonoBehaviour
	{
		[Tooltip("Also works as Container name in XBoxOne.")]
		public string basePath;

		[Header("PSVita setup")]
		[HideInInspector]
		[Tooltip("Set the slots used for saving data on PS Vita.")]
		public int slotCount = 1;

		[HideInInspector]
		[Tooltip("Determine if the system shows a dialog warning when the game is run out space to save games.")]
		public bool enableNotSpaceDialogWarning;

		private void Awake()
		{
			SCSSaveLoadManager sCSSaveLoadManager = base.gameObject.AddComponent<SCSSaveLoadManager>();
			sCSSaveLoadManager.basePath = basePath;
			sCSSaveLoadManager.slotCount = slotCount;
			sCSSaveLoadManager.enableNotSpaceDialogWarning = enableNotSpaceDialogWarning;
			Object.Destroy(this);
		}
	}
}

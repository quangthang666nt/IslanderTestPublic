using UnityEngine;

[ExecuteInEditMode]
public class StructureID : MonoBehaviour
{
	public int iID = -1;

	private void OnEnable()
	{
		SaveLoadManager.liStructIDRegister.Add(this);
		SaveLoadManager.liGoStructIDRegister.Add(base.gameObject);
	}
	private void OnDisable()
	{
		SaveLoadManager.liStructIDRegister.Remove(this);
		SaveLoadManager.liGoStructIDRegister.Remove(base.gameObject);
	}
}

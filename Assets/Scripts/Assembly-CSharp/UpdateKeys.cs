using I2.Loc;
using Rewired;
using UnityEngine;

public class UpdateKeys : MonoBehaviour
{
	public Localize[] refresh;

	private void OnEnable()
	{
		try
		{
			ActionElementMap keyboardAction = KeyMappingTool.GetKeyboardAction("Camera Pan X+");
			ActionElementMap keyboardAction2 = KeyMappingTool.GetKeyboardAction("Camera Pan X-");
			ActionElementMap keyboardAction3 = KeyMappingTool.GetKeyboardAction("Camera Pan Z+");
			ActionElementMap keyboardAction4 = KeyMappingTool.GetKeyboardAction("Camera Pan Z-");
			if (keyboardAction != null && keyboardAction2 != null && keyboardAction3 != null && keyboardAction4 != null)
			{
				I2Manager.Instance.localizationParamsManager.SetParameterValue("MOVE_KEYS", "<style=POS>" + keyboardAction3.elementIdentifierName + ", " + keyboardAction4.elementIdentifierName + ", " + keyboardAction2.elementIdentifierName + ", " + keyboardAction.elementIdentifierName + "</style>");
			}
			ActionElementMap keyboardAction5 = KeyMappingTool.GetKeyboardAction("Camera Rotate+");
			ActionElementMap keyboardAction6 = KeyMappingTool.GetKeyboardAction("Camera Rotate-");
			if (keyboardAction5 != null && keyboardAction6 != null)
			{
				I2Manager.Instance.localizationParamsManager.SetParameterValue("ROTATE_KEYS", "<style=POS>" + keyboardAction5.elementIdentifierName + ", " + keyboardAction6.elementIdentifierName + "</style>");
			}
			Localize[] array = refresh;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnLocalize(Force: true);
			}
		}
		catch
		{
		}
	}
}

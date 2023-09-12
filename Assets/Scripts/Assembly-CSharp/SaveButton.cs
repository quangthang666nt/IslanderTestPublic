using UnityEngine;

public class SaveButton : BasicNavigationItem
{
	public GameObject m_SavePossibleVisual;

	public GameObject m_SavingInProgressVisual;

	public GameObject m_SaveImpossibleVisual;

	public GenericMenuList m_Menu;

	public override void OnSubmit()
	{
		base.OnSubmit();
	}

	private void Start()
	{
		SaveLoadManager.OnLastAutoSaveDeniedChange += OnLastAutoSaveDeniedChange;
		OnLastAutoSaveDeniedChange(SaveLoadManager.LastAutoSaveDenied, SaveLoadManager.singleton.m_IsSaving);
	}

	private void OnDestroy()
	{
		SaveLoadManager.OnLastAutoSaveDeniedChange -= OnLastAutoSaveDeniedChange;
	}

	private void OnLastAutoSaveDeniedChange(bool denied, bool isSaving)
	{
		if (isSaving)
		{
			base.enabled = false;
			m_SavePossibleVisual.gameObject.SetActive(value: false);
			m_SaveImpossibleVisual.gameObject.SetActive(value: false);
			m_SavingInProgressVisual.gameObject.SetActive(value: true);
			m_Menu.UnlockAfterDelay(0.5f);
			m_Menu.SelectFirstAvailable();
		}
		else if (denied)
		{
			base.enabled = true;
			m_SavePossibleVisual.gameObject.SetActive(value: true);
			m_SaveImpossibleVisual.gameObject.SetActive(value: false);
			m_SavingInProgressVisual.gameObject.SetActive(value: false);
			m_Menu.UnlockAfterDelay(0.5f);
		}
		else
		{
			base.enabled = false;
			m_SavePossibleVisual.gameObject.SetActive(value: false);
			m_SaveImpossibleVisual.gameObject.SetActive(value: true);
			m_SavingInProgressVisual.gameObject.SetActive(value: false);
			m_Menu.UnlockAfterDelay(0.5f);
			m_Menu.SelectFirstAvailable();
		}
	}
}

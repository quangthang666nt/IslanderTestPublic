using UnityEngine;

public struct ScoreContributorData
{
	public Transform trans;

	public int iScoreContribution;

	public ScoreContributorData(Transform _trans, int _iContribution)
	{
		trans = _trans;
		iScoreContribution = _iContribution;
	}
}

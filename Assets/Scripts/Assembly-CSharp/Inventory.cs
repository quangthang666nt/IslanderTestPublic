using System;
using System.Collections.Generic;

[Serializable]
public class Inventory<T>
{
	public List<T> liT = new List<T>();

	public List<int> liIAmount = new List<int>();

	public void Add(T _t, int _iAmount = 1)
	{
		if (liT.Contains(_t))
		{
			int index = IListFindIndex(liT, _t);
			liIAmount[index] += _iAmount;
		}
		else
		{
			liT.Add(_t);
			liIAmount.Add(_iAmount);
		}
	}

	public int IGet(T _t)
	{
		if (liT.Contains(_t))
		{
			int index = IListFindIndex(liT, _t);
			return liIAmount[index];
		}
		return 0;
	}

	public void Set(T _t, int _iAmount)
	{
		if (liT.Contains(_t))
		{
			int index = IListFindIndex(liT, _t);
			liIAmount[index] = _iAmount;
		}
		else
		{
			liT.Add(_t);
			liIAmount.Add(_iAmount);
		}
	}

	public void Clear()
	{
		liT.Clear();
		liIAmount.Clear();
	}

	private int IListFindIndex<Ts>(List<Ts> lits, Ts ts)
	{
		for (int i = 0; i < lits.Count; i++)
		{
			if (lits[i].Equals(ts))
			{
				return i;
			}
		}
		return -1;
	}
}

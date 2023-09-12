using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PositionHeatmap
{
	public class PixelData
	{
		public int iXPos;

		public int iYPos;

		public float fValue;

		public PixelData(int _xPos, int _yPos, float _fValue)
		{
			iXPos = _xPos;
			iYPos = _yPos;
			fValue = _fValue;
		}
	}

	[SerializeField]
	private Texture2D t2DHeatmap;

	private List<PixelData> liPixelData = new List<PixelData>();

	private List<int> liWeightedPixelIndices = new List<int>();

	public void UpdatePixelData()
	{
		Color[] pixels = t2DHeatmap.GetPixels();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		liPixelData.Clear();
		liWeightedPixelIndices.Clear();
		Color[] array = pixels;
		for (int i = 0; i < array.Length; i++)
		{
			Color color = array[i];
			float num4 = 1f - color.r;
			liPixelData.Add(new PixelData(num, num2, num4));
			int num5 = Mathf.RoundToInt(10f * num4);
			for (int j = 0; j < num5; j++)
			{
				liWeightedPixelIndices.Add(num3);
			}
			num3++;
			num++;
			if (num >= t2DHeatmap.width)
			{
				num = 0;
				num2++;
			}
		}
	}

	public Vector2 GetRandomPosition()
	{
		PixelData pixelData = new PixelData(0, 0, 0f);
		if (liWeightedPixelIndices.Count > 0)
		{
			int index = liWeightedPixelIndices[UnityEngine.Random.Range(0, liWeightedPixelIndices.Count)];
			pixelData = liPixelData[index];
		}
		float num = 2f * (Mathf.InverseLerp(0f, t2DHeatmap.width, pixelData.iXPos) - 0.5f);
		float num2 = 2f * (Mathf.InverseLerp(0f, t2DHeatmap.height, pixelData.iYPos) - 0.5f);
		float x = num + 2f / (float)t2DHeatmap.width * (UnityEngine.Random.value - 0.5f);
		num2 += 2f / (float)t2DHeatmap.width * (UnityEngine.Random.value - 0.5f);
		return new Vector2(x, num2);
	}

	public Vector2 v2GetRandomPositionInRange()
	{
		Vector2 result = Vector2.one;
		while (result.magnitude > 1f)
		{
			result = GetRandomPosition();
		}
		return result;
	}
}

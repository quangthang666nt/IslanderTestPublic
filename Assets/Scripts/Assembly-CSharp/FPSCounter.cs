using UnityEngine;

public class FPSCounter : MonoBehaviour
{
	public int frameRange = 60;

	private int[] fpsBuffer;

	private int fpsBufferIndex;

	public int AverageFPS { get; private set; }

	public int HighestFPS { get; private set; }

	public int LowestFPS { get; private set; }

	private void Update()
	{
		if (fpsBuffer == null || fpsBuffer.Length != frameRange)
		{
			InitializeBuffer();
		}
		UpdateBuffer();
		CalculateFPS();
	}

	private void InitializeBuffer()
	{
		if (frameRange <= 0)
		{
			frameRange = 1;
		}
		fpsBuffer = new int[frameRange];
		fpsBufferIndex = 0;
	}

	private void UpdateBuffer()
	{
		fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
		if (fpsBufferIndex >= frameRange)
		{
			fpsBufferIndex = 0;
		}
	}

	private void CalculateFPS()
	{
		int num = 0;
		int num2 = 0;
		int num3 = int.MaxValue;
		for (int i = 0; i < frameRange; i++)
		{
			int num4 = fpsBuffer[i];
			num += num4;
			if (num4 > num2)
			{
				num2 = num4;
			}
			if (num4 < num3)
			{
				num3 = num4;
			}
		}
		AverageFPS = (int)((float)num / (float)frameRange);
		HighestFPS = num2;
		LowestFPS = num3;
	}
}

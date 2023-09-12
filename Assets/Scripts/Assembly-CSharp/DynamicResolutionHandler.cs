using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicResolutionHandler : MonoBehaviour
{
	public static List<DynamicResolutionHandler> DynamicResolutionHandlers = new List<DynamicResolutionHandler>();

	public Text m_DebugText;

	public bool m_UseDynamicResolution = true;

	public bool m_UseFixedScale;

	public float m_FixedScale = 1f;

	public int m_SampleCount = 1;

	private float m_ScaleFactor = 1f;

	private static float s_DebugScaleFactor = -1f;

	private float m_TargetScale = 1f;

	private const float c_MinimumScaleFactor = 1f / 3f;

	private const float c_UpperFrameRateThresholdAdaptation = -0.6f;

	private const float c_LowerFrameRateThresholdAdaption = -1.6f;

	private const float c_MaxScaleChangePerFrame = 0.005f;

	private FrameTiming[] m_FrameTimings;

	private double m_AverageGPUTime;

	private int m_FramesSinceLastAdapt;

	private void Start()
	{
		m_FrameTimings = new FrameTiming[m_SampleCount];
		PlatformPlayerManagerSystem.Instance.PlatformPlayerManager.OnSystemOutOfFocus += OnOutOfFocus;
		PlatformPlayerManagerSystem.Instance.PlatformPlayerManager.OnSystemFocus += OnFocus;
	}

	private void OnEnable()
	{
		DynamicResolutionHandlers.Add(this);
		if (PlatformPlayerManagerSystem.Instance != null)
		{
			PlatformPlayerManagerSystem.Instance.PlatformPlayerManager.OnSystemOutOfFocus += OnOutOfFocus;
			PlatformPlayerManagerSystem.Instance.PlatformPlayerManager.OnSystemFocus += OnFocus;
		}
	}

	private void OnFocus()
	{
		m_UseDynamicResolution = true;
		ScalableBufferManager.ResizeBuffers(1f, 1f);
		m_ScaleFactor = 1f;
	}

	private void OnOutOfFocus()
	{
		m_UseDynamicResolution = false;
		ScalableBufferManager.ResizeBuffers(1f, 1f);
		m_ScaleFactor = 1f;
	}

	private void OnDisable()
	{
		DynamicResolutionHandlers.Remove(this);
		if (PlatformPlayerManagerSystem.IsReady)
		{
			PlatformPlayerManagerSystem.Instance.PlatformPlayerManager.OnSystemOutOfFocus -= OnOutOfFocus;
			PlatformPlayerManagerSystem.Instance.PlatformPlayerManager.OnSystemFocus -= OnFocus;
		}
	}

	private void Update()
	{
		if (m_UseDynamicResolution)
		{
			AdaptResolution();
		}
	}

	private void AdaptResolution()
	{
		if (m_UseFixedScale)
		{
			m_ScaleFactor = m_FixedScale;
			ScalableBufferManager.ResizeBuffers(m_ScaleFactor, m_ScaleFactor);
		}
		else
		{
			if (m_FramesSinceLastAdapt >= m_SampleCount)
			{
				m_FramesSinceLastAdapt = 0;
				FrameTimingManager.CaptureFrameTimings();
				FrameTimingManager.GetLatestTimings((uint)m_SampleCount, m_FrameTimings);
				m_AverageGPUTime = 0.0;
				int num = 0;
				for (int i = 0; i < m_FrameTimings.Length; i++)
				{
					if (m_FrameTimings[i].gpuFrameTime < 100.0)
					{
						m_AverageGPUTime += m_FrameTimings[i].gpuFrameTime;
						num++;
					}
				}
				m_AverageGPUTime /= num;
			}
			else
			{
				m_FramesSinceLastAdapt++;
			}
			if (m_AverageGPUTime > 0.0 && PlatformPlayerManagerSystem.Instance != null)
			{
				float targetFrameTime = PlatformPlayerManagerSystem.Instance.GetTargetFrameTime();
				double num2 = targetFrameTime + -0.6f;
				double num3 = targetFrameTime + -1.6f;
				if (m_AverageGPUTime > num2)
				{
					float num4 = (float)m_AverageGPUTime / (float)num2;
					m_TargetScale -= 0.005f * (num4 * num4 * num4);
				}
				else if (m_AverageGPUTime < num3 && m_ScaleFactor < 1f)
				{
					m_TargetScale += 0.005f;
				}
			}
			m_ScaleFactor = Mathf.Clamp(m_TargetScale, 1f / 3f, 1f);
			if (s_DebugScaleFactor > 0f)
			{
				m_ScaleFactor = s_DebugScaleFactor;
			}
			ScalableBufferManager.ResizeBuffers(m_ScaleFactor, m_ScaleFactor);
		}
		if (m_DebugText != null)
		{
			m_DebugText.text = "Resolution Scale: " + m_ScaleFactor + "\nAverage GPU Time: " + m_AverageGPUTime;
		}
	}

	[CommandLine("dyn_res_debug_scale", "Set debug scale factor to test dynamic resolution", null, true)]
	public static void DebugSetScale(float scale = -1f)
	{
		s_DebugScaleFactor = scale;
	}
}

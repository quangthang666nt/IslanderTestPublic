using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttractableCursor : MonoBehaviour
{
	private struct DynamicTarget
	{
		public RectTransform Target;

		public bool FollowHorizontal;

		public bool FollowVertical;

		public void Reset()
		{
			Target = null;
			FollowHorizontal = false;
			FollowVertical = false;
		}

		public bool IsValid()
		{
			return Target != null;
		}
	}

	public float m_TargetPositionTrackingRate = 1f;

	public float m_TargetWidthTrackingRate = 1f;

	public bool m_UseWorldSpacePosition;

	public bool m_NextMoveIsInstant;

	public bool m_NextResizeIsInstant;

	public RectTransform m_RectTransform;

	public Image m_CursorImage;

	public TextMeshProUGUI m_SelectionDescription;

	public Animator m_Animator;

	public string m_OnSelectAnimation = "Select";

	public float m_CursorMinimumWidth = 600f;

	public float m_CursorBufferAdaptativeWidth = 30f;

	public bool m_EmitSoundsOnSelect;

	public float m_CloseEnoughDistance = 0.1f;

	private SmoothPosition m_TargetPosition = new SmoothPosition(1f);

	private SmoothFloat m_TargetWidth = new SmoothFloat(1f);

	private TextMeshProUGUI m_CurrentTextResizeTarget;

	private Action m_CallbackWhenDone;

	private DynamicTarget m_CurrentTarget;

	private bool m_JustMoved;

	public bool m_SkipNextSelectAnimation;

	private void Awake()
	{
		if (m_RectTransform == null)
		{
			m_RectTransform = GetComponent<RectTransform>();
		}
		m_TargetPosition.SetNow(m_RectTransform.anchoredPosition);
	}

	public void UpdateCursorSize(TextMeshProUGUI contentText)
	{
		contentText.ForceMeshUpdate();
		float a = contentText.preferredWidth + m_CursorBufferAdaptativeWidth;
		if (m_SelectionDescription != null)
		{
			m_SelectionDescription.ForceMeshUpdate();
			a = Mathf.Max(a, m_SelectionDescription.preferredWidth + m_CursorBufferAdaptativeWidth);
		}
		a = Mathf.Max(a, m_CursorMinimumWidth + m_CursorBufferAdaptativeWidth);
		if (m_NextResizeIsInstant)
		{
			m_TargetWidth.SetNow(a);
			m_CurrentTextResizeTarget = null;
			Update();
		}
		else
		{
			m_CurrentTextResizeTarget = contentText;
			m_TargetWidth.Value = a;
		}
		m_NextResizeIsInstant = false;
	}

	public void ClearSelectionDescription()
	{
		if (m_SelectionDescription != null)
		{
			m_SelectionDescription.text = string.Empty;
		}
	}

	public void SetTargetTransform(RectTransform rectTransform, bool allowHorizontal, bool allowVertical, bool instant = false, Action callbackWhenDone = null)
	{
		m_CurrentTarget.Reset();
		m_CurrentTarget.Target = rectTransform;
		m_CurrentTarget.FollowHorizontal = allowHorizontal;
		m_CurrentTarget.FollowVertical = allowVertical;
		m_CallbackWhenDone = callbackWhenDone;
		m_JustMoved = true;
		if (!(m_NextMoveIsInstant || instant))
		{
			return;
		}
		if (m_CurrentTarget.IsValid())
		{
			Vector2 zero = Vector2.zero;
			if (m_UseWorldSpacePosition)
			{
				zero = new Vector2(m_CurrentTarget.Target.position.x, m_CurrentTarget.Target.position.y);
				if (!m_CurrentTarget.FollowHorizontal)
				{
					zero.x = m_RectTransform.position.x;
				}
				if (!m_CurrentTarget.FollowVertical)
				{
					zero.y = m_RectTransform.position.y;
				}
			}
			else
			{
				zero = new Vector2(m_CurrentTarget.Target.anchoredPosition.x, m_CurrentTarget.Target.anchoredPosition.y);
				if (!m_CurrentTarget.FollowHorizontal)
				{
					zero.x = m_RectTransform.anchoredPosition.x;
				}
				if (!m_CurrentTarget.FollowVertical)
				{
					zero.y = m_RectTransform.anchoredPosition.y;
				}
			}
			m_TargetPosition.SetNow(zero);
		}
		Update();
		m_NextMoveIsInstant = false;
	}

	public void SetTargetPosition(Vector2 position, Action callbackWhenDone = null)
	{
		m_JustMoved = true;
		m_CallbackWhenDone = callbackWhenDone;
		if (m_NextMoveIsInstant)
		{
			m_TargetPosition.SetNow(position);
			Update();
			m_NextMoveIsInstant = false;
		}
		else
		{
			m_CallbackWhenDone = callbackWhenDone;
			m_TargetPosition.Value = position;
		}
	}

	private void Update()
	{
		if (m_CurrentTarget.IsValid())
		{
			Vector2 zero = Vector2.zero;
			if (m_UseWorldSpacePosition)
			{
				zero = new Vector2(m_CurrentTarget.Target.position.x, m_CurrentTarget.Target.position.y);
				if (!m_CurrentTarget.FollowHorizontal)
				{
					zero.x = m_RectTransform.position.x;
				}
				if (!m_CurrentTarget.FollowVertical)
				{
					zero.y = m_RectTransform.position.y;
				}
			}
			else
			{
				zero = new Vector2(m_CurrentTarget.Target.anchoredPosition.x, m_CurrentTarget.Target.anchoredPosition.y);
				if (!m_CurrentTarget.FollowHorizontal)
				{
					zero.x = m_RectTransform.anchoredPosition.x;
				}
				if (!m_CurrentTarget.FollowVertical)
				{
					zero.y = m_RectTransform.anchoredPosition.y;
				}
			}
			m_TargetPosition.Value = zero;
		}
		m_TargetPosition.SetTrackingRate(m_TargetPositionTrackingRate);
		m_TargetPosition.Update(Time.unscaledDeltaTime);
		if (m_CurrentTextResizeTarget != null)
		{
			float a = m_CurrentTextResizeTarget.preferredWidth + m_CursorBufferAdaptativeWidth;
			if (m_SelectionDescription != null)
			{
				a = Mathf.Max(a, m_SelectionDescription.preferredWidth + m_CursorBufferAdaptativeWidth);
			}
			a = Mathf.Max(a, m_CursorMinimumWidth + m_CursorBufferAdaptativeWidth);
			m_TargetWidth.Value = a;
		}
		m_TargetWidth.SetTrackingRate(m_TargetWidthTrackingRate);
		m_TargetWidth.Update(Time.unscaledDeltaTime);
		if (m_JustMoved && m_TargetPosition.IsCloseEnough(m_CloseEnoughDistance))
		{
			if (m_CallbackWhenDone != null)
			{
				m_CallbackWhenDone();
			}
			m_CallbackWhenDone = null;
			if (!m_SkipNextSelectAnimation && m_Animator != null)
			{
				m_Animator.Play(m_OnSelectAnimation, 0, 0f);
			}
			m_SkipNextSelectAnimation = false;
			m_JustMoved = false;
		}
		if (m_CurrentTarget.IsValid() && m_TargetPosition.IsCloseEnough(2f))
		{
			m_CurrentTarget.Reset();
		}
		if (m_UseWorldSpacePosition)
		{
			if (m_TargetPosition.Value.x != m_RectTransform.position.x || m_TargetPosition.Value.y != m_RectTransform.position.y)
			{
				m_RectTransform.position = m_TargetPosition.Value;
				m_RectTransform.localPosition = new Vector3(m_RectTransform.localPosition.x, m_RectTransform.localPosition.y, 0f);
			}
		}
		else if (m_TargetPosition.Value.x != m_RectTransform.anchoredPosition.x || m_TargetPosition.Value.y != m_RectTransform.anchoredPosition.y)
		{
			m_RectTransform.anchoredPosition = m_TargetPosition.Value;
		}
		if (m_CurrentTextResizeTarget != null)
		{
			float value = m_TargetWidth.Value;
			if (Mathf.Abs(value - m_RectTransform.rect.width) > 1f)
			{
				m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
			}
			else
			{
				m_TargetWidth.SetNow(m_RectTransform.rect.width);
			}
		}
	}

	public void UpdateImage(Sprite sprite)
	{
		if (m_CursorImage != null)
		{
			m_CursorImage.sprite = sprite;
		}
	}
}

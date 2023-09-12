using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Rewired;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InGameConsole : MonoBehaviour
{
	private static InGameConsole s_instance;

	private UdpClient m_UdpClient;

	private Thread m_BroadcastThread;

	private Thread m_ReceiveThread;

	private bool m_Displayed;

	private bool m_ShouldReenableKeyboard;

	public GameObject m_ConsoleObject;

	public InputField m_InputField;

	public Text m_LogText;

	public int m_MaxLogBufferSize = 50;

	public int m_MaxHistorySize = 200;

	public List<string> m_LogTextBuffer = new List<string>();

	private List<string> m_SuccessfulCommandsHistory = new List<string>();

	private int m_CurrentHistoryIndex = -1;

	private GameObject m_PreviouslySelectedObject;

	private Transform m_InputFieldCaret;

	[Header("Line Suggestions")]
	private string m_InitialValue = "";

	public GameObject m_SuggestionRoot;

	public GameObject m_SuggestionsLayoutObject;

	public Text[] m_SuggestionTexts;

	public Color m_CurrentSelectionColor = Color.white;

	public Color m_UnselectedColor = Color.gray;

	private List<CommandLineData> m_Suggestions = new List<CommandLineData>();

	private int m_CurrentSuggestionIndex = -1;

	private bool m_controlEnabled = true;

	private const int c_BroadcastingReceiverPort = 8555;

	public string m_ClientName = "";

	public static InGameConsole Instance => s_instance;

	public bool Broadcasting { get; private set; }

	public bool Listening { get; private set; }

	private void OnValidate()
	{
		if (Application.isPlaying && base.gameObject.scene.isLoaded)
		{
			if (s_instance == null)
			{
				s_instance = this;
			}
			string[] array = m_LogTextBuffer.ToArray();
			m_LogTextBuffer.Clear();
			for (int i = 0; i < m_MaxLogBufferSize && i < array.Length; i++)
			{
				m_LogTextBuffer.Add(array[i]);
			}
		}
	}

	private void Awake()
	{
		m_ClientName = Application.productName + " [" + Application.platform.ToString() + "]";
		s_instance = this;
		m_LogTextBuffer = new List<string>();
		StartBroadcast();
	}

	private void OnDestroy()
	{
		StopBroadcast();
	}

	public void StartBroadcast()
	{
		m_UdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
		m_UdpClient.EnableBroadcast = true;
		Broadcasting = true;
		m_BroadcastThread = new Thread(Broadcast);
		m_BroadcastThread.Start();
		Listening = true;
		m_ReceiveThread = new Thread(ListenToCommands);
		m_ReceiveThread.Start();
	}

	public void StopBroadcast()
	{
		Broadcasting = false;
		Listening = false;
		if (m_BroadcastThread != null)
		{
			m_BroadcastThread.Abort();
		}
		if (m_ReceiveThread != null)
		{
			m_ReceiveThread.Abort();
		}
		if (m_UdpClient != null)
		{
			m_UdpClient.Close();
		}
		m_UdpClient = null;
		m_BroadcastThread = null;
	}

	private void ListenToCommands()
	{
		while (Listening)
		{
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
			byte[] bytes = m_UdpClient.Receive(ref remoteEP);
			string @string = Encoding.ASCII.GetString(bytes);
			ExecuteCommand(@string);
		}
	}

	private void Broadcast()
	{
		byte[] bytes = Encoding.ASCII.GetBytes(m_ClientName);
		IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 8555);
		while (Broadcasting)
		{
			m_UdpClient.Send(bytes, bytes.Length, endPoint);
			Thread.Sleep(500);
		}
	}

	private void Update()
	{
		if (m_Displayed && m_InputFieldCaret == null)
		{
			m_InputFieldCaret = m_ConsoleObject.transform.Find(m_InputField.name + " Input Caret");
			if (m_InputFieldCaret != null)
			{
				m_InputFieldCaret.SetParent(m_InputField.transform);
			}
		}
		if (m_Displayed && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.BackQuote))
		{
			ToggleControl();
		}
		else if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			ToggleDisplay();
		}
		else
		{
			if (!m_Displayed || !m_controlEnabled)
			{
				return;
			}
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				if (m_CurrentSuggestionIndex >= 0)
				{
					m_SuggestionTexts[m_CurrentSuggestionIndex].color = m_UnselectedColor;
					m_CurrentSuggestionIndex--;
					if (m_CurrentSuggestionIndex >= 0)
					{
						m_SuggestionTexts[m_CurrentSuggestionIndex].color = m_CurrentSelectionColor;
						if (m_Suggestions[m_CurrentSuggestionIndex].IsFieldCommand || (m_Suggestions[m_CurrentSuggestionIndex].IsMethodCommand && m_Suggestions[m_CurrentSuggestionIndex].Method.GetParameters().Length != 0 && m_Suggestions[m_CurrentSuggestionIndex].AllowArgumentsOverride))
						{
							SetInputFieldValue(m_Suggestions[m_CurrentSuggestionIndex].CommandLine);
						}
						else
						{
							SetInputFieldValue(m_Suggestions[m_CurrentSuggestionIndex].CommandLine);
						}
					}
					else
					{
						SetInputFieldValue(m_InitialValue);
						m_InitialValue = "";
					}
				}
				else if (m_CurrentHistoryIndex < m_SuccessfulCommandsHistory.Count - 1)
				{
					m_CurrentHistoryIndex++;
					SetInputFieldValue(m_SuccessfulCommandsHistory[m_SuccessfulCommandsHistory.Count - 1 - m_CurrentHistoryIndex]);
					ClearSuggestions();
				}
			}
			else
			{
				if (!Input.GetKeyDown(KeyCode.DownArrow))
				{
					return;
				}
				if (m_CurrentHistoryIndex > 0)
				{
					m_CurrentHistoryIndex--;
					SetInputFieldValue(m_SuccessfulCommandsHistory[m_SuccessfulCommandsHistory.Count - 1 - m_CurrentHistoryIndex]);
					ClearSuggestions();
				}
				else if (m_CurrentHistoryIndex == 0)
				{
					m_CurrentHistoryIndex--;
					SetInputFieldValue("");
				}
				else if (m_CurrentSuggestionIndex < m_SuggestionTexts.Length - 1 && m_CurrentSuggestionIndex < m_Suggestions.Count - 1)
				{
					if (m_CurrentSuggestionIndex >= 0)
					{
						m_SuggestionTexts[m_CurrentSuggestionIndex].color = m_UnselectedColor;
					}
					else
					{
						m_InitialValue = m_InputField.text;
					}
					m_CurrentSuggestionIndex++;
					m_SuggestionTexts[m_CurrentSuggestionIndex].color = m_CurrentSelectionColor;
					if (m_Suggestions[m_CurrentSuggestionIndex].IsFieldCommand || (m_Suggestions[m_CurrentSuggestionIndex].IsMethodCommand && m_Suggestions[m_CurrentSuggestionIndex].Method.GetParameters().Length != 0))
					{
						SetInputFieldValue(m_Suggestions[m_CurrentSuggestionIndex].CommandLine);
					}
					else
					{
						SetInputFieldValue(m_Suggestions[m_CurrentSuggestionIndex].CommandLine);
					}
				}
			}
		}
	}

	private void SetInputFieldValue(string value)
	{
		m_InputField.text = value;
		m_InputField.caretPosition = m_InputField.text.Length;
	}

	private void ToggleDisplay()
	{
		if (!m_Displayed)
		{
			Display();
		}
		else
		{
			Hide();
		}
	}

	private void ToggleControl()
	{
		if (!m_controlEnabled)
		{
			m_controlEnabled = true;
			m_InputField.enabled = true;
			if (EventSystem.current != null)
			{
				m_PreviouslySelectedObject = EventSystem.current.currentSelectedGameObject;
			}
			else
			{
				m_PreviouslySelectedObject = null;
			}
			m_InputField.ActivateInputField();
			return;
		}
		m_controlEnabled = false;
		m_InputField.enabled = false;
		if (EventSystem.current != null)
		{
			EventSystem.current.SetSelectedGameObject(m_PreviouslySelectedObject);
		}
		m_PreviouslySelectedObject = null;
		m_InputField.textComponent.text = "Control disabled, press Shift + ` to enable";
	}

	private void Display()
	{
		if (EventSystem.current == null)
		{
			base.gameObject.AddComponent<EventSystem>();
		}
		m_ShouldReenableKeyboard = ReInput.controllers.Keyboard.enabled;
		ReInput.controllers.Keyboard.enabled = false;
		m_ConsoleObject.SetActive(value: true);
		m_Displayed = true;
		if (m_controlEnabled)
		{
			if (EventSystem.current != null)
			{
				m_PreviouslySelectedObject = EventSystem.current.currentSelectedGameObject;
			}
			else
			{
				m_PreviouslySelectedObject = null;
			}
			m_InputField.ActivateInputField();
		}
	}

	private void Hide()
	{
		ReInput.controllers.Keyboard.enabled = m_ShouldReenableKeyboard;
		m_ConsoleObject.SetActive(value: false);
		if (EventSystem.current != null)
		{
			EventSystem.current.SetSelectedGameObject(m_PreviouslySelectedObject);
		}
		m_PreviouslySelectedObject = null;
		m_Displayed = false;
	}

	public void AddToLog(string log)
	{
		if (m_LogTextBuffer.Count >= m_MaxLogBufferSize)
		{
			m_LogTextBuffer.RemoveAt(0);
		}
		m_LogTextBuffer.Add(log);
		UpdateLog();
	}

	private void AddToHistory(string command)
	{
		if (m_SuccessfulCommandsHistory.Count == 0 || m_SuccessfulCommandsHistory[m_SuccessfulCommandsHistory.Count - 1] != command)
		{
			if (m_SuccessfulCommandsHistory.Count >= m_MaxHistorySize)
			{
				m_SuccessfulCommandsHistory.RemoveAt(0);
			}
			m_SuccessfulCommandsHistory.Add(command);
		}
	}

	public bool ExecuteCommand(string command)
	{
		return CommandLineHandler.CallCommandLine(command);
	}

	private void UpdateLog()
	{
		StringBuilder stringBuilder = new StringBuilder("");
		for (int i = 0; i < m_LogTextBuffer.Count; i++)
		{
			if (i == m_LogTextBuffer.Count - 1)
			{
				stringBuilder.Append(m_LogTextBuffer[i]);
			}
			else
			{
				stringBuilder.AppendLine(m_LogTextBuffer[i]);
			}
		}
		m_LogText.text = stringBuilder.ToString();
	}

	public void OnEndEdit(string value)
	{
		string text = ((value != "") ? value : m_InputField.text);
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			if (!string.IsNullOrEmpty(text))
			{
				if (ExecuteCommand(text))
				{
					AddToHistory(text);
					AddToLog(text);
				}
				else
				{
					AddToLog("Command not found: " + text);
				}
			}
		}
		else
		{
			Input.GetKeyDown(KeyCode.Escape);
		}
		m_CurrentHistoryIndex = -1;
		m_CurrentSuggestionIndex = -1;
		ClearSuggestions();
		SetInputFieldValue("");
		m_InputField.ActivateInputField();
	}

	public void LookForSuggestions(string value)
	{
		if (m_CurrentSuggestionIndex != -1)
		{
			return;
		}
		string text = m_InputField.text;
		m_Suggestions.Clear();
		string[] array = text.Split(' ');
		if (array[0] != "" && array.Length == 1)
		{
			Dictionary<string, CommandLineData>.Enumerator enumerator = CommandLineHandler.m_CommandLines.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Key.StartsWith(array[0]))
				{
					m_Suggestions.Add(enumerator.Current.Value);
				}
			}
		}
		if (m_Suggestions.Count == 0 && array[0].Length >= 3)
		{
			Dictionary<string, CommandLineData>.Enumerator enumerator2 = CommandLineHandler.m_CommandLines.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				if (enumerator2.Current.Key.Contains(array[0]))
				{
					m_Suggestions.Add(enumerator2.Current.Value);
				}
			}
			for (int i = 1; i < array.Length; i++)
			{
				for (int num = m_Suggestions.Count - 1; num >= 0; num--)
				{
					if (!m_Suggestions[num].CommandLine.Contains(array[i]))
					{
						m_Suggestions.RemoveAt(num);
					}
				}
			}
		}
		UpdateSuggestionsField();
	}

	private void UpdateSuggestionsField()
	{
		if (m_Suggestions.Count > 0)
		{
			m_SuggestionRoot.SetActive(value: true);
			m_SuggestionsLayoutObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_Suggestions.Count * 17);
			m_CurrentSuggestionIndex = -1;
			for (int i = 0; i < m_SuggestionTexts.Length; i++)
			{
				if (i < m_Suggestions.Count)
				{
					m_SuggestionTexts[i].gameObject.SetActive(value: true);
					m_SuggestionTexts[i].text = m_Suggestions[i].CommandLine + " //" + m_Suggestions[i].HelpText;
					m_SuggestionTexts[i].gameObject.name = m_Suggestions[i].CommandLine;
					m_SuggestionTexts[i].color = m_UnselectedColor;
				}
				else
				{
					m_SuggestionTexts[i].gameObject.SetActive(value: false);
				}
			}
		}
		else
		{
			m_SuggestionRoot.SetActive(value: false);
		}
	}

	public void ClearSuggestions()
	{
		m_Suggestions.Clear();
		UpdateSuggestionsField();
	}

	[CommandLine("clear", "Clear the console log", null, true)]
	private static void ClearLog()
	{
		if (Instance != null)
		{
			Instance.m_LogTextBuffer.Clear();
			Instance.UpdateLog();
		}
	}

	[CommandLine("console_size", "Changes the height of the console", new object[] { 400f }, true)]
	private static void ChangeConsoleHeight(float height)
	{
		height = Mathf.Clamp(height, 17f, 720f);
		RectTransform component = Instance.m_ConsoleObject.GetComponent<RectTransform>();
		if (component != null)
		{
			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}
	}

	[CommandLine("console_opacity", "Changes the opacity of the console (between 0.0 and 1.0)", new object[] { 0.5f }, true)]
	private static void ChangeConsoleOpacity(float opacity)
	{
		opacity = Mathf.Clamp01(opacity);
		Image component = Instance.m_ConsoleObject.GetComponent<Image>();
		if (component != null)
		{
			component.color = new Color(component.color.r, component.color.g, component.color.b, opacity);
		}
		Image component2 = Instance.m_SuggestionsLayoutObject.GetComponent<Image>();
		if (component2 != null)
		{
			component2.color = new Color(component2.color.r, component2.color.g, component2.color.b, opacity);
		}
	}
}

using System.Reflection;

public class CommandLineData
{
	private string m_CommandLine = "";

	private MethodInfo m_Method;

	private FieldInfo m_Field;

	private object[] m_Arguments;

	private string m_HelpText;

	private bool m_AllowArgumentsOverride;

	public string CommandLine => m_CommandLine;

	public MethodInfo Method => m_Method;

	public FieldInfo Field => m_Field;

	public bool IsMethodCommand => m_Method != null;

	public bool IsFieldCommand => m_Field != null;

	public object[] Arguments => m_Arguments;

	public string HelpText => m_HelpText;

	public bool AllowArgumentsOverride => m_AllowArgumentsOverride;

	public CommandLineData(string commandLine, MethodInfo method, object[] arguments, string helpText, bool allowArgumentsOverride)
	{
		m_CommandLine = commandLine;
		m_Method = method;
		m_Arguments = arguments;
		m_HelpText = helpText;
		m_AllowArgumentsOverride = allowArgumentsOverride;
	}

	public CommandLineData(string commandLine, FieldInfo field, object value, string helpText)
	{
		m_CommandLine = commandLine;
		m_Field = field;
		m_Arguments = new object[1] { value };
		m_HelpText = helpText;
	}

	public void SetNewFieldValue(object value)
	{
		m_Arguments = new object[1] { value };
	}

	public void SetNewArguments(object[] newArguments)
	{
		m_Arguments = newArguments;
	}

	public void MergeNewArguments(object[] newArguments)
	{
		for (int i = 0; i < newArguments.Length; i++)
		{
			if (newArguments[i] != null)
			{
				m_Arguments[i] = newArguments[i];
			}
		}
	}

	public void MergeFieldValue(object value)
	{
		if (m_Arguments != null && m_Arguments.Length != 0 && value != null)
		{
			m_Arguments[0] = value;
		}
	}
}

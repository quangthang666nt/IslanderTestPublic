using System;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true)]
public class CommandLineAttribute : Attribute
{
	private string m_CommandLine = "";

	public string m_HelpText;

	public object[] m_Arguments;

	public bool m_AllowArgumentsOverride;

	public string CommandLine => m_CommandLine;

	public CommandLineAttribute(string commandLine, string helpText = "", object[] arguments = null, bool allowArgumentsOverride = true)
	{
		m_CommandLine = commandLine;
		m_HelpText = helpText;
		m_Arguments = arguments;
		m_AllowArgumentsOverride = allowArgumentsOverride;
	}
}

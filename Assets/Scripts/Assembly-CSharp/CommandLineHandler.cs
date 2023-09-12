using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using UnityEngine;

public static class CommandLineHandler
{
	public static Dictionary<string, CommandLineData> m_CommandLines = null;

	public static Queue<CommandLineData> m_MainThreadCommandLines = new Queue<CommandLineData>();

	private static bool m_Initialised = false;

	private static string s_applicationName = "";

	public static string ApplicationName
	{
		get
		{
			return s_applicationName;
		}
		set
		{
			if (s_applicationName != value)
			{
				lock (s_applicationName)
				{
					s_applicationName = value;
				}
			}
		}
	}

	public static void Initialize()
	{
		if (!m_Initialised)
		{
			m_Initialised = true;
			m_CommandLines = new Dictionary<string, CommandLineData>();
			GatherCommandLineMethods(m_CommandLines);
		}
	}

	public static void AddCommandLineMethodsFromAssembly(Assembly assembly)
	{
		if (!m_Initialised)
		{
			Initialize();
		}
		GatherMethodsFromAssembly(assembly, m_CommandLines);
	}

	private static void GatherCommandLineMethods(Dictionary<string, CommandLineData> outDictionary)
	{
		if (!m_Initialised)
		{
			Initialize();
		}
		GatherMethodsFromAssembly(typeof(CommandLineHandler).Assembly, outDictionary);
	}

	private static void GatherMethodsFromAssembly(Assembly assembly, Dictionary<string, CommandLineData> outDictionary)
	{
		if (!m_Initialised)
		{
			Initialize();
		}
		Type[] types = assembly.GetTypes();
		for (int i = 0; i < types.Length; i++)
		{
			MethodInfo[] methods = types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo[] fields = types[i].GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			for (int j = 0; j < methods.Length; j++)
			{
				object[] customAttributes = methods[j].GetCustomAttributes(typeof(CommandLineAttribute), inherit: true);
				if (customAttributes.Length == 0)
				{
					continue;
				}
				for (int k = 0; k < customAttributes.Length; k++)
				{
					CommandLineAttribute commandLineAttribute = (CommandLineAttribute)customAttributes[k];
					if (commandLineAttribute == null)
					{
						continue;
					}
					ParameterInfo[] parameters = methods[j].GetParameters();
					object[] array = new object[parameters.Length];
					for (int l = 0; l < parameters.Length; l++)
					{
						if (commandLineAttribute.m_Arguments != null && l < commandLineAttribute.m_Arguments.Length)
						{
							if (commandLineAttribute.m_Arguments[l] == null)
							{
								array[l] = parameters[l].DefaultValue;
							}
							else
							{
								array[l] = commandLineAttribute.m_Arguments[l];
							}
						}
						else
						{
							array[l] = parameters[l].DefaultValue;
						}
					}
					CommandLineData value = new CommandLineData(commandLineAttribute.CommandLine, methods[j], array, commandLineAttribute.m_HelpText, commandLineAttribute.m_AllowArgumentsOverride);
					outDictionary.Add(commandLineAttribute.CommandLine, value);
				}
			}
			for (int m = 0; m < fields.Length; m++)
			{
				object[] customAttributes2 = fields[m].GetCustomAttributes(typeof(CommandLineAttribute), inherit: true);
				if (customAttributes2.Length == 0)
				{
					continue;
				}
				for (int n = 0; n < customAttributes2.Length; n++)
				{
					CommandLineAttribute commandLineAttribute2 = (CommandLineAttribute)customAttributes2[n];
					if (commandLineAttribute2 != null)
					{
						object value2 = null;
						if (commandLineAttribute2.m_Arguments != null && commandLineAttribute2.m_Arguments.Length != 0 && commandLineAttribute2.m_Arguments[0].GetType().Equals(fields[m].FieldType))
						{
							value2 = commandLineAttribute2.m_Arguments[0];
						}
						CommandLineData value3 = new CommandLineData(commandLineAttribute2.CommandLine, fields[m], value2, commandLineAttribute2.m_HelpText);
						outDictionary.Add(commandLineAttribute2.CommandLine, value3);
					}
				}
			}
		}
	}

	public static bool CallCommandLine(string command)
	{
		string[] array = command.Split(' ');
		if (!m_CommandLines.ContainsKey(array[0]))
		{
			return false;
		}
		return DispatchCommandLineToMainThread(ParseArguments(array, m_CommandLines[array[0]]));
	}

	public static bool CallMethod(CommandLineData command)
	{
		try
		{
			if (command.IsMethodCommand)
			{
				command.Method.Invoke(null, command.Arguments);
			}
			else if (command.IsFieldCommand)
			{
				if (command.Arguments[0] != null)
				{
					command.Field.SetValue(null, command.Arguments[0]);
				}
				else
				{
					Console.Log("Command '" + command.CommandLine + "' couldn't be called, missing argument of type " + command.Field.FieldType.ToString(), "red");
				}
			}
			return true;
		}
		catch (Exception message)
		{
			string text = command.CommandLine;
			if (command.IsMethodCommand)
			{
				ParameterInfo[] parameters = command.Method.GetParameters();
				for (int i = 0; i < parameters.Length; i++)
				{
					text = text + " " + parameters[i].ParameterType.ToString();
				}
			}
			Console.Log("Command '" + command.CommandLine + "' couldn't be called. Command should be: " + text, "red");
			Debug.Log(message);
			return false;
		}
	}

	private static bool DispatchCommandLineToMainThread(CommandLineData command)
	{
		m_MainThreadCommandLines.Enqueue(command);
		return true;
	}

	private static object ParseArgument(string argString, Type paramType)
	{
		if (paramType == typeof(int))
		{
			int result = 0;
			if (!int.TryParse(argString, out result))
			{
				return null;
			}
			return result;
		}
		if (paramType == typeof(float))
		{
			float result2 = 0f;
			if (!float.TryParse(argString, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result2))
			{
				return null;
			}
			return result2;
		}
		if (paramType == typeof(string))
		{
			string text = argString;
			if (text != null && text.StartsWith("\"") && text.EndsWith("\""))
			{
				text = text.Remove(0, 1);
				text = text.Remove(text.Length - 1, 1);
			}
			return text;
		}
		if (paramType == typeof(bool))
		{
			bool result3 = false;
			if (!bool.TryParse(argString, out result3))
			{
				int result4 = 0;
				if (!int.TryParse(argString, out result4))
				{
					return null;
				}
				result3 = result4 > 0;
			}
			return result3;
		}
		if (paramType.IsEnum && !string.IsNullOrEmpty(argString))
		{
			object obj = null;
			try
			{
				return Enum.Parse(paramType, argString, ignoreCase: true);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				return null;
			}
		}
		return null;
	}

	private static CommandLineData ParseArguments(string[] commandAndArgs, CommandLineData data)
	{
		CommandLineData commandLineData = null;
		if (data.IsMethodCommand)
		{
			commandLineData = new CommandLineData(data.CommandLine, data.Method, (object[])data.Arguments.Clone(), data.HelpText, data.AllowArgumentsOverride);
			if (data.AllowArgumentsOverride)
			{
				ParameterInfo[] parameters = data.Method.GetParameters();
				object[] array = new object[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					if (commandAndArgs.Length <= i + 1)
					{
						array[i] = ParseArgument(null, parameters[i].ParameterType);
					}
					else
					{
						array[i] = ParseArgument(commandAndArgs[i + 1], parameters[i].ParameterType);
					}
				}
				commandLineData.MergeNewArguments(array);
			}
		}
		else if (data.IsFieldCommand)
		{
			commandLineData = new CommandLineData(data.CommandLine, data.Field, data.Arguments[0], data.HelpText);
			if (commandAndArgs.Length > 1)
			{
				commandLineData.SetNewFieldValue(ParseArgument(commandAndArgs[1], data.Field.FieldType));
			}
			else
			{
				commandLineData.SetNewFieldValue(ParseArgument(null, data.Field.FieldType));
			}
		}
		return commandLineData;
	}

	[CommandLine("help", "Print the help text of any command line passed as parameter", null, true)]
	private static void HelpText(string commandLine)
	{
		if (m_CommandLines.ContainsKey(commandLine))
		{
			if (m_CommandLines[commandLine].HelpText != "")
			{
				Console.Log(commandLine + ": " + m_CommandLines[commandLine].HelpText);
			}
			else
			{
				Console.Log(commandLine + ": Could not find a help message :(");
			}
		}
		else
		{
			Console.Log("Command '" + commandLine + "' could not be found.");
		}
	}

	[CommandLine("attach_unity_output", "Attach or detach Unity's console output to the tool", null, true)]
	private static void CatchDebugOutput(bool enable)
	{
		if (enable)
		{
			Application.logMessageReceivedThreaded += RedirectLog;
		}
		else
		{
			Application.logMessageReceivedThreaded -= RedirectLog;
		}
	}

	private static void RedirectLog(string logString, string stackTrace, LogType type)
	{
		Console.Log("Unity " + type.ToString() + ": " + logString, "", null, logToUnity: false);
	}
}

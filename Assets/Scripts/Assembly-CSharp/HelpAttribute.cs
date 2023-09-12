using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class HelpAttribute : PropertyAttribute
{
	public readonly string text;

	public readonly MessageType type;

	public HelpAttribute(string text, MessageType type = MessageType.Info)
	{
		this.text = text;
		this.type = type;
	}
}

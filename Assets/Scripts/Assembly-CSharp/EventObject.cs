using UnityEngine;

[CreateAssetMenu(fileName = "EventObject", menuName = "SCS/Scriptable Basic Types/EventObject")]
public class EventObject : ScriptableObject
{
	public delegate void ObjectEvent(object arg);

	public ObjectEvent objectEvent;
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EventManager : MonoBehaviour
{

	//singleton reference
	public static EventManager info;

	//declaring events/delegates---
	public EventNotification<GameObject> onDeath;
	
	//declare any new events here; can call/invoke by InvokeEvent method
	
	//initialize before start
	void Awake ()
	{
		//referencing the singleton
		if (info == null) {
			info = this;
		}
	}
	
	//Event utilities-----------	
	
	/// <summary>
	/// Invokes event.
	/// </summary>
	public void invokeEvent<T> (ref EventNotification inEvent)
	{
		if (inEvent != null) {
			inEvent ();
		}
	}
	
	/// <summary>
	///  Invokes event with value
	/// </summary>
	/// <param name="inEvent">In event.</param>
	/// <param name="inObject">In object.</param>
	public void invokeEvent<T> (ref EventNotification<T> inEvent, T inObject)
	{
		if (inEvent != null) {
			inEvent (inObject);
		}
	}
	
}

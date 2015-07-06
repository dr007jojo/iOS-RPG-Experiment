using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


/// <summary>
/// Defines the method signature used for general notifications
/// </summary>
public delegate void EventNotification ();

/// <summary>
///generic Event notification with one param
/// </summary>
public delegate void EventNotification<T> (T Item);

/// <summary>
///generic method/event signature with 2 param
/// </summary>
public delegate void EventNotification<T,U> (T param1,U param2);
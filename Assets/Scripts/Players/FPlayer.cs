﻿using UnityEngine;
using System.Collections;

public class FPlayer : Character
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	/// <summary>
	/// when this character is activated. 
	/// </summary>
	public override void onActivated (GameObject currentTarget, bool bActive)
	{
		base.onActivated (currentTarget, bActive);
		
		battleInfo.info.playerFlag.GetComponent<ActiveCharacterInfo> ().setActivateArrow (this.transform);
		battleInfo.info.enemyFlag.GetComponent<ActiveCharacterInfo> ().setActivateArrow (currentTarget.transform);
	}
}
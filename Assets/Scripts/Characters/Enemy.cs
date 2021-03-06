﻿using UnityEngine;
using System.Collections;

public class Enemy : Character
{
	
	/// <summary>
	/// when this character is activated. 
	/// </summary>
	public override void onActivated (GameObject currentTarget, bool bActive)
	{
		base.onActivated (currentTarget, bActive);
	
		battleInfo.info.enemyFlag.GetComponent<ActiveCharacterInfo> ().setActivateArrow (this.transform);
		battleInfo.info.playerFlag.GetComponent<ActiveCharacterInfo> ().setActivateArrow (currentTarget.transform);
	}
}

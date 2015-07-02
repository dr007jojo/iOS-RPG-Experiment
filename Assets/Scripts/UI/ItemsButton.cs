using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ItemsButton : MonoBehaviour
{
	// use this to set the info of a skill button after crating it
	
	public Text itemButtonText;
	int itemId;
	
	public void SetData (string itemName, int itemId)
	{
		//gameObject.GetComponent<RectTransform> ().SetWidth (transform.parent.GetComponent<RectTransform> ().rect.width);
		
		itemButtonText.text = itemName;
		this.itemId = itemId;
	}
	
	public void OnClick ()
	{
		battleController.info.currentItemId = itemId;
		battleController.info.currentBattleAction = battleController.BattleActions.ITEM;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate IEnumerator ItemImplementation (GameObject caster,GameObject target);
public class ItemClass
{
	public bool mIsAreaItem;
	public string mItemName;
	public string mItemDescription;
	public ItemImplementation mItemImplementation; // assume it is an Area spell
}

public enum ItemNames // This will be used as int to save available items to playerData so dont change the number assigned to a item
{
	HealthPotion = 0,
	StunFlask = 1,
	// add more items below
};

public class ItemList : MonoBehaviour
{
	public static Dictionary<int, ItemClass> mAllItems;
	
	void Awake ()
	{
		
		mAllItems = new Dictionary<int, ItemClass> ();
		// add items here
		
		// health potion
		ItemClass healthPotionItem = new ItemClass ();
		healthPotionItem.mIsAreaItem = false;
		healthPotionItem.mItemName = "Healing Potion";
		healthPotionItem.mItemDescription = "Potion made from virgin's tears. Drink to restore health.";
		healthPotionItem.mItemImplementation = temp;
		mAllItems.Add ((int)ItemNames.HealthPotion, healthPotionItem);
		
		
		// stun
		ItemClass stunFlask = new ItemClass ();
		stunFlask.mIsAreaItem = false;
		stunFlask.mItemName = "Stun Flask";
		stunFlask.mItemDescription = "Throw the rum bottle your party had been drinking on the enemies to stun them.";
		stunFlask.mItemImplementation = temp;
		mAllItems.Add ((int)ItemNames.StunFlask, stunFlask);
	}
	
	public IEnumerator temp (GameObject caster, GameObject target)
	{
		Debug.Log ("Item used by " + caster.name + ". Target is " + target.name + ". Implementation pending.");
		yield return null;
	}
}

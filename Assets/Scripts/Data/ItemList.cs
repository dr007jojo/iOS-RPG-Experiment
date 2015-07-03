using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate IEnumerator ItemImplementation (GameObject caster,GameObject target);

public class ItemClass
{
	public TargetType mTargetType;

	public bool mIsAreaItem;
	public string mItemName;
	public string mItemDescription;
	public ItemImplementation mItemImplementation; // assume it is an Area spell
}

public enum ItemNames // This will be used as int to save available items to playerData so dont change the number assigned to a item
{
	HealthPotion = 0,
	StunFlask = 1,
	item_03 = 2,
	item_04 = 3,
	item_05 = 4,
	item_06 = 5,
	item_07 = 6
	// add more items below
}
;

public class ItemList : MonoBehaviour
{
	public static Dictionary<int, ItemClass> mAllItems;
	
	void Awake ()
	{
		
		mAllItems = new Dictionary<int, ItemClass> ();
		// add items here
		
		// health potion
		ItemClass healthPotionItem = new ItemClass ();
		healthPotionItem.mTargetType = TargetType.Player;
		healthPotionItem.mItemName = "Healing Potion";
		healthPotionItem.mItemDescription = "Potion made from virgin's tears. Drink to restore health.";
		healthPotionItem.mItemImplementation = addHPItemImplementation;
		mAllItems.Add ((int)ItemNames.HealthPotion, healthPotionItem);
		
		
		// stun
		ItemClass stunFlask = new ItemClass ();
		stunFlask.mTargetType = TargetType.Enemy;
		stunFlask.mItemName = "Stun Flask";
		stunFlask.mItemDescription = "Throw the rum bottle your party had been drinking on the enemies to stun them.";
		stunFlask.mItemImplementation = temp;
		mAllItems.Add ((int)ItemNames.StunFlask, stunFlask);
		
		// item 03
		ItemClass item_03 = new ItemClass ();
		item_03.mTargetType = TargetType.Area;
		item_03.mItemName = "Item 03";
		item_03.mItemDescription = "Explanation !!!";
		item_03.mItemImplementation = temp;
		mAllItems.Add ((int)ItemNames.item_03, item_03);
		
		// item 04
		ItemClass item_04 = new ItemClass ();
		item_04.mTargetType = TargetType.Player;
		item_04.mItemName = "Item 04";
		item_04.mItemDescription = "Explanation !!!";
		item_04.mItemImplementation = temp;
		mAllItems.Add ((int)ItemNames.item_04, item_04);
		
		// item 05
		ItemClass item_05 = new ItemClass ();
		item_05.mTargetType = TargetType.Player;
		item_05.mItemName = "Item 05";
		item_05.mItemDescription = "Explanation !!!";
		item_05.mItemImplementation = temp;
		mAllItems.Add ((int)ItemNames.item_05, item_05);
		
		// item 06
		ItemClass item_06 = new ItemClass ();
		item_06.mTargetType = TargetType.Player;
		item_06.mItemName = "Item 06";
		item_06.mItemDescription = "Explanation !!!";
		item_06.mItemImplementation = temp;
		mAllItems.Add ((int)ItemNames.item_06, item_06);
		
		// item 07
		ItemClass item_07 = new ItemClass ();
		item_07.mTargetType = TargetType.Player;
		item_07.mItemName = "Item 07";
		item_07.mItemDescription = "Explanation !!!";
		item_07.mItemImplementation = temp;
		mAllItems.Add ((int)ItemNames.item_07, item_07);
	}
	public IEnumerator addHPItemImplementation (GameObject caster, GameObject target)
	{
		Debug.Log ("Health potion used by " + caster.name + ". Target is " + target.name);
		Character Attacker = caster.GetComponent<Character> ();
		yield return Attacker.StartCoroutine (Attacker.addHealthPoint (target, 1.0f));
	}
	
	public IEnumerator temp (GameObject caster, GameObject target)
	{
		Debug.Log ("Item used by " + caster.name + ". Target is " + target.name + ". Implementation pending.");
		yield return null;
	}
}

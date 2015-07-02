using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

using UnityEngine.Events;

public enum menus
{
	None,
	Item,
	Skill,
	Pause,
	Retreat
	
	//add all the menus here
}

public class Hud : MonoBehaviour
{
	
	public Text labelLevel;
	public Text labelGold;
	public Slider SliderXP;
	public Transform skillsButtonHolder;
	public Transform itemsButtonHolder;
	
	public GameObject skillsPanel;
	public GameObject itemsPanel;
	
	public SkillButton skillButtonInstance;
	public ItemsButton itemButtonInstance;
	
	private float skillTop;
	private SkillButton skillBtn;
	private ItemsButton itemBtn;

	public UIPopupBox retreatConfirmBox;
	public UIMessage messageBox;
	
	static Hud thisInstance = null;
	
	public menus currentMenu = menus.None;	//current menu active
	
	void Awake ()
	{
		thisInstance = this;
	}
	
	public static Hud GetInstance ()
	{
		return thisInstance;
	}
	
	void Start ()
	{
		skillTop = skillsPanel.GetComponent<RectTransform> ().offsetMax.y;
		//设置两个面板为未激活状态
		skillsPanel.SetActive (false);
		itemsPanel.SetActive (false);
		retreatConfirmBox.gameObject.SetActive (false);
		messageBox.gameObject.SetActive (false);
	}
	
	void Update ()
	{
		labelLevel.text = "LEVEL : " + PlayerData.GetInstance ().partyLevel; //更新文本内容
		labelGold.text = "GOLD : " + PlayerData.GetInstance ().partyGold;
		
		float xp = (float)PlayerData.GetInstance ().partyExperience;
		SliderXP.value = (xp / 100.0f);
	}
	
	public void OnSave ()
	{
		PlayerData.GetInstance ().Save ();
	}
	
	public void OnLoad ()
	{
		PlayerData.GetInstance ().Load ();
	}
	
	//shows the respective skills on the skill menu
	public void OnShowSkills ()
	{
		if (currentMenu != menus.None)	//if any menu is open, just return
			return;						//TODO: Dont return! That could cause infinite loop in BattleController. Instead close the other menus.
		
		skillsPanel.SetActive (true);
		currentMenu = menus.Skill;	//skill menu active
		
		foreach (Transform child in skillsButtonHolder) {//we click skills button again,we destroy the old skill
			if (child != null) {
				Destroy (child.gameObject);
			}
		}
		
		int currentCharacter = battleController.info.currentPlayerIndex;//第几个元素
		int totalNumberOfSkills = Enum.GetNames (typeof(SkillNames)).Length;//skillList中枚举skilname的元素长度
		
		int numberOfButtons = 0;
		
		if (currentCharacter == 0) {
			for (int id = 0; id < totalNumberOfSkills; ++id) {
				if (PlayerData.GetInstance ().mPartyLeaderSkills [id]) {
				
					skillBtn = Instantiate (skillButtonInstance) as SkillButton;
					skillBtn.SetData (SkillList.mAllSkills [id].mSkillName, id);
					skillBtn.transform.SetParent (skillsButtonHolder);
					numberOfButtons += 1;
					// 每次获取playData的技能解锁数据,改变我们的技能按钮，通过skillButton实时更新
				}
			}
		}
		if (currentCharacter == 1) {
			for (int id = 0; id < totalNumberOfSkills; ++id) {
				if (PlayerData.GetInstance ().mPartySecondCharSkills [id]) {
					
					skillBtn = Instantiate (skillButtonInstance) as SkillButton;
					skillBtn.SetData (SkillList.mAllSkills [id].mSkillName, id);
					skillBtn.transform.SetParent (skillsButtonHolder);
					numberOfButtons += 1;
					// 每次获取playData的技能解锁数据,改变我们的技能按钮，通过skillButton实时更新
				}
			}
		}
		if (currentCharacter == 2) {
			for (int id = 0; id < totalNumberOfSkills; ++id) {
				if (PlayerData.GetInstance ().mPartyThirdCharSkills [id]) {
		
					skillBtn = Instantiate (skillButtonInstance) as SkillButton;
					skillBtn.SetData (SkillList.mAllSkills [id].mSkillName, id);
					skillBtn.transform.SetParent (skillsButtonHolder);
					numberOfButtons += 1;
					// 每次获取playData的技能解锁数据,改变我们的技能按钮，通过skillButton实时更新
				}
			}
		}
		
	}
	
	public void OnHideSkills ()
	{
		foreach (Transform child in skillsButtonHolder) {
			Destroy (child.gameObject);
		}
		skillsPanel.SetActive (false);
		
		if (currentMenu != menus.None)	//on lose menu
			currentMenu = menus.None;
	}
	
	//showx the respective items on the item menu
	public void OnShowItems ()
	{
		if (currentMenu != menus.None)	//if any menu is open, just return
			return;						//TODO: Dont return! That could cause infinite loop in BattleController. Instead close the other menus.
			
		itemsPanel.SetActive (true);
		currentMenu = menus.Item;	//item menu active
		
		int totalNmeberOfItems = Enum.GetNames (typeof(ItemNames)).Length;
		int numberOfButtons = 0;
		
		for (int id = 0; id < totalNmeberOfItems; ++id) {
			uint quantity = PlayerData.GetInstance ().mPartyItems [id];
			
			if (quantity > 0) { // spawn item button only if party has more than one of it 道具数量至少有1个才能触发
				itemBtn = Instantiate (itemButtonInstance) as ItemsButton;
				string buttonName = ItemList.mAllItems [id].mItemName + " X" + quantity;
				itemBtn.transform.SetParent (itemsButtonHolder);
				itemBtn.SetData (buttonName, id);
				numberOfButtons += 1;
			}
		}
	}
	
	public void OnHideItems ()
	{
		foreach (Transform child in itemsButtonHolder) {
			Destroy (child.gameObject);
		}
		itemsPanel.SetActive (false);
		
		if (currentMenu != menus.None)	//on close menu
			currentMenu = menus.None;
	}
	
	public void ShowRetreatConfirmation ()
	{
		if (currentMenu != menus.None)	//TODO: Dont return! That could cause infinite loop in BattleController. Instead close the other menus.
			return;

		retreatConfirmBox.gameObject.SetActive (true);
		currentMenu = menus.Retreat;

		retreatConfirmBox.SetData ("Are you sure you want to retreat?", OnRetreatYes, OnRetreatNo);
		battleController.info.OnRetreat ();
	}

	public void OnClickAttack ()
	{
		battleController.info.OnAttack ();
	}

	public void OnRetreatYes ()
	{
		currentMenu = menus.None;
		retreatConfirmBox.gameObject.SetActive (false);
		battleController.info.OnRetreatConfirm ();
	}

	public void OnRetreatNo ()
	{
		currentMenu = menus.None;
		retreatConfirmBox.gameObject.SetActive (false);
		battleController.info.OnRetreatCancel ();
	}

	public void ShowRetreatSuccessMessage ()
	{
		Debug.Log ("Retreat Success");
		messageBox.gameObject.SetActive (true);
		messageBox.ShowWithText ("Retreat Success!");
	}

	public void ShowRetreatFailMessage ()
	{
		Debug.Log ("Retreat Fail");
		messageBox.gameObject.SetActive (true);
		messageBox.ShowWithText ("Retreat Failed!");
	}

	public void ShowBattleStartMessage ()
	{
		Debug.Log ("Battle Start");
		messageBox.gameObject.SetActive (true);
		messageBox.ShowWithText ("Battle!");
	}

	public void ShowBattleWinMessage ()
	{
		Debug.Log ("Battle Win");
		messageBox.gameObject.SetActive (true);
		messageBox.ShowWithText ("Battle Won!");
	}

	public void ShowBattleLostMessage ()
	{
		Debug.Log ("Battle Lose");
		messageBox.gameObject.SetActive (true);
		messageBox.ShowWithText ("Battle Lost!");
	}
	

}



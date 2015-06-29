using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

using UnityEngine.Events;

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
	
	private SkillButton skillBtn;
	private ItemsButton itemBtn;
	
	static Hud thisInstance = null;
	
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
		//设置两个面板为未激活状态
		skillsPanel.SetActive (false);
		itemsPanel.SetActive (false);
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
		skillsPanel.SetActive (true);
		
		int currentCharacter = battleController.info.currentPlayerIndex;//第几个元素
		int totalNumberOfSkills = Enum.GetNames (typeof(SkillNames)).Length;//skillList中枚举skilname的元素长度
		
		float btnY = 130;
		float btnBaseXPos = 70;
		float perButtonXPosMultiplier = 120;
		int numberOfButtons = 0;
		
		if (currentCharacter == 0) {
			for (int id = 0; id < totalNumberOfSkills; ++id) {
				if (PlayerData.GetInstance ().mPartyLeaderSkills [id]) {
					float btnX = btnBaseXPos + perButtonXPosMultiplier * numberOfButtons;
					skillBtn = Instantiate (skillButtonInstance, new Vector3 (btnX, btnY, 0), Quaternion.identity) as SkillButton;
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
					float btnX = btnBaseXPos + perButtonXPosMultiplier * numberOfButtons;
					skillBtn = Instantiate (skillButtonInstance, new Vector3 (btnX, btnY, 0), Quaternion.identity) as SkillButton;
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
					float btnX = btnBaseXPos + perButtonXPosMultiplier * numberOfButtons;
					skillBtn = Instantiate (skillButtonInstance, new Vector3 (btnX, btnY, 0), Quaternion.identity) as SkillButton;
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
	}
	
	//showx the respective items on the item menu
	public void OnShowItems ()
	{
		itemsPanel.SetActive (true);
		
		int totalNmeberOfItems = Enum.GetNames (typeof(ItemNames)).Length;
		float btnX = 230;
		float btnBaseYPos = 480;
		float perButtonYPosMultiplier = 50;
		int numberOfButtons = 0;
		
		for (int id = 0; id < totalNmeberOfItems; ++id) {
			uint quantity = PlayerData.GetInstance ().mPartyItems [id];
			
			if (quantity > 0) { // spawn item button only if party has more than one of it 道具数量至少有1个才能触发
				float btnY = btnBaseYPos - perButtonYPosMultiplier * numberOfButtons;
				itemBtn = Instantiate (itemButtonInstance, new Vector3 (btnX, btnY, 0), Quaternion.identity) as ItemsButton;
				string buttonName = ItemList.mAllItems [id].mItemName + " X" + quantity;
				itemBtn.SetData (buttonName, id);
				itemBtn.transform.SetParent (itemsButtonHolder);
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
	}
	
	//changing the state to attack
	public void OnAttack ()
	{
		battleController.info.currentBattleAction = battleController.BattleActions.ATTACK;
	}
}



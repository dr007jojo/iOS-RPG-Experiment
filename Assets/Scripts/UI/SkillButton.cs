using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SkillButton : MonoBehaviour
{
	// use this to set the info of a skill button after crating it
	// 存储放置技能按钮的信息，用于实例化
	
	public Text skillButtonText;
	int skillId;
	
	public void SetData (string skillName, int skillId)
	{
		skillButtonText.text = skillName;
		this.skillId = skillId;
	}
	
	public void OnClick ()
	{
		battleController.info.currentSkillId = skillId;
		battleController.info.currentBattleAction = battleController.BattleActions.SKILL;	
		Hud.GetInstance().OnHideSkills();
	}
}

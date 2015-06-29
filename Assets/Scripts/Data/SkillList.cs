using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//委托，将方法temp作为参数，可以在其他脚本创建对象调用
public delegate IEnumerator SkillImplementation (GameObject caster,GameObject target);
public class SkillClass
{
	public bool mIsAreaSpell;
	public int mManaRequired;
	public string mSkillName;
	public string mSkillDescription;
	public SkillImplementation mSkillImplementation; // assume it is an Area spell 一个委托的实例
}

//编号保存技能
public enum SkillNames // This will be used as int to save unlocked skills to playerData so dont change the number assigned to a skill
{
	MultiAttack = 0,
	Hex = 1,
	BackStab = 2
	// add more skills below
}
;


public class SkillList : MonoBehaviour
{
	//静态字典 
	public static Dictionary<int, SkillClass> mAllSkills;
	
	void Awake ()
	{
		
		mAllSkills = new Dictionary<int, SkillClass> ();
		// add skills here
		
		// multi-attack skill 攻击多人
		SkillClass multiAttackSkill = new SkillClass ();
		multiAttackSkill.mIsAreaSpell = true;
		multiAttackSkill.mManaRequired = 25;
		multiAttackSkill.mSkillName = "Multi-Attack";
		multiAttackSkill.mSkillDescription = "The hero moves like lighting and attacks all the enemies in one move. Consumes " + multiAttackSkill.mManaRequired + " mana.";
		multiAttackSkill.mSkillImplementation = multiAttackSkillImplementation;
		mAllSkills.Add ((int)SkillNames.MultiAttack, multiAttackSkill);
		
		
		// hex skill
		SkillClass hexSkill = new SkillClass ();
		multiAttackSkill.mIsAreaSpell = false;
		hexSkill.mManaRequired = 50;
		hexSkill.mSkillName = "Hex";
		hexSkill.mSkillDescription = "Use arcane powers to one enemy into chicken for the next turn. Consumes " + hexSkill.mManaRequired + " mana.";
		hexSkill.mSkillImplementation = temp;
		mAllSkills.Add ((int)SkillNames.Hex, hexSkill);
		
		// back-stab skill
		SkillClass backStabSkill = new SkillClass ();
		multiAttackSkill.mIsAreaSpell = false;
		backStabSkill.mManaRequired = 30;
		backStabSkill.mSkillName = "Backstab";
		backStabSkill.mSkillDescription = "The hero sinks into the shadows and attacks unseen to deal 3X damage. Consumes " + backStabSkill.mManaRequired + " mana.";
		backStabSkill.mSkillImplementation = temp;
		mAllSkills.Add ((int)SkillNames.BackStab, backStabSkill);
	}
	
	public IEnumerator multiAttackSkillImplementation (GameObject caster, GameObject target)
	{
		Character Attacker = caster.GetComponent<Character> ();
		yield return Attacker.StartCoroutine (Attacker.fightMultiple (battleController.info.Enemies, 1.0f));
	}
	
	public IEnumerator temp (GameObject caster, GameObject target)
	{
		Debug.Log ("Skill activated by " + caster.name + ". Target is " + target.name + ". Implementation pending.");
		yield return null;
	}
}

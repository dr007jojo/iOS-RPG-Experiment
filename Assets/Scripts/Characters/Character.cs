using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using System;		//using for system defined delegates like Action<T1, T2>


/// <summary>
/// Main Character class, holding basic variable and methods of any character
/// </summary>
public class Character : Actor
{

	// basic attributes---------
	public float damage = 10;			//basic damage this character can give
	public float mana = 100;			//current mana of the player
	public float maxMana = 100;			//max mana a character can have
	public int characterLevel = 1;				//currrent level of the character
	//------
	
	//---fight status---
	public Slider manaSlider;			//to reference the slider component for Mana
	
	
	//initialize
	public override void postBeginPlay ()
	{
		base.postBeginPlay ();
		
		//updating the health and mana slider at the begining
		updateHealthSlider ();
		updateManaSlider ();
	}
	
	//-----Fight Related----
	
	/// <summary>
	/// Fight the specified target for fightDuration.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="fightDuration">Fight duration.</param>
	virtual public void fight (GameObject target, out Tweener fightEndTweener, float fightDuration = 0.425f, bool useBonusDamage=false, float bonusDamage = 10)
	{
		
		if (DOTween.IsTweening (transform)) {	//if any tween is playing, dont play it again
			fightEndTweener = null;		//it will null as this whole function will be useless
			return;
		}
		
		Debug.Log ("i, " + gameObject + " is fighting: " + target);
		
		onActivated (target, true);		//calling on activates as soon as the fignting starts
		gotoAndAttack (target, fightDuration, out fightEndTweener, useBonusDamage, bonusDamage);		//basic fight action
	}
	
	
	
	
	/// <summary>
	/// Shake this instance
	/// </summary>
	void  shake (GameObject target, float duration=1, float strength=1, int vibrato = 5, float randomness = 10, Action<float,GameObject> InMethod = null, float inValue = 0)
	{
		target.transform.DOShakePosition (duration, strength, vibrato, randomness)		//tween shake
		//.OnComplete (() => doDamage (damage, target));									//on complete, do the damage to the target
		.OnComplete (() => InMethod (inValue, target));
	}
	
	/// <summary>
	/// Goto and attack the traget for the fight duration
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="fightDuration">Fight duration.</param>
	void gotoAndAttack (GameObject target, float fightDuration, out Tweener fightEndTweener, bool useBonusDamage, float bonusDamage)
	{
		Sequence attackSequence = DOTween.Sequence ();	//initializing the sequence
		Vector3 targetPos = target.transform.position;	
		Vector3 originalCharacterPos = transform.position;
		float thisAttackDamage = damage;
		
		Renderer targetRenderer = target.GetComponent<Renderer> ();		//referencing the general renderer of the target
		SpriteRenderer thisSpriteRenderer = gameObject.GetComponent<SpriteRenderer> ();	//referencing the sprite renderer of this object
		
		//safety for null reference
		if (targetRenderer == null || thisSpriteRenderer == null) {
			fightEndTweener = null;		//it will null as this whole function will be useless
			return;
		}
		
		//damage manipulation
		if (useBonusDamage) {		//if using bonus damage
			thisAttackDamage = bonusDamage;
		}
		
		//creating the sequence
		attackSequence.Append (transform.DOMove (new Vector3 ((targetPos.x + (targetRenderer.bounds.extents.x * transform.localScale.x)), targetPos.y, targetPos.z), 0.5f)	//adjusting the character position with the bounds of the target's renderer
			.SetEase (Ease.InOutElastic)
			.OnComplete (() => shake (target, 1f, 0.2f, 7, 1, doDamage, thisAttackDamage)))		//Move to the target position and then shake the target; onComplete -> doDamage
		.Join (thisSpriteRenderer.DOFade (0f, 0.175f).SetDelay (0.15f))		//simultaneously fade the active character to 0
		.Append (thisSpriteRenderer.DOFade (1, 0.5f));			//then, fade the active character to 1
		
		//after the fighting sequence the character has to go to the original position
		fightEndTweener = backToIdle (originalCharacterPos, 0.5f, fightDuration);
	
	}
	
	/// <summary>
	/// Backs to idle pos.
	/// </summary>
	/// <param name="originalPos">Original position.</param>
	/// <param name="timeAfter">Time after.</param>
	protected Tweener backToIdle (Vector3 originalPos, float moveTime, float timeAfter)
	{
		//onActivated (null, false);	//deactivating the infos
		//tween for moving to the original position
		return transform.DOMove (originalPos, moveTime).SetDelay (timeAfter)
		.OnComplete (() => onActivated (null, false));	//deactivating the infos
	}
	
	//skill related----------------
	
	/// <summary>
	/// skill - Fights multiple target -> basic use is select the character and press multiple attack button it will attack multiple targets---
	/// </summary>
	/// <param name="targets">Targets.</param>
	/// <param name="fightEndTweener">Fight end tweener.</param>
	/// <param name="eachFightDuration">Each fight duration.</param>
	virtual public IEnumerator fightMultiple (List<GameObject> targets, float eachFightDuration = 0.5f, float requiredMana = 25)
	{
		
		if (DOTween.TotalPlayingTweens () > 0 || !checkForMana (requiredMana) || targets.Count <= 0) {	//if any tween is playing, dont play it again and there are enough mana or there are targets available
			yield return null;
		}
		
		updateMana (-25f);		//updating the mana cost
		
		foreach (var target in targets.ToArray()) {	//for each target
			
			Tweener yieldTweener;
			
			if (targets.Count <= 0) 	//break it if there is no enemies, for safety
				break;
			
			
			fight (target, out yieldTweener, eachFightDuration); //fighting each target
			
			
			if (yieldTweener != null) {
				yield return yieldTweener.WaitForCompletion ();	//watiing for the fight to finish*
			}
			yield return null;
		}
		
	}
	
	/// <summary>
	/// Updates the mana.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void updateMana (float amount)
	{
		mana = Mathf.Clamp (mana + amount, 0, maxMana);	//clamping the resulting value to go beyond 0
		
		//updating the slider
		updateManaSlider ();
	}
	
	// functions to add health
	// Refactored -> using same shake function. shake definition modified with Action<T1, T2> delegate
	virtual public IEnumerator addHealthPoint (GameObject target, float eachFightDuration = 0.5f)
	{
		if (DOTween.TotalPlayingTweens () > 0 || !target) {
			yield return null;
		}
		
		//shake with onComplete-> doReply
		shake (target, 1f, 0.2f, 5, 5f, doReply, 20.0f);		//-->20.0f will change to a variable
	}

	public void doReply (float replyHp, GameObject target)
	{
		if (target.GetComponent<Actor> () == null)
			return;
		
		target.GetComponent<Actor> ().ReplyHP (replyHp, target, this);
	}

	
	//utilities-----------------------
	
	//Mana Slider
	protected void updateManaSlider ()
	{
		if (manaSlider != null) {
			manaSlider.value = mana;	//slider amount to that of mana
		}
	}
	
	/// <summary>
	/// Checks for mana. True, if available
	/// </summary>
	/// <returns><c>true</c>, if for mana was checked, <c>false</c> otherwise.</returns>
	/// <param name="requiredMana">Required mana.</param>
	protected bool checkForMana (float requiredMana)
	{
		return (mana >= requiredMana) ? true : false;
	}
	
	/// <summary>
	/// when this character is activated. 
	/// </summary>
	virtual public void onActivated (GameObject currentTarget = null, bool bActive =false)
	{
		//we may write some common feature here later
		
		//(test)-will make it more efficient later
		battleController.info.enemyFlag.SetActive (bActive);
		battleController.info.playerFlag.SetActive (bActive);
		
		//implement in the subclass
	}
	
	
	
	
}

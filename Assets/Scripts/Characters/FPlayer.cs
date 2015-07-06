using UnityEngine;
using System.Collections;
using System.Linq;
using DG.Tweening;

public class FPlayer : Character
{
	[Tooltip("base gold decreases upon death")]
	public int
		baseGoldPanaltyOnDeath = 30;		//base gold decreases upon death
	
	public bool hasPlayedThisTurn = false;
	
	/// <summary>
	/// when this character is activated. 
	/// </summary>
	public override void onActivated (GameObject currentTarget, bool bActive)
	{
		base.onActivated (currentTarget, bActive);
		
		battleController.info.playerFlag.GetComponent<ActiveCharacterInfo> ().setActivateArrow (this.transform);
		
		if (currentTarget != null)		//only if valid
			battleController.info.enemyFlag.GetComponent<ActiveCharacterInfo> ().setActivateArrow (currentTarget.transform);
	}
	
	/// <summary>
	/// overriding on death. 
	/// </summary>
	public override void onDeath ()
	{
		base.onDeath ();
		
		//decrease gold when ally dies
		battleController.info.decreaseGoldOnPlayerPartyDeath (characterLevel);
		
		//destroying the gameobject
		Destroy (this.gameObject);
	}
	
	//--------joint attack related....
	/// <summary>
	/// Comes to me and attack a single enemy
	/// </summary>
	public IEnumerator comeToMeAndAttack (GameObject[] inAllies, GameObject target, float durationMax, float durationMin)
	{
		Vector3[] originalPositions = new Vector3[inAllies.Length];
		
		//grouping with a random allies
		foreach (var gObject in inAllies.Select((value,i)=>new {i, value})) {
			originalPositions [gObject.i] = gObject.value.transform.position;	//saving original positions relative to the array index
			gObject.value.transform.DOMove (transform.position.getRandomVector3Around (0.5f, 0.2f), UnityEngine.Random.Range (durationMax, durationMin));	//Move Tween
		}
		
		//wait before the charge button appers
		yield return new WaitForSeconds (0.5f);
		EventManager.info.invokeEvent (ref EventManager.info.onJointAttack, true);
		yield return new WaitForSeconds (3F);
		EventManager.info.invokeEvent (ref EventManager.info.onJointAttack, false);
		
		//damage calculation
		float increasedDamage = damage * battleController.info.joinAttackQTENumber;
			
		Tweener yieldTweener = null;
		
		//fight and deal damage
		foreach (var gObject in inAllies) {
			
			if (target == null)	//if targets die in between the attack
				break;
			
			gObject.GetComponent<FPlayer> ().fight (target, out yieldTweener, 0.5f, true, increasedDamage);
			
			if (yieldTweener != null)								//removing this two lines will make all the allies attack at the same time
				yield return yieldTweener.WaitForCompletion ();
			
			yield return null;
		}
		
		//return to original position
		foreach (var gObject in inAllies.Select((value,i)=>new {i, value})) {
			yieldTweener = gObject.value.GetComponent<FPlayer> ().backToIdle (originalPositions [gObject.i], 0.3f, 0.1f);
		}
		
		if (yieldTweener != null)
			yield return yieldTweener.WaitForCompletion ();
		else 
			yield return new WaitForSeconds (0.5f);	//if there is no calculated yield, just wait for 0.5 sec
	}
}

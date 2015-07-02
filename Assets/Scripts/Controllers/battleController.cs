using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class battleController : MonoBehaviour
{
//----
	public static battleController info;				//self reference for Class singleton, this class can only have 1 instance
//----
	public List<GameObject> Players;		//list of players
	public List<GameObject> Enemies;			//list of enemies
	
	public GameObject playerFlag;		//arrow to notify which character is active
	public GameObject enemyFlag;		//arrow to notify which character is active
	
	int numberOfRemaingPlayerCharactersThisTurn;
	bool isPlayerTurn = false;
	
	public int currentPlayerIndex = 1; // so that we can show correct skills 标记记录，显示正确的技能
	int currentEnemyIndex = 0; 
	
	public int currentSkillId = -1;
	public int currentItemId = -1;
	
	//actions while battle scene is active
	public enum BattleActions
	{
		NONE,
		ATTACK,
		JOINT_ATTACK,
		SKILL,
		ITEM,
		RETREAT
	}
	;
	public BattleActions currentBattleAction = BattleActions.NONE;	//default
	bool isPlayerRetreating = false;	//player retreat
	
	
	// swipe variables
	Vector2 startPos = new Vector2 (0.0f, 0.0f);
	bool couldBeSwipe = false;
	float startTime = 0.0f;
	float comfortZone = 0.001f;
	float maxSwipeTime = 5.0f;
	float minSwipeDist = 0.01f;
	
	
	[Tooltip("base xp granted to the winner")]
	public int
		baseXPGranted = 20;	//base xp granted to the winner
	
	[Tooltip("base gold decreases upon death")]
	public int
		baseGoldPanaltyOnDeath = 30;	//base gold decreases upon death
	
	//every initial init
	void Awake ()
	{
		//initializing singleton
		info = this;
		
	}
	
	void OnEnable ()
	{
		//registering event
		EventManager.info.onDeath += onActorDeath;
	}
	
	void OnDisable ()
	{
		//deregistering event
		EventManager.info.onDeath -= onActorDeath;
	}
	
	// Use this for initialization
	void Start ()
	{
		StartCoroutine (battleSequence ());
	}
	
	//main battle sequence
	IEnumerator battleSequence ()
	{
		Debug.Log ("Start battle");
		isPlayerRetreating = false;
		
		do {
			yield return StartCoroutine (playerTurn ());
			
			if (isPlayerRetreating)
				break;
			yield return StartCoroutine (enemyAttack ());
			
		} while(Players.Any() && Enemies.Any());
		
		Debug.Log ("Battle End");

		if (isPlayerRetreating)
			OnBattleRetreat ();
		else if (Enemies.Any ())
			OnBattleLose ();
		else
			OnBattleWin ();

		yield return null;
	}

	// call the distribute the winnings according to enemy level
	public void OnBattleWin ()
	{
		Debug.Log ("Battle Win");
	}

	// deduct gold based on player level
	public void OnBattleLose ()
	{
		Debug.Log ("Battle Lose");
	}

	// deduct less gold than losing
	public void OnBattleRetreat ()
	{
		Debug.Log ("Retreat");
	}
	
	/// <summary>
	/// Fights  multiple target.
	/// </summary>
	/// <param name="Attacker">Attacker.</param>
	public void fightMultiple (Character Attacker)
	{
		if (Players.Count <= 0)	//early exit if there is no player available
			return;
			
		
		Attacker = Players [Random.Range (0, Players.Count)].GetComponent<Character> ();	//getting a random element
		
		Attacker.StartCoroutine (Attacker.fightMultiple (Enemies, 1.0f));	//selected attacker will attack all enimies
	}
	
	/// <summary>
	/// called from Actor class, when a associated character died
	/// </summary>
	/// <param name="inActor">In actor.</param>
	void onActorDeath (GameObject inActor)
	{
		Debug.Log ("Actor died ##: " + inActor.ToString ());
		
		//finds the inActor in the player/enemy list and removed it.
		Players.findAndRemove (inActor);
		Enemies.findAndRemove (inActor);
		
		//destroying the characters that are dead, we can use particle/extra effects later.
		//Destroy (inActor);		//obsolute - moved to per character subclass
		
	}
	
	/// <summary>
	/// Increases the exp on enemy kill.
	/// </summary>
	/// <param name="killedCharacterLevel">Killed character level.</param>
	public void increaseExpOnEnemyKill (int killedCharacterLevel)
	{
		dataAccess.info.updateXp ((uint)(killedCharacterLevel * baseXPGranted));	//with respect to enemy killed
	}
	
	/// <summary>
	/// Increases the gold on enemy kill.
	/// </summary>
	/// <param name="Amount">Amount.</param>
	public void increaseGoldOnEnemyKill (int Amount)
	{
		dataAccess.info.updateGold (Amount);	//with respect to enemy killed
	}
	
	/// <summary>
	/// Decreases the gold when all player party members die
	/// </summary>
	/// <param name="decreaseGoldOnPlayerPartyDeath">Killed character level.</param>
	public void decreaseGoldOnPlayerPartyDeath (int characterLevel)
	{
		dataAccess.info.updateGold (-characterLevel * baseGoldPanaltyOnDeath);
	}
	
	//fight sequence--------------
	
	/// <summary>
	/// Allies attack. player turn
	/// </summary>
	/// <returns>The attack.</returns>
	// On Player Turn
	IEnumerator playerTurn ()
	{
		Debug.Log ("player turn");
		numberOfRemaingPlayerCharactersThisTurn = Players.Count ();
		
		for (int i = 0; i < Players.Count(); ++i) {
			Players [i].GetComponent<FPlayer> ().hasPlayedThisTurn = false;
		}
		
		while (numberOfRemaingPlayerCharactersThisTurn > 0) {
			// by default select the first available player
			for (int i = 0; i < Players.Count(); ++i) {
				if (!Players [i].GetComponent<FPlayer> ().hasPlayedThisTurn) {
					OnSwitchPlayer (i);
					break;
				}
			}
			
			
			isPlayerTurn = true;
			currentBattleAction = BattleActions.NONE;
			
			while (currentBattleAction == BattleActions.NONE) {
				for (int i = 0; i <Players.Count(); ++i) {
					if (!Players [i].GetComponent<FPlayer> ().hasPlayedThisTurn && Players [i].GetComponent<FPlayer> ().isTouched ())
						OnSwitchPlayer (i);
				}
				yield return null;
			}
			
			switch (currentBattleAction) {
			case BattleActions.ATTACK:
				{
					int swipeResult = -1;
					while (swipeResult == -1) {
						swipeResult = swipedOnEnemy ();
						yield return null;
					}
				
					numberOfRemaingPlayerCharactersThisTurn -= 1;
					Players [currentPlayerIndex].GetComponent<FPlayer> ().hasPlayedThisTurn = true;
				
					Tweener yieldTweener;
					Players [currentPlayerIndex].GetComponent<Character> ().fight (Enemies [swipeResult], out yieldTweener, 1.0f);
					if (yieldTweener != null)
						yield return yieldTweener.WaitForCompletion ();	//watiing for the fight to finish*
				
				
				
				}
				break;
				
			case BattleActions.JOINT_ATTACK:
				break;
				
			case BattleActions.ITEM: 
				{
					numberOfRemaingPlayerCharactersThisTurn -= 1;
					Players [currentPlayerIndex].GetComponent<FPlayer> ().hasPlayedThisTurn = true;
				
					currentEnemyIndex = -1;
					if (ItemList.mAllItems [currentItemId].mIsAreaItem) {
						while (currentEnemyIndex == -1) {
							// wait for player to select target
							for (int i = 0; i <Players.Count(); ++i) {
								if (Enemies [i].GetComponent<Enemy> ().isTouched ())
									OnSwitchEnemy (i);
							}
							yield return null;
						}
					} else {
						currentEnemyIndex = 0; // This is done to avoid null reference
					}
				
					Debug.Log ("item animation start");
					yield return StartCoroutine (ItemList.mAllItems [currentItemId].mItemImplementation (Players [currentPlayerIndex], Enemies [currentEnemyIndex]));
					Debug.Log ("item animation end");
				}
				break;
				
			case BattleActions.SKILL:
				{
					numberOfRemaingPlayerCharactersThisTurn -= 1;
					Players [currentPlayerIndex].GetComponent<FPlayer> ().hasPlayedThisTurn = true;
				
					currentEnemyIndex = -1;
					if (SkillList.mAllSkills [currentSkillId].mIsAreaSpell) {
						Debug.Log ("waiting for player to select target");
						while (currentEnemyIndex == -1) {
							while (currentEnemyIndex == -1) {
								// wait for player to select emey in case of targetted attack
								for (int i = 0; i <Players.Count(); ++i) {
									if (Enemies [i].GetComponent<Enemy> ().isTouched ())
										OnSwitchEnemy (i);
								}
							
							
								yield return null;
							}
						}
					} else {
						currentEnemyIndex = 0; // This is done to avoid null reference
					}
				
					Debug.Log ("skill animation start");
					yield return StartCoroutine (SkillList.mAllSkills [currentSkillId].mSkillImplementation (Players [currentPlayerIndex], Enemies [currentEnemyIndex]));
					Debug.Log ("skill animation end");
				}
				break;
				
			case BattleActions.RETREAT:
				{
					// show a dialogue box asking for confirmation
					// on click yes isPlayerRetreating = true;
					// on click no currentBattleAction = BattleActions.NONE;
				}
				break;
			}
			
			Hud.GetInstance ().OnHideItems ();
			Hud.GetInstance ().OnHideSkills ();
			enemyFlag.SetActive (false);
			yield return null;
		}
		
		isPlayerTurn = false;
		playerFlag.SetActive (false);
		yield return null;
	}
	
	/// <summary>
	/// Enemies attack.
	/// </summary>
	/// <returns>The attack.</returns>
	IEnumerator enemyAttack ()
	{
		//enemies fight
		foreach (var enemy in Enemies) {
			
			Tweener yieldTweener; //using for yielding untill a single fighting is done.
			
			if (Players.Count <= 0) 	//break it if there is no enemies
				break;
			
			enemy.GetComponent<Character> ().fight (Players [Random.Range (0, Players.Count)], out yieldTweener, 1.0f);	//fight sequence
			
			if (yieldTweener != null) {
				yield return yieldTweener.WaitForCompletion ();	//watiing for the fight to finish
			}
			yield return null;
		}
	}
	
	
	/// <summary>
	/// On switch player.
	/// </summary>
	/// <param name="index">Index of the player.</param>
	void OnSwitchPlayer (int index)
	{
		playerFlag.SetActive (true);
		currentPlayerIndex = index;
		playerFlag.GetComponent<ActiveCharacterInfo> ().setActivateArrow (Players [currentPlayerIndex].transform);
		Debug.Log ("current player index is " + currentPlayerIndex);
	}
	
	void OnSwitchEnemy (int index)
	{
		enemyFlag.SetActive (true);
		currentEnemyIndex = index;
		enemyFlag.GetComponent<ActiveCharacterInfo> ().setActivateArrow (Enemies [currentEnemyIndex].transform);
		Debug.Log ("current enemy index is " + currentEnemyIndex);
	}
	
	/// <summary>
	/// Swipe gesture. Returns enemy index, if swiped on enemy object.
	/// </summary>
	/// <returns>enemy index.</returns>
	int swipedOnEnemy ()
	{
		
		#if UNITY_EDITOR
		if (Input.GetMouseButtonDown (0)) {
			Vector3 wp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			startPos = new Vector2 (wp.x, wp.y);
			startTime = Time.time;
			couldBeSwipe = true;
		}
		
		if (Input.GetMouseButtonUp (0) && couldBeSwipe) {
			Vector3 wp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector2 EndPos = new Vector2 (wp.x, wp.y);
			
			var swipeTime = Time.time - startTime;
			var swipeDist = (EndPos - startPos).magnitude;
			
			if (couldBeSwipe && (swipeTime < maxSwipeTime) && (swipeDist > minSwipeDist)) {
				couldBeSwipe = false;
				Debug.Log ("swipe detected");
				RaycastHit2D hit;
				
				hit = Physics2D.Linecast (startPos, EndPos);
				
				for (int i = 0; i < Enemies.Count(); ++i) {
					if (hit == null)
						return -1;
					if (hit.collider == null)
						return -1;
					if (hit.collider.gameObject == null)
						return -1;
					
					if (hit.collider.gameObject == Enemies [i].gameObject) {
						return i;		
					}
				}
			} else {
				couldBeSwipe = false;
			}
		}
		#else
		foreach (Touch touch in Input.touches)
		{
			switch (touch.phase) 
			{
			case TouchPhase.Began:  couldBeSwipe = true;
				startPos = touch.position;
				startTime = Time.time;
				break;
				
			case TouchPhase.Moved:
				if (Mathf.Abs(touch.position.y - startPos.y) > comfortZone) {
					couldBeSwipe = false;
				}
				break;
				
			case TouchPhase.Stationary:
				couldBeSwipe = false;
				break;
				
			case TouchPhase.Ended:
				var swipeTime = Time.time - startTime;
				Vector2 EndPos = touch.position;
				var swipeDist = (EndPos - startPos).magnitude;
				
				if (couldBeSwipe  && (swipeTime < maxSwipeTime) && (swipeDist > minSwipeDist)) 
				{
					// It's a swiiiiiiiiiiiipe!
					var swipeDirection = Mathf.Sign(touch.position.y - startPos.y);
					
					// Do something here in reaction to the swipe.
					RaycastHit2D hit;
					
					hit = Physics2D.Linecast(startPos, EndPos);
					
					for(int i = 0; i < Enemies.Count(); ++i)
					{
						if(hit.collider.gameObject == Enemies[i].gameObject)
						{
							return i;
							
						}
					}
				}
				break;
			}
		}
		#endif
		
		return -1;
	}
	
	//UI interaction related --------------------
	
	//attack button
	public void buttonPress_OnAttack ()
	{
		//changing the state to attack
		currentBattleAction = BattleActions.ATTACK;
	}
	
}

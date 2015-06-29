using UnityEngine;
using System.Collections;

public class FPlayer : Character
{
	[Tooltip("base gold decreases upon death")]
	public int
		baseGoldPanaltyOnDeath = 30;		//base gold decreases upon death
	
	public bool hasPlayedThisTurn = false;
	
	//handle selecting actor
	public bool isTouched ()
	{
		bool result = false;
		if (Input.touchCount == 1) {
			if (Input.touches [0].phase == TouchPhase.Ended) {
				Vector3 wp = Camera.main.ScreenToWorldPoint (Input.GetTouch (0).position);
				Vector2 touchPos = new Vector2 (wp.x, wp.y);
				if (GetComponent<Collider2D> () == Physics2D.OverlapPoint (touchPos)) {
					result = true;
				}
			}
		}
		if (Input.GetMouseButtonUp (0)) {
			Vector3 wp = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector2 mousePos = new Vector2 (wp.x, wp.y);
			if (GetComponent<Collider2D> () == Physics2D.OverlapPoint (mousePos)) {
				result = true;
			}
		}
		return result;
	}
	
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
}

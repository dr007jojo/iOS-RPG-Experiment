using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Actor : MonoBehaviour
{
	// basic attributes---------
	public float health = 100;			//current health of the player
	public float maxHealth = 100;		//max health a character can have
	//-------
	
	//status
	public Slider healthSlider;			//to reference the slider component for Health
	
	//on Start
	void Start ()
	{
		postBeginPlay ();	//initialize for subclasses at start
	}
	
	//called per render update
	void Update ()
	{
		Tick (); 		//for updating subclasses
	}


	//basic methods-----------
	
	//for subclass initialize
	virtual public void postBeginPlay ()
	{
		//implement in the subclass
	}
	
	/// <summary>
	/// Tick this instance.
	/// </summary>
	virtual public void Tick ()
	{
		//implement in the subclass
	}
	
	/// <summary>
	/// Take damage, this method will deal damage to this actor/character
	/// </summary>
	/// <param name="inDamage">In damage.</param>
	/// <param name="damageDealer">Damage dealer.</param>
	/// <param name="Instigator">Instigator (damage dealer class, only if a character).</param>
	virtual public void takeDamage (float inDamage, GameObject damageDealer, Actor instigator = null)
	{
		//basic implementation
		//further implementation in the subclass may have armor and damage reduction features
		health = Mathf.Clamp (health - inDamage, 0, maxHealth);	//clamping the resulting value to go beyond 0
		
		//update the health slider, if presence
		updateHealthSlider ();
		
		//if health is 0, calling the death
		if (health <= 0) {
			onDeath ();
		}
	}
	
	/// <summary>
	/// Do damage to the target. simplified use
	/// </summary>
	/// <param name="damage">Damage.</param>
	/// <param name="target">Damaged actor.</param>
	public void doDamage (float damage, GameObject target)
	{
		if (target.GetComponent<Actor> () == null)	//early exit if the actor component is not found (safety)
			return;
			
		target.GetComponent<Actor> ().takeDamage (damage, target, this);
	}
	
	
	/// <summary>
	/// on death. It can be overriden on the subclass for specific actions
	/// </summary>
	virtual public void onDeath ()
	{
		//logging out
		//Debug.Log ("Actor died: " + this.gameObject.ToString ());
		
		//invoking event on death
		EventManager.info.invokeEvent (ref EventManager.info.onDeath, gameObject);
	}
	
	
	//utilities---
	//health slider
	protected void updateHealthSlider ()
	{
		if (healthSlider != null) {
			healthSlider.value = health;		//slider amount to that of health
		}
	}
	
	/// <summary>
	/// enable the actors to get touch and selected
	/// </summary>
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
	
	//--?? some explaination please. <kashyap>
	virtual public void ReplyHP (float RHP, GameObject damageDealer, Actor instigator = null)
	{
		health = Mathf.Clamp (health + RHP, 0, maxHealth);	
		
		updateHealthSlider ();
	}
	
}

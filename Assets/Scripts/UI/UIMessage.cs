using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIMessage : MonoBehaviour 
{
	public Text mDescription;
	
	public void ShowWithText(string displayTextn)
	{
		mDescription.text = displayTextn;
		StartCoroutine(Disappear());
	}

	IEnumerator Disappear()
	{
		yield return new WaitForSeconds(3.0f);
		this.gameObject.SetActive(false);
	}
}

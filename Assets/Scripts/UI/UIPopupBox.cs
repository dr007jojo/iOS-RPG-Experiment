using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public delegate void ClickImplementation ();

public class UIPopupBox : MonoBehaviour {
	
	ClickImplementation mOnClickYes;
	ClickImplementation mOnClickNo;

	public Text mDescription;

	public void SetData(string displayText, ClickImplementation yesImplementation, ClickImplementation noImplementation)
	{
		mDescription.text = displayText;
		mOnClickYes = yesImplementation;
		mOnClickNo = noImplementation;
	}

	public void OnClickYes()
	{
		mOnClickYes();
	}

	public void OnClickNo()
	{
		mOnClickNo();
	}
}

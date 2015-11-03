using UnityEngine;
using System.Collections;

public class OldExitButton : StandardButton 
{
	protected override void ButtonPressedAction()
	{
		base.ButtonPressedAction();
	}
	
	protected override void ButtonReleasedAction()
	{
		Application.Quit();
		
		base.ButtonReleasedAction();
	}
}

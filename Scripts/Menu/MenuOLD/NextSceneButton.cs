using UnityEngine;
using System.Collections;

public class NextSceneButton : StandardButton
{
	public int NextSceneIndex = 3;
	
	protected override void ButtonPressedAction()
	{
		base.ButtonPressedAction();
	}
	
	protected override void ButtonReleasedAction()
	{
		MenuManager.ChangeScene(NextSceneIndex, false);
		
		base.ButtonReleasedAction();
	}
}

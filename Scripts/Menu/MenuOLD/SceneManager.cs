using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour 
{
	private delegate void ClickAction (IButton button);

	private void FindButtonComponent (Transform obj, ClickAction callback) 
	{
		foreach(var component in obj.gameObject.GetComponents<Component>()) 
		{
			IButton buttonComponent = component as IButton;
			if(buttonComponent != null) 
			{
				callback(buttonComponent);
			}
		}
	}
		
	void Update () 
    {
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit raycastHit = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit);
			if (hit)
			{
				FindButtonComponent(raycastHit.transform, delegate(IButton button) { button.ButtonPressed ();});
			}
		}
		
		if (Input.GetMouseButtonUp(0))
		{
			RaycastHit raycastHit = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit);
			if (hit)
			{
				FindButtonComponent(raycastHit.transform, delegate(IButton button) { button.ButtonReleased ();});
			}
		}
	}
}

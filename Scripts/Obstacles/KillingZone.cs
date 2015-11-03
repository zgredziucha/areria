using UnityEngine;
using System.Collections;

public class KillingZone : MonoBehaviour {

	public delegate void ChangingStateEvent (bool isPlayerColliding, KillingZone self);
	public event ChangingStateEvent onChangeState;
    private BoxCollider2D _collider;

    public void Start ()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

	public void OnChangeState(bool isPlayerColliding)
	{
		if (onChangeState != null)
		{
			onChangeState(isPlayerColliding, this);
		}
	}

	public void OnTriggerEnter2D (Collider2D other) 
	{
		if (other.CompareTag ("Player")) 
		{
			OnChangeState(isPlayerColliding : true);
			//Debug.Log("ENTER " + gameObject.name);
		}
	}

	public void OnTriggerExit2D (Collider2D other) 
	{
		if (other.CompareTag ("Player")) 
		{
			OnChangeState(isPlayerColliding : false);
			//Debug.Log("EXIT " + gameObject.name);
		}
	}

    public void SetDisabled ()
    {
        _collider.enabled = false;
        OnChangeState(isPlayerColliding: false);
    }

    public void SetEnabled ()
    {
        _collider.enabled = true;
    }
	
}

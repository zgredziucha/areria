using UnityEngine;
using System.Collections;

public class TutorialEnd : MonoBehaviour {

	public delegate void TriggerBehavior();

    public TriggerBehavior onTrigger;
    public void OnTriggerEnter2D  (Collider2D other)
    {
        if (other.CompareTag("Player") && onTrigger != null)
        {
            onTrigger();
        }
    }


}

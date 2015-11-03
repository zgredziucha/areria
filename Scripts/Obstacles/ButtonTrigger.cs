using UnityEngine;
using System.Collections;

public class ButtonTrigger : MonoBehaviour {

    public delegate void PlayerDetect ();
    public event PlayerDetect onPlayerPush;

    public void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Push();
        }
    }

    public void Push ()
    {
        //TODO: Aniamator albo particle
        if (onPlayerPush != null)
        {
            onPlayerPush();
        }
    }
}

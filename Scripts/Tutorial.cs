using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

    public string Message = "message";

    public delegate void MessageEvent(bool isMessageShown, string text);
    public static event MessageEvent onPrompt;

    public bool invokePause = true;

    public float timer = 0.0f;
    

    public void Start ()
    {
        var closingTrigger = GetComponentInChildren<TutorialEnd>();
        if (closingTrigger != null)
        {
            closingTrigger.onTrigger += delegate()
            {
                HideMessage();
                StartCoroutine("Unsubscribe");
            };
        }
    }

    private void ShowMessage ()
    {
        if (invokePause)
        {
            Pauser.Pause(true);
        }

        GetComponent<Collider2D>().enabled = false;

        if (onPrompt != null)
        {
            onPrompt(true, Message);
        }
    }

    private void HideMessage ()
    {
        if (onPrompt != null)
        {
            onPrompt(false, Message);
        }

        if (invokePause)
        {
            Pauser.Pause(false);
        }
    }

    public IEnumerator HideTiming ()
    {
        yield return new WaitForSeconds(timer);
        HideMessage();
        StartCoroutine("Unsubscribe");
    }
    
    public void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowMessage();
            if (timer > 0.0f)
            {
                StartCoroutine("HideTiming");
            }

            LevelManager.Instance.Player.SubscribeKey(KeyCode.E, delegate() {
                HideMessage();
                //ugly hack! todo: find out more why unscubscribe is unpossible right now ! 
                StartCoroutine("Unsubscribe");
                //LevelManager.Instance.Player.UnsubscribeKey(KeyCode.E);
			});

           
        }
    }

    public IEnumerator Unsubscribe ()
    {
        yield return new WaitForSeconds(0.5f);
        LevelManager.Instance.Player.UnsubscribeKey(KeyCode.E);
        Destroy(gameObject);
    }
}

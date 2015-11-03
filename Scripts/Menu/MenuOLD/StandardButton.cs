using UnityEngine;
using System.Collections;

public class StandardButton : MonoBehaviour, IButton 
{
    public Texture2D pressedTexture;
    public Texture2D releasedTexture;

    public AudioClip pressedSound;
    public AudioClip releasedSound;

    private AudioSource audioSource;

    private bool isPressed = false;
	
	protected void Start () 
	{
        if (gameObject.GetComponent<AudioSource>() != null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }    
	}

    protected virtual void ButtonPressedAction()
    {

    }

    protected virtual void ButtonReleasedAction()
    {

    }
	

	public void ButtonPressed()
	{
	    isPressed = true;
	    SetTexture();
	    PlaySound();
	}

	public void ButtonReleased()
	{
	    isPressed = false;
	    SetTexture();
	    PlaySound(); 
	}

    private void PlaySound()
    {
        if (isPressed)
        {
            if (pressedSound != null)
            {	
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(pressedSound);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(pressedSound, Camera.main.transform.position);
                }
                
				StartCoroutine(WaitForSound(pressedSound));
            }
            else
            {
				ButtonPressedAction();
            }
        }
        else if (!isPressed)
        {
            if (releasedSound != null)
            {
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(releasedSound);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(releasedSound, Camera.main.transform.position);
					
                }
                
				StartCoroutine(WaitForSound(releasedSound));
            }
            else
            {
				ButtonReleasedAction();
            }
        }

    }

    private void SetTexture()
    {
        if (isPressed)
        {
            if (pressedTexture != null)
            {
                gameObject.GetComponent<Renderer>().material.mainTexture = pressedTexture;
            }
        }
        else if (!isPressed)
        {
            if (releasedTexture != null)
            {
                gameObject.GetComponent<Renderer>().material.mainTexture = releasedTexture;
            }
        }
    }
    
	private IEnumerator WaitForSound(AudioClip audioClip)
	{
		yield return new WaitForSeconds(audioClip.length);
		if(isPressed)
		{
			ButtonPressedAction();
			
		}
		else if(!isPressed)
		{
			ButtonReleasedAction();
		}
		
	}
}

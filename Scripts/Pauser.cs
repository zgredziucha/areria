using UnityEngine;
using System.Collections;

public class Pauser : MonoBehaviour {
	private static bool _isPaused = false;

    public delegate void PauseEvent(bool isPaused);
    public static event PauseEvent onPause;

	void Update () {
		if(Input.GetKeyUp(KeyCode.P))
		{
			//_isPaused = !_isPaused;
            Pause(!_isPaused, triggerEvent: true);

		}
	}

    public static void Pause (bool doPause, bool triggerEvent = false)
    {
        _isPaused = doPause;
        if (_isPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        
        if (onPause != null && triggerEvent)
        {
            onPause(_isPaused);
        }
    }
}


using UnityEngine;
using System.Collections;

public class ButtonObserver : Observer {

    public ButtonTrigger button;
    public SpriteRenderer buttonLight;

    public void OnEnable()
    {
        button.onPlayerPush += Broadcast;
        base.OnEnable();
    }

    public void OnDisable()
    {
        button.onPlayerPush -= Broadcast;
        base.OnDisable();
    }

    public override void StopWorking()
    {
        base.StopWorking();
        ChangeEnable(false);
    }

    private void ChangeEnable (bool isEnabled)
    {
        if (buttonLight != null)
        {
            buttonLight.enabled = isEnabled;
        }

        button.enabled = isEnabled;
    }

    public override void StartWorking()
    {
        base.StartWorking();
        ChangeEnable(true);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_PauseController : UI_PromptWindow {


    public void OnEnable ()
    {
        Pauser.onPause += ChangeActivity;
    }

    public void OnDisable ()
    {
        Pauser.onPause -= ChangeActivity;
    }
}

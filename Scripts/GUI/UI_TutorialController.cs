using UnityEngine;
using System.Collections;

public class UI_TutorialController : UI_PromptWindow {

    public void OnEnable()
    {
        Tutorial.onPrompt += ChangeActivity;
    }

    public void OnDisable()
    {
        Tutorial.onPrompt -= ChangeActivity;
    }


    public  void ChangeActivity(bool isActive, string text)
    {
        SetText(text);
        base.ChangeActivity(isActive);
    }

    public void SetText (string text)
    {
        texts[0].text = text;
    }
}

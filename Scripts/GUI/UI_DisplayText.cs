using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_DisplayText : MonoBehaviour {

    private Text textRenderer;

    public void Start()
    {
        textRenderer = GetComponent<Text>();
    }

    public void OnEnable()
    {
        GUI_Events.promptText += DisplayText;
    }

    public void OnDisable()
    {
        GUI_Events.promptText -= DisplayText;
    }

    public void DisplayText(string text)
    {
        textRenderer.text = text;
        textRenderer.enabled = true;
    }

    public void HideText()
    {
        textRenderer.enabled = false;
    }
}

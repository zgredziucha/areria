using UnityEngine;
using System.Collections;

public class GUI_DisplayText : MonoBehaviour {

    private MeshRenderer renderer;
    private TextMesh textRenderer;

    public void Start ()
    {
        renderer = GetComponent<MeshRenderer>();
        textRenderer = GetComponent<TextMesh>();
    }

	public void OnEnable ()
    {
        GUI_Events.promptText += DisplayText;
    }

    public void OnDisable ()
    {
        GUI_Events.promptText -= DisplayText;
    }

    public void DisplayText (string text)
    {
        textRenderer.text = text;
        renderer.enabled = true;
    }

    public void HideText ()
    {
        renderer.enabled = false;
    }
}

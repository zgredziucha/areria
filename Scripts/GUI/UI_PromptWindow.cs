using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_PromptWindow : MonoBehaviour {

    protected Image[] images;
    protected Text[] texts;

    public void Start()
    {
        images = GetComponentsInChildren<Image>();
        texts = GetComponentsInChildren<Text>();
        ChangeActivity(false);
    }

    public void ChangeActivity(bool isActive)
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].enabled = isActive;
        }

        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].enabled = isActive;
        }
    }
}

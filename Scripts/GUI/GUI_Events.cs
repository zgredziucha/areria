using UnityEngine;
using System.Collections;

public class GUI_Events : MonoBehaviour {

    public delegate void DisplayTextEvent(string text);
    public static event DisplayTextEvent promptText;

    public static event DisplayTextEvent displayTutorial;

    public static void DisplayTutorial(string text)
    {
        if (displayTutorial != null)
        {
            displayTutorial(text);
        }
    }


    public static void DisplayText (string text )
    {
        if (promptText != null)
        {
            promptText(text);
        }
    }
}

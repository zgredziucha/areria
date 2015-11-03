using UnityEngine;
using System.Collections;

public class ChangeSceneButton : MonoBehaviour {

    public int NextSceneIndex = 2;

    public void ButtonPressed()
    {
        MenuManager.ChangeScene(NextSceneIndex, false);
    }
}

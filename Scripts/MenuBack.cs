using UnityEngine;
using System.Collections;

public class MenuBack : MonoBehaviour {

    public void OnButtonPressed()
    {
        MenuManager.ChangeScene(2, true);
    }
}

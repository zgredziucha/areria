using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_Collectible : MonoBehaviour {

    public Image image;
    public Text text;

    public float displayTime = 1.0f;

    private bool _isVisible = false;

	public void OnEnable ()
    {
        Collectible.onTake += Display;
        LevelManager.onPlayerDied += ForceHide;
    }

    public void OnDisable()
    {
        Collectible.onTake -= Display;
        LevelManager.onPlayerDied -= ForceHide;
    }

    private void ForceHide ()
    {
        if (_isVisible)
        {
            StopCoroutine("Displaying");
            SetVisibility(false);
        }
    }

    private void Display (int TotalCollectibles, int currentCollectibles)
    {
        if (_isVisible)
        {
            StopCoroutine("Displaying");
        }
        SetText(TotalCollectibles, currentCollectibles);
        StartCoroutine("Displaying");
       
    }

    public IEnumerator Displaying()
    {
        SetVisibility(true);
        yield return new WaitForSeconds(displayTime);
        SetVisibility(false);
    }

    private void SetVisibility (bool isVisible)
    {
        _isVisible = isVisible;
        image.enabled = isVisible;
        text.enabled = isVisible;
    }

    private void SetText (int TotalCollectibles, int currentCollectibles)
    {
        string _text = string.Format("{0} / {1}", currentCollectibles, TotalCollectibles);
        text.text = _text;
    }


}

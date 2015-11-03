using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChooseLevelButton : MonoBehaviour {
    public int sceneIndex = 3;
    public bool isUnlock = false;

    public Sprite unlockTexture;
    public Sprite lockTexture;

    private Image textureRenderer;
    private Button button;

    public void ButtonPressed()
    {
        if (isUnlock)
        {
            MenuManager.ChangeScene(sceneIndex, true);
        }
    }

    public void OnEnable()
    {
        GameManager.onLevelUnlock += OnLevelUnlock;
    }

    public void OnDisable()
    {
        GameManager.onLevelUnlock -= OnLevelUnlock;
    }

    private void OnLevelUnlock(int levelIndex)
    {
        if (levelIndex == sceneIndex)
        {
            isUnlock = true;
            RenderTexture();
        }
    }

    public void Awake()
    {
        if (GameManager.firstLevelIndex == sceneIndex)
        {
            GameManager.UnlockLevel(sceneIndex);
        }

        isUnlock = true;
            
            //= GameManager.GetLevelState(sceneIndex);
        textureRenderer = GetComponent<Image>();
        button = GetComponent<Button>();

        RenderTexture();
    }

    private void RenderTexture()
    {
        if (textureRenderer != null)
        {
            var texture = isUnlock ? unlockTexture : lockTexture;
            textureRenderer.sprite = texture;
            button.enabled = isUnlock;
        }
    }
}

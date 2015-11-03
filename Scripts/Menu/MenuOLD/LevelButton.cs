using UnityEngine;
using System.Collections;

public class LevelButton : StandardButton {

	public int sceneIndex = 3;
	public bool isUnlock = false;

	public Sprite unlockTexture;
	public Sprite lockTexture;

	public SpriteRenderer textureRenderer;

	protected override void ButtonPressedAction()
	{
		base.ButtonPressedAction();
	}
	
	protected override void ButtonReleasedAction()
	{
		if (isUnlock) 
		{
			MenuManager.ChangeScene(sceneIndex, true);
		}
		
		base.ButtonReleasedAction();
	}

//	public void Start ()
//	{
//		isUnlock = GameManager.Instance.GetLevelState (sceneName);
//		RenderTexture ();
//		base.Start ();
//	}
	public void OnEnable () 
	{
		GameManager.onLevelUnlock += OnLevelUnlock;
	}

	public void OnDisable ()
	{
		GameManager.onLevelUnlock -= OnLevelUnlock;
	}
	
	private void OnLevelUnlock (int levelIndex)
	{
		if ( levelIndex == sceneIndex)
		{
			isUnlock = true;
			RenderTexture ();
		}
	}

	public void Awake ()
	{
		isUnlock = GameManager.GetLevelState (sceneIndex);
		RenderTexture ();
	}

	private void RenderTexture () 
	{
		if (textureRenderer != null)
		{
			var texture = isUnlock ? unlockTexture : lockTexture;
			textureRenderer.sprite = texture;
		}
	}

}

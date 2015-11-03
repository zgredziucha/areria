using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MenuManager : MonoBehaviour 
{
	public AudioSource menuMusic;
	
	public delegate void SceneChangeEventHandler (int sceneIndex, bool isExitingMenu);
	
	public static event SceneChangeEventHandler changeScene;
	public static event SceneChangeEventHandler sceneWasChanged;
	
	public static void ChangeScene(int sceneIndex, bool isExitingMenu)
	{
		if (changeScene != null)
		{
			changeScene(sceneIndex, isExitingMenu);
		}
	}
	
	public static void SceneWasChanged(int sceneIndex, bool isExitingMenu)
	{
		if (sceneWasChanged != null)
		{
			sceneWasChanged(sceneIndex, isExitingMenu);
		}
	}
	
	void Start ()
	{
		menuMusic = GetComponent<AudioSource>();
		changeScene += SwitchScene;
		sceneWasChanged += OnChangeScene;
	}
	
	void OnDestroy ()
	{
		changeScene -= SwitchScene;
		sceneWasChanged -= OnChangeScene;
	}
	
	void Awake () 
	{
		DontDestroyOnLoad(this);
	}
	
	public void OnChangeScene(int levelIndex, bool isExitingMenu)
	{
		if(isExitingMenu)
		{
			menuMusic.Stop();
		}
		else
		{
			if(!menuMusic.isPlaying)
			{
				menuMusic.Play();
			}
		}
	}
	
	public void SwitchScene (int levelIndex, bool isExitingMenu)
	{
		Debug.Log (levelIndex);
		if(isExitingMenu)
		{
			menuMusic.Stop();
		}
		else
		{
			if(!menuMusic.isPlaying)
			{
				menuMusic.Play();
			}
		}
		
		Application.LoadLevel (levelIndex);
	}
	
}

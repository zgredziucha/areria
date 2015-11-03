using UnityEngine;
using System.Collections;

public class LoadingScene : MonoBehaviour {

	public int levelIndex = 3;
	public float timer = 1;
	private float tempTimer = 0;

	AsyncOperation async;

	public GameObject fadeTexture;
	public float fadeSpeed = 3.8f;
	private float alpha = 0.0f; 
	private float fadeDir = -1;
	
	private void FadeScreen () 
	{
		alpha -= fadeDir * fadeSpeed * Time.deltaTime;  
		alpha = Mathf.Clamp01(alpha);   
		fadeTexture.GetComponent<Renderer>().material.color = new Color(0, 0, 0, alpha);
	}

	void Start () {
		tempTimer = timer;
		//StartCoroutine("Load");
	}

	private void SwitchScene()
	{
		if (async != null) 
		{
			async.allowSceneActivation = true;
			MenuManager.SceneWasChanged(levelIndex, true);	
		}
	}

	void Update () {
		tempTimer -= Time.deltaTime;
		if (tempTimer < 0) 
		{
			tempTimer = timer;
			SwitchScene();
			FadeScreen();
			fadeTexture.GetComponent<Renderer>().enabled = true;

		}
	}

	IEnumerator Load () {
		async = Application.LoadLevelAsync(levelIndex);
		async.allowSceneActivation = false;
		yield return async;
	}


}

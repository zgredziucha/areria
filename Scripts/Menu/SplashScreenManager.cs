using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

public class SplashScreenManager : MonoBehaviour 
{
	public int mainMenuIndex = 1;
	
	public float splashTimer = 3;
	
	private float startTime = 0;

	private bool _isSceneSwitched = false;
	void Start ()
	{
		startTime = Time.time;
		
//		OptionsData.GetPresets();
//        OptionsData.ApplyData();
	}

	void Update () 
	{
		if(!_isSceneSwitched && Time.time - startTime > splashTimer)
		{
			_isSceneSwitched = true;
			MenuManager.ChangeScene(mainMenuIndex, false);
		}
	}
}

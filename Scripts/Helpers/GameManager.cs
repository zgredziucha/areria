using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LevelState
{
	Lock = 1,
	Unlock = 2
}
public class GameManager : MonoBehaviour {
	public static int unlockLevels = 0;
	public static int firstLevelIndex = 3;
	public static int lastLevelIndex = 9;
	public static int ChoosePanelIndex = 2;

	private static Hashtable levelsState = new Hashtable();

	public delegate void UnlockEvent (int levelIndex);
	public static event UnlockEvent onLevelUnlock;
	
	public static void OnLevelUnlock(int levelIndex)
	{
		if (onLevelUnlock != null)
		{
			onLevelUnlock(levelIndex);
		}
	}

	public static bool GetLevelState (int levelIndex)
	{
		if (levelsState.ContainsKey(levelIndex))
		{
			return (LevelState)levelsState[levelIndex] == LevelState.Unlock ? true : false;
		}
		return false;
	}
	
	public static  void UnlockLevel (int levelIndex)
	{
		if (levelsState.ContainsKey(levelIndex) && (LevelState)levelsState[levelIndex] == LevelState.Lock)
		{
			levelsState[levelIndex] = LevelState.Unlock;
			OnLevelUnlock(levelIndex);
			unlockLevels++;
			SaveLevelsData (levelIndex);
		}
	}

	void Awake () 
	{
		DontDestroyOnLoad(this);
		InitializeLevelsData ();
	}

	private static void InitializeLevelsData ()
	{
		for (int i = firstLevelIndex; i <= lastLevelIndex; i++)
		{
			if (!levelsState.ContainsKey(i))
			{
				levelsState.Add(i, LevelState.Lock);
			}
		}

		LoadLevelsData ();
		UnlockLevel (firstLevelIndex);

	}

	public static  void SaveLevelsData (int levelIndex)
	{
		PlayerPrefs.SetInt ("Levels", unlockLevels);
		PlayerPrefs.SetInt (levelIndex.ToString(), (int)levelsState [levelIndex]);
	}

	public static void LoadLevelsData ()
	{
		PlayerPrefs.GetInt ("Levels", unlockLevels);
		for (int i = firstLevelIndex; i <= lastLevelIndex; i++)
		{
			int nameIndex = i;
			int state = 0;
			PlayerPrefs.GetInt(nameIndex.ToString(), state);
			if (state == 0)
			{
				PlayerPrefs.SetInt (nameIndex.ToString(), (int)levelsState [nameIndex]);
			}
			else 
			{
				UnlockLevel(nameIndex);
			}
		}
	}

	public static int GetNextLevelIndex (int currentLevel)
	{
		if (currentLevel >= firstLevelIndex && currentLevel <= lastLevelIndex)
		{
			UnlockLevel (currentLevel++);
			return currentLevel++;
		}
		else 
		{
			return ChoosePanelIndex;
		}

	}
}

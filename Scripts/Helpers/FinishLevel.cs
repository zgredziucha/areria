using UnityEngine;
using System.Collections;

public class FinishLevel : MonoBehaviour {

	public void OnTriggerEnter2D (Collider2D other)
	{
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.GotoNextLevel();
		}
	}
}

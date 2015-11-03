using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour {

    public static int Count { get; private set; }

    public delegate void TakeEvent(int totalCount, int currentCount );
    public static event TakeEvent onTake;

    void Start () {
        Count++;
        Debug.Log(Count);
	}
	
	public void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.Player.Collectibles++;
            if (onTake != null)
            {
                onTake(Count, LevelManager.Instance.Player.Collectibles);
            }
            Destroy(gameObject);
        }
    }
}

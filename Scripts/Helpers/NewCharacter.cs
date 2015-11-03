using UnityEngine;
using System.Collections;

public class NewCharacter : MonoBehaviour {

    public CharacterName name;
    public GameObject spawnEffect;

    public void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.Characters.SetSwitchPossibility(true);
            LevelManager.Instance.Characters.SetCharacterByName(name);
           
            if (spawnEffect != null)
            {
                GameObject _particle = Instantiate(spawnEffect, other.transform.position, Quaternion.identity) as GameObject;
                _particle.transform.parent = other.transform;
            }
           
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}

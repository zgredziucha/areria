using UnityEngine;
using System.Collections;

public class Swicher : MonoBehaviour {

    public float energy = 0;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.Characters.currentSwitchEnergyCost = energy;
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var characters = LevelManager.Instance.Characters;
            characters.currentSwitchEnergyCost = characters.SwitchEnergyCost;
        }
    }
}

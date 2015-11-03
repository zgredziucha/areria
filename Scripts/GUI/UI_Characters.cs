using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI_Characters : MonoBehaviour {

    //ćma = 0
    //modliszka = 1
    //public List<Image> CharacterImages = new List<Image>();

    public List<RectTransform> CharacterRenderers = new List<RectTransform>();
    public RectTransform Frame;

    private Vector2 firstRendererAnchorMinMax = new Vector2 (0.95f, 0.5f);
    private Vector2 firstRendererAnchoredPosition = new Vector2(-33.0f, -2.9f);
    private Vector2 offset = new Vector2 (.3f, .0f);

    public Vector2 currentImageSize;
    public Vector2 normalImageSize;

    private int currentCharacterID = 0;

	public void OnEnable ()
    {
        CharactersController.onSwitch += OnSwitchCharacter;
        CharactersController.onLockCharacters += OnCharactersLock;

    }

    public void OnDisable()
    {
        CharactersController.onSwitch -= OnSwitchCharacter;
        CharactersController.onLockCharacters -= OnCharactersLock;
    }
    //public void Start ()
    //{
    //    Frame.anchoredPosition = new Vector2(-30.0f, -7.0f);
    //    Debug.Log(Frame.anchoredPosition);
    //}

    public void OnCharactersLock (Character currentCharacter, bool areMultipleCharacters)
    {
        
        for (int i = 0; i < CharacterRenderers.Count; i ++)
        {
            if (i == currentCharacterID)
            {
                CharacterRenderers[i].anchorMax = firstRendererAnchorMinMax;
                CharacterRenderers[i].anchorMin = firstRendererAnchorMinMax;
                OnSwitchCharacter(currentCharacter);
                continue;
            }

            if (areMultipleCharacters)
            {
                CharacterRenderers[i].anchoredPosition = firstRendererAnchoredPosition;
                CharacterRenderers[i].anchorMax = firstRendererAnchorMinMax - offset;
                CharacterRenderers[i].anchorMin = firstRendererAnchorMinMax - offset;
            }
            else
            {
                CharacterRenderers[i].anchoredPosition = new Vector2(202.0f, firstRendererAnchoredPosition.y);
            }
        }
    }

    public void OnSwitchCharacter(Character character)
    {
        currentCharacterID = character.ID;

        Frame.anchoredPosition = CharacterRenderers[currentCharacterID].anchoredPosition;
        Frame.anchorMax = CharacterRenderers[currentCharacterID].anchorMax;
        Frame.anchorMin = CharacterRenderers[currentCharacterID].anchorMin;
    }
}

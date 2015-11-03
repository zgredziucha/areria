﻿using UnityEngine;
using System.Collections;

public class FloatingText : MonoBehaviour {

    //private TextMesh _content;
    private static readonly GUISkin Skin = Resources.Load<GUISkin>("GameSkin");
    
    public GUIStyle Style { get; set; }
    public string Text { get { return _content.text; } set { _content.text = value; } }

    private GUIContent _content;
    private IFloatingTextPositioner _positioner;

     public static FloatingText Show (string text, string style, IFloatingTextPositioner positioner)
    {
        var go = new GameObject("Floating Text");
        var floatingText = go.AddComponent<FloatingText>();
        floatingText.Style = Skin.GetStyle(style);
        floatingText._positioner = positioner;
        floatingText._content = new GUIContent(text);
        return floatingText;
    }

    public void OnGUI()
    {
        var position = new Vector2();
        var contentSize = Style.CalcSize(_content);
        if (!_positioner.GetPosition(ref position, _content, contentSize))
        {
            Destroy(gameObject);
            return;
        }

        GUI.Label(new Rect(position.x, position.y, contentSize.x, contentSize.y), _content, Style);
    }


}

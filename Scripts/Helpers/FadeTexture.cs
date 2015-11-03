using UnityEngine;
using System.Collections;

public class FadeTexture : MonoBehaviour {

	private float _destinationAlpha = 255;
	public float _currentAlpha = 255;

	private float max = 255;
	private float min = 255;

	public SpriteRenderer _spriteRenderer;

	public void Start ()
	{
		_spriteRenderer = GetComponent<SpriteRenderer> ();
		_currentAlpha = _spriteRenderer.color.a;
		_destinationAlpha = _currentAlpha;
		max = _currentAlpha;
		min = _currentAlpha;
	}

	public void FadeIn()
	{
		_spriteRenderer.enabled = true;
	}
	
	public void FadeOut()
	{
		_spriteRenderer.enabled = false;
	}

//	public void FadeIn()
//	{
//		_destinationAlpha = 1;
//		max = Mathf.Max (_currentAlpha, _destinationAlpha);
//		min = Mathf.Min (_currentAlpha, _destinationAlpha);
//	}
//	
//	public void FadeOut()
//	{
//		_destinationAlpha = 0;
//		max = Mathf.Max (_currentAlpha, _destinationAlpha);
//		min = Mathf.Min (_currentAlpha, _destinationAlpha);
//	}
//
//	public float duration = 5.0f;
//	public void Update ()
//	{
//		if ( _destinationAlpha != _currentAlpha)
//		{
////			_currentAlpha = Mathf.Lerp(min,  max, Time.deltaTime * 0.02f);
////			_spriteRenderer.color =  new Color(255f, 255f, 255f, _currentAlpha);
//			_currentAlpha = Mathf.SmoothStep(min, max, Time.deltaTime / duration);
//			_spriteRenderer.color =  new Color(1f, 1f, 1f, _currentAlpha);
//
//		}
//	}
}

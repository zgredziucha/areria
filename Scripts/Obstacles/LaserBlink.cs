using UnityEngine;
using System.Collections;

public class LaserBlink : MonoBehaviour {

//	public LaserBeam laserBeam;
	public bool isLaserVisibleOnStart = true;
	private bool _isLaserVisible = true;
	private SpriteRenderer _spriteRenderer;
	private BoxCollider2D _boxCollider;

	protected void ChangeLaserApperance ()
	{
		if (_spriteRenderer != null && _boxCollider != null) 
		{
			_isLaserVisible = !_isLaserVisible;
			_spriteRenderer.enabled = _isLaserVisible;
			_boxCollider.enabled = _isLaserVisible;
		}

	}
	
	void Start () {
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_boxCollider = GetComponent<BoxCollider2D>();
		_isLaserVisible = isLaserVisibleOnStart;
		ChangeLaserApperance ();
	}

	public void TurnOff () 
	{
		_spriteRenderer.enabled = false;
		_boxCollider.enabled = false;
	}

	public void TurnOn () 
	{
		_spriteRenderer.enabled = true;
		_boxCollider.enabled = true;
	}


}

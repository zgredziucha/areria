using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private bool _isFacingRight;
	private GroundMovement _controller;
	private float _normalizedHorizontalSpeed;

	public float MaxSpeed = 8;
	public float SpeedAccelerationOnGround = 10f;
	public float SpeedAccelerationInAir = 5f;

	public void Start () 
	{
		_controller = GetComponent<GroundMovement> ();
		_isFacingRight = transform.localScale.x > 0;


	}

	public void Update () {
		HandleInput ();


		var movementFactor = _controller.State.IsGrounded ? SpeedAccelerationOnGround : SpeedAccelerationInAir;
		//lerp between current velocity and proper direction multiplied by maxspeed)
		// _normalizedHorizontalSpeed - direction the player will face (0 for no changes, 1 if should turn right, -1 if should turn left)
		// if max speed equal 0 only turn movement, esle turn + movement
		_controller.SetHorizontalForce (Mathf.Lerp(_controller.Velocity.x, _normalizedHorizontalSpeed * MaxSpeed, Time.deltaTime * movementFactor));
	}

	private void HandleInput () {
		if (Input.GetKey (KeyCode.D))
		{
			_normalizedHorizontalSpeed = 1;
			if (!_isFacingRight)
			{
				Flip();
			}
		} 
		else if (Input.GetKey (KeyCode.A)) 
		{
			_normalizedHorizontalSpeed = -1;
			if (_isFacingRight)
			{
				Flip();
			}
		}
		else 
		{
			_normalizedHorizontalSpeed = 0;
		}

		if (_controller.CanJump && Input.GetKeyDown(KeyCode.Space)) 
		{
			_controller.Jump();
		}
	}

	private void Flip () 
	{
		//if change local scale in x dimension to opposite negative number it will flip sprite vertically
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		// if local scale in x dimension is greater than 0 i facing right, else it facing left
		_isFacingRight = transform.localScale.x > 0;
	}

}

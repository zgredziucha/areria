using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum VectorDirection
{
	Invalid,
	Up,
	Down,
	Right,
	Left,
	None
}

public class PlayerController : MonoBehaviour {

	public CharactersController characterController;

	public delegate void MovingInputEvent (VectorDirection direction);
	public static event MovingInputEvent onPlayerMovingInput;

	public delegate void ForceMovingEvent (Vector2 movingVector);
	public static event ForceMovingEvent onForceMovement;

	//public bool IsDead { get; private set; }
	public bool IsRespawning { get; private set; }
	
	public delegate void KeyAction ();
	private Dictionary<KeyCode, KeyAction> _subscribedKeys = new Dictionary<KeyCode, KeyAction>();

    public int Collectibles { get; set; }
    public int Deaths { get; set; }

    public static void OnMovingInput(VectorDirection direction)
	{
		if (onPlayerMovingInput != null)
		{
			onPlayerMovingInput(direction);
		}
	}

	public void Start ()
	{
		//IsDead = false;
		IsRespawning = false;
	}

	public void Update () {
		if (!IsRespawning)
		{
			HandleMovingInput ();
		}
	}

	public void ForceMovement (Vector3 movementVector) 
	{
		if ( onForceMovement != null)
		{
			onForceMovement(movementVector);
		}
	}
	
	private void HandleMovingInput () {
		VectorDirection[] directions = new VectorDirection[2];
		directions[0] = VectorDirection.None; 
		var index = 0;

		if (Input.GetKey (KeyCode.D))
		{
			directions[index] = VectorDirection.Right;
			index ++;
		} 
		else if (Input.GetKey (KeyCode.A)) 
		{
			directions[index] = VectorDirection.Left;
			index ++;
		}

		if (Input.GetKey (KeyCode.W))
		{
			directions[index] = VectorDirection.Up;
			index ++;
		} 
		else if (Input.GetKey (KeyCode.S)) 
		{
			directions[index] = VectorDirection.Down;
			index ++;
		}

		for (var i = 0; i < directions.Length; i++) 
		{
			if (directions[i] != VectorDirection.Invalid)
			{
				OnMovingInput(directions[i]);
			}
		}

		foreach(KeyValuePair<KeyCode, KeyAction> subscribedKey in _subscribedKeys )
		{
			if (Input.GetKeyUp (subscribedKey.Key))
			{
				subscribedKey.Value();
			} 
		}
	}

	public void SubscribeKey (KeyCode keyCode, KeyAction keyAction) 
	{
		if (!_subscribedKeys.ContainsKey (keyCode)) 
		{
			_subscribedKeys.Add (keyCode, keyAction);
		} 
		else 
		{
			_subscribedKeys [keyCode] = keyAction;
		}
	}

	public void UnsubscribeKey (KeyCode keyCode) 
	{
		if (_subscribedKeys.ContainsKey (keyCode)) 
		{
			_subscribedKeys.Remove(keyCode);
		} 
	}

	public void TurnOff()
	{
		IsRespawning = true;
		characterController.TurnOff ();
		//collider2D.enabled = false;
	}

	public void TurnOn()
	{
		IsRespawning = false;
		characterController.TurnOn ();
		//collider2D.enabled = true;
	}

	public void RespawnAt(Vector3 spawnPoint, float rotation)
	{
		transform.position = spawnPoint;
		transform.rotation = Quaternion.Euler(0, 0, 0);// rotation);
	}

	public void FinishLevel ()
	{
        TurnOff();
//		TODO: make it work! 

//		enabled = false;
//		Flying.enabled = false;
//		collider2D.enabled = false;
	}
}

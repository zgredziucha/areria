using UnityEngine;
using System.Collections;

public enum MoveMode
{
	Flying,
	Snapping,
	UnSnapping,
	Walking, 
    Falling,
    Standing
}

public class MovingState {

	public MoveMode MovingMode{ get; set; }

	public bool IsCollidingRight { get; set; }
	public bool IsCollidingLeft { get; set; }
	public bool IsCollidingAbove { get; set; }
	public bool IsCollidingBelow { get; set; }
	public bool IsMovingDownSlope { get; set; }
	public bool IsMovingUpSlope { get; set; }
	
	public bool IsGrounded { get{ return IsCollidingBelow; }}
	public float SlopeAngle { get; set; }
	public float DestinationAngle { get; set; }

	public Vector3 SlopeShiftMovement { get; set; }
	
	public bool HasCollisions { get { return IsCollidingLeft || IsCollidingRight || IsCollidingAbove || IsCollidingBelow;} }
	
	public void ResetCollisionsState() 
	{
		IsMovingDownSlope =
			IsMovingUpSlope = 
			IsCollidingLeft =
			IsCollidingRight = 
			IsCollidingBelow =
				IsCollidingAbove = false;
			
		
		SlopeAngle = 0;
	}

}

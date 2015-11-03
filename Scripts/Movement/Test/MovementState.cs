using UnityEngine;
using System.Collections;

public class MovementState 
{
	public bool IsCollidingRight { get; set; }
	public bool IsCollidingLeft { get; set; }
	public bool IsCollidingAbove { get; set; }
	public bool IsCollidingBelow { get; set; }
	public bool IsMovingDownSlope { get; set; }
	public bool IsMovingUpSlope { get; set; }

	public bool IsGrounded { get{ return IsCollidingBelow; }}
	public float SlopeAngle { get; set; }

	public bool HasCollisions { get { return IsCollidingLeft || IsCollidingRight || IsCollidingAbove || IsCollidingBelow;} }

	public void Reset() 
	{
		IsMovingDownSlope =
		IsMovingUpSlope = 
		IsCollidingLeft =
		IsCollidingRight = 
		IsCollidingBelow =
		IsCollidingAbove = false;

		SlopeAngle = 0;
	}

	public override string ToString()
	{
		return string.Format (
			"(cotroller: r:{0}, l:{1}, a:{2}, b:{3}, down-slope:{4}, up-slope:{5}, angle:{6})",
			IsCollidingRight,
			IsCollidingLeft,
			IsCollidingAbove,
			IsCollidingBelow,
			IsMovingDownSlope,
			IsMovingDownSlope,
			SlopeAngle
		);
	}
}

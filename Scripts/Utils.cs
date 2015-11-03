using UnityEngine;
using System.Collections;

public class Utils {

	public static float Cos(float angle)
	{
		angle = Mathf.Round (angle);
		angle = Convert360 (angle);
		if (angle == 0 || angle == 360) 
		{
			return 1;
		}
		else if (angle == 90.0f) 
		{
			return 0;
		}
		else if (angle == 180.0f) 
		{
			return -1;
		}
		else if (angle == 270.0f) 
		{
			return 0;
		}
		else 
		{
			return Mathf.Cos(angle * Mathf.Deg2Rad);
		}
	}

	public static float Convert360 (float angle)
	{
		if (angle < 0)
		{
			return 360 + angle;
		} 
		else if (angle >= 360) 
		{
			return angle - 360;
		}
		else 
		{
			return angle;
		}
	}

	public static float Round90 (float angle)
	{
		return Convert360 (Mathf.Round (angle / 90) * 90);
	}

	public static float Sin(float angle)
	{
		angle = Mathf.Round (angle);
		angle = Convert360 (angle);
		if (angle == 0 || angle == 360) 
		{
			return 0;
		}
		else if (angle == 90.0f) 
		{
			return 1;
		}
		else if (angle == 180.0f) 
		{
			return 0;
		}
		else if (angle == 270.0f) 
		{
			return -1;
		}
		else 
		{
			return Mathf.Cos(angle * Mathf.Deg2Rad);
		}
	}

}

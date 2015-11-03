using UnityEngine;
using System.Collections;

public class BackgroundScroll : MonoBehaviour 
{

	public float offsetSpeedX = 0.0f;
	public float offsetSpeedY = 0.0f;
	
	private static float tmpOffsetX = 0.0f;
	private static float tmpOffsetY = 0.0f;


	public float TmpOffsetX
	{
		get { return tmpOffsetX; }
		set { tmpOffsetX = value; } 
	}
	
	public float TmpOffsetY
	{
		get { return tmpOffsetY; }
		set { tmpOffsetY = value; } 
	}
	
	private Mesh mesh;
	
	void Start () 
	{
		CheckSpeeds(offsetSpeedX);
		CheckSpeeds(offsetSpeedY);
	}
	
	void Update () 
	{
		tmpOffsetX += offsetSpeedX * Time.deltaTime;
		tmpOffsetY += offsetSpeedY * Time.deltaTime;
		
		//renderer.material.SetTextureOffset("_MainTex", new Vector2(-tmpOffsetX, -tmpOffsetY));
		GetComponent<Renderer>().material.mainTextureOffset = new Vector2(-tmpOffsetX, -tmpOffsetY);
	}
	
	private void CheckSpeeds(float speed)
	{
		if(speed > 1.0f)
		{
			speed = 1.0f;
		}
		
		if(speed < 0.0f)
		{
			speed = 0.0f;
		}
	}
}

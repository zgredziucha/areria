using UnityEngine;
using System.Collections;

public class LayerProperties : MonoBehaviour {
	
	public string sortingLayerName = "Background";
	public int sortingOrder = 1; 

	void Start () 
	{
		GetComponent<Renderer>().sortingLayerName = sortingLayerName;
		GetComponent<Renderer>().sortingOrder = sortingOrder;
	}
}

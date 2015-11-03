using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalTimer : MonoBehaviour {

    public delegate void Callback ();
    public Dictionary<float, Callback> Timers = new Dictionary<float, Callback>();
	
	// Update is called once per frame
	void Update () {
	
	}
}

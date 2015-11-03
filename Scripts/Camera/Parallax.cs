using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

	public Transform[] layers;
	public float parallaxScale;
	public float parallaxReductionFactor;
	public float smoothing;

	private Vector3 _lastPosition;

	void Start () {
		_lastPosition = transform.position;
	}

	void Update () {
		var parallax = (_lastPosition.x - transform.position.x) * parallaxScale;

		for (var i = 0; i < layers.Length; i++)
		{
			var layerTargetPosition = layers[i].position.x + parallax * (i * parallaxReductionFactor + 1);
			layers[i].position = Vector3.Lerp(
				layers[i].position, 
				new Vector3(layerTargetPosition, layers[i].position.y, layers[i].position.z),
				smoothing * Time.deltaTime);
		}

		_lastPosition = transform.position;
	}
}

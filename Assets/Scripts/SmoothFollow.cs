using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {

	public Transform target;
	public bool followPosition = false;
	public bool followRotation = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 rel = transform.position - target.position;

		if(followPosition)
			transform.position -= rel * Time.deltaTime;

		if (followRotation)
		{
			Quaternion rot = transform.rotation * Quaternion.Inverse(target.rotation);
			transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rel.magnitude * Time.deltaTime);
		}
	}
}

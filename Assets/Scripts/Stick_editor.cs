using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class Stick_editor : MonoBehaviour  {
	Stick stickScript;
	Vector3 prevPos;

	// Use this for initialization
	void Start () {
		stickScript = GetComponent<Stick>();
		prevPos = transform.position;
		stickScript.x = (int)transform.position.x;
		stickScript.z = (int)transform.position.z;
 
		
	}
	
	// Update is called once per frame
	void Update()
	{
		if (stickScript && prevPos != transform.position)
		{
			stickScript.x = (int)transform.position.x;
			stickScript.z = (int)transform.position.z;
			prevPos = transform.position;
			//gameObject
		}
	}
}

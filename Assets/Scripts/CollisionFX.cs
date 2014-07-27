using UnityEngine;
using System.Collections;

public class CollisionFX : MonoBehaviour {

	public GameObject dust;

	void OnCollisionEnter(Collision col)
	{
		if (col.relativeVelocity.magnitude > 7)
		{
			GameObject g = (GameObject)Instantiate(dust, transform.position, transform.rotation*Quaternion.Euler(90,0,0));
			g.particleSystem.startSpeed = col.relativeVelocity.magnitude;
			g.particleSystem.Emit((int)Mathf.Pow(col.relativeVelocity.magnitude-7,4));
		}
	}
}

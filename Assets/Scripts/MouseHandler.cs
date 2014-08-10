using UnityEngine;
using System.Collections;

public class MouseHandler : MonoBehaviour
{
	// common globals
	float pressTime_l = 0;
	//float pressTime_r = 0;
	public float clickThreshold = 1;
	GameState gs;

	// prefabs
	GameObject field;

	// spherical coordinates
	float phi, psy = 0.7f, r = GameState.sz + 3;
	float r_last = GameState.sz + 3;

	// Use this for initialization
	void Start()
	{
		field = GameObject.Find("Field");
		transform.position = field.transform.position + new Vector3(r * Mathf.Cos(phi) * Mathf.Cos(psy), r * Mathf.Sin(psy), r * Mathf.Sin(phi) * Mathf.Cos(psy));
		transform.LookAt(field.transform.position);
		gs = field.GetComponent<GameState>();
	}

	// Update is called once per frame
	void Update()
	{
		r = Mathf.Clamp(r + Input.GetAxis("Mouse ScrollWheel"), 4, 25);

		if (r_last != r)
		{
			transform.position = field.transform.position + new Vector3(r * Mathf.Cos(phi) * Mathf.Cos(psy), r * Mathf.Sin(psy), r * Mathf.Sin(phi) * Mathf.Cos(psy));
			r_last = r;
		}

		if (Input.GetMouseButton(0))
		{
			pressTime_l += Time.deltaTime;
			if (pressTime_l > clickThreshold)
			{
				phi += Input.GetAxis("Mouse X");
				psy = Mathf.Clamp(psy + Input.GetAxis("Mouse Y"), 0, 1);
				transform.position = field.transform.position + new Vector3(r * Mathf.Cos(phi) * Mathf.Cos(psy), r * Mathf.Sin(psy), r * Mathf.Sin(phi) * Mathf.Cos(psy));
				transform.LookAt(field.transform.position);

			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			if (pressTime_l < clickThreshold)
			{
				Ray ray = camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 150, ~0))
					if (hit.collider && hit.collider.name.StartsWith("s_"))
					{
						Stick stickHit = hit.collider.gameObject.GetComponent<Stick>();
						gs.Add(stickHit.x, stickHit.height + 1, stickHit.z);
						//stickHit.Add(GameState.PlayerColour.Black);
					}
			}
			pressTime_l = 0;
		}

	}

}

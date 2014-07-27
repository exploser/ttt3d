using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stick : MonoBehaviour {

	public int x, z;
	public int height
	{
		get
		{
			return chips.Count;
		}
	}


	public List<GameState.PlayerColour> chips;

	void Start () {
		x = (int)transform.position.x;
		z = (int)transform.position.z;
		gameObject.name = "s_" + x + "" + z;
	}

	[RPC]
	public void ChangeState(int x, int y, int z, int cc)
	{
		if (!Network.isServer)
			return;
		GameState.state[x, y, z] = (GameState.PlayerColour)cc;
		GameState.Check();
		Debug.Log("RPC CALLED");
	}

	[RPC]
	public void CallCheck()
	{
		GameState.Check();
	}
	
	[RPC]
	public int Add(GameState.PlayerColour cc)
	{
		if (chips.Count < GameState.sz)
		{
			if (!Network.isServer)
				return 0;
			chips.Add(cc);
			
			GameObject tmp = (GameObject)Network.Instantiate(FindObjectOfType<GameState>().blackPrefab, transform.position + Vector3.up * 4, Quaternion.identity, 0);
			tmp.transform.parent = transform;
			
		}
		return chips.Count;
	}
	 
}

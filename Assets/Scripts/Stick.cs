using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stick : MonoBehaviour {

	public int x, z;
	int height;


	public List<GameState.PlayerColour> chips;

	void Start () {
	
	}
	
	void Update () {
	
	}

	public int Add(GameState.PlayerColour cc)
	{
		if (chips.Count < GameState.sz)
		{
			GameState.state[x-1, chips.Count, z-1] = cc;
			chips.Add(cc);
			GameObject tmp = (GameObject)Instantiate(FindObjectOfType<GameState>().blackPrefab, transform.position + Vector3.up * 5, Quaternion.identity);
			tmp.transform.parent = transform;
			GameState.Check();
		}
		if (chips.Count == GameState.sz)
		{
			if (chips.TrueForAll(x => x == GameState.PlayerColour.Black))
				GameState.Win(GameState.PlayerColour.Black);
			if (chips.TrueForAll(x => x == GameState.PlayerColour.White))
				GameState.Win(GameState.PlayerColour.White);
		}
		return chips.Count;
	}
}

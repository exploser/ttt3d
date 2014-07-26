using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
	List<Rect> rects;
	Rect tmprect;
	public GameObject stick_prefab;
	public GameObject blackPrefab;
	public GameObject whitePrefab;
	public static string winner = "nobody";
	public static int sz = 3;

	public enum PlayerColour
	{
		None,
		White,
		Black
	};

	public static PlayerColour[, ,] state = new PlayerColour[sz, sz, sz];

	// Use this for initialization
	void Start()
	{
		stick_prefab.transform.localScale = new Vector3(0.1f, (sz + 1) * 0.2f, 0.1f);
		tmprect = new Rect(Screen.width - 150, 20, 130, 90);
		state.Initialize();
		for (int x = 1; x <= sz; x++)
			for (int z = 1; z <= sz; z++)
				Instantiate(stick_prefab, new Vector3(x, 1+(sz-4)*0.2f, z), Quaternion.identity);
		transform.position = new Vector3((sz + 1) / 2f, 0, (sz + 1) / 2f);
		transform.localScale = new Vector3(sz + 2, 0.1f, sz + 2);
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnGUI()
	{
		GUI.Box(new Rect(10, 10, 130, 230), "State 1");
		GUI.RepeatButton(new Rect(20, 20, 30, 30), "0");
		GUI.RepeatButton(new Rect(60, 20, 30, 30), "0");
		GUI.RepeatButton(new Rect(100, 20, 30, 30), "0");
		GUI.RepeatButton(new Rect(140, 20, 30, 30), "0");
		GUI.RepeatButton(new Rect(20, 60, 30, 30), "0");
		GUI.RepeatButton(new Rect(60, 60, 30, 30), "0");
		GUI.RepeatButton(new Rect(100, 60, 30, 30), "0");
		GUI.RepeatButton(new Rect(140, 60, 30, 30), "0");
		GUI.RepeatButton(new Rect(20, 100, 30, 30), "0");
		GUI.RepeatButton(new Rect(60, 100, 30, 30), "0");
		GUI.RepeatButton(new Rect(100, 100, 30, 30), "0");
		GUI.RepeatButton(new Rect(140, 100, 30, 30), "0");
		GUI.RepeatButton(new Rect(20, 140, 30, 30), "0");
		GUI.RepeatButton(new Rect(60, 140, 30, 30), "0");
		GUI.RepeatButton(new Rect(100, 140, 30, 30), "0");
		GUI.RepeatButton(new Rect(140, 140, 30, 30), "0");
		tmprect = GUI.Window(0, tmprect, wndfunc, "WINNER");
	}

	private void wndfunc(int id)
	{
		GUI.Label(new Rect(50, 50, 200, 50), winner);
		GUI.DragWindow();
	}

	public static void Win(PlayerColour winnr)
	{
		Debug.Log(winnr.ToString() + " won!"); // the kostyl'
		winner = winnr.ToString();
	}

	internal static void Check()
	{
		// vertical axes are checked in Stick.cs directly

		for (int x = 0; x < sz; x++) // straight horizontal lookat z
			for (int y = 0; y < sz; y++)
				if (state[x, y, 0] != PlayerColour.None)
					for (int t = 1; t < sz; t++)
					{
						if (state[x, y, 0] != state[x, y, t])
							break;
						if (t == sz - 1)
							Win(state[x, 0, 0]);
					}

		for (int z = 0; z < sz; z++) // straight horizontal lookat x
			for (int y = 0; y < sz; y++)
				if (state[0, y, z] != PlayerColour.None)
					for (int t = 1; t < sz; t++)
					{
						if (state[0, y, z] != state[t, y, z])
							continue;
						if (t == sz - 1)
							Win(state[0, 0, z]);
					}
		for (int y = 0; y < sz; y++) // main diagonals
		{
			if (state[0, y, 0] != PlayerColour.None)
				for (int t = 1; t < sz; t++)
				{
					if (state[0, y, 0] != state[t, y, t])
						break;
					if (t == sz - 1)
						Win(state[0, y, 0]);
				}

			if (state[sz-1, y, 0] != PlayerColour.None)
				for (int t = 1; t < sz; t++)
				{
					if (state[sz - 1, y, 0] != state[sz - 1 - t, y, 1])
						break;
					if (t == sz - 1)
						Win(state[sz-1, y, 0]);
				}
		}
		for (int x = 0; x < sz; x++) // diagonal horizontals lookat z
		{
			if (state[x, 0, 0] != PlayerColour.None)
				for (int t = 1; t < sz; t++)
				{
					if (state[x, 0, 0] != state[x, t, t])
						break;
					if (t == sz - 1)
						Win(state[x, 0, 0]);
				}
			if (state[x, sz-1, 0] != PlayerColour.None)
				for (int t = 1; t < sz; t++)
				{
					if (state[x, sz - 1, 0] != state[x, sz - 1 - t, t])
						break;
					if (t == sz - 1)
						Win(state[x, sz-1, 0]);
				}
		}
		for (int z = 0; z < sz; z++) // diagonal horizontals lookat x
		{
			if (state[0, 0, z] != PlayerColour.None)
				for (int t = 1; t < sz; t++)
				{
					if (state[0, 0, z] != state[t, t, z])
						break;
					if (t == sz - 1)
						Win(state[0, 0, z]);
				}
			if (state[0, sz-1, z] != PlayerColour.None)
				for (int t = 1; t < sz; t++)
				{
					if (state[0, sz - 1, z] != state[t, sz - 1 - t, z])
						break;
					if (t == sz - 1)
						Win(state[0, sz-1, z]);
				}
		}

	}
}

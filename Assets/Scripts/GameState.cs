using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
	// common globals
	public static int sz = 4;
	Rect tmprect;
	const float MAGIC = 1.41421f; // sqrt(2)
	public string connectionIP = "127.0.0.1";
	public int connectionPort = 25555;

	public enum PlayerColour
	{
		None,
		White,
		Black
	};

	// colours
	public static PlayerColour winner = PlayerColour.None;
	PlayerColour ownColour = PlayerColour.None;
	PlayerColour turn = PlayerColour.White;
	public PlayerColour[, ,] state = new PlayerColour[sz, sz, sz];
	public static Dictionary<NetworkPlayer,PlayerColour> players = new Dictionary<NetworkPlayer,PlayerColour>();

	// prefabs
	public GameObject stick_prefab;
	public GameObject blackPrefab;
	public GameObject whitePrefab;

	void Start()
	{
		stick_prefab.transform.localScale = new Vector3(0.1f, (sz + 1) * 0.2f, 0.1f);
		tmprect = new Rect(Screen.width - 150, 20, 130, 90);
		state.Initialize();
		players.Clear();
		for (int x = 1; x <= sz; x++)
			for (int z = 1; z <= sz; z++)
				Instantiate(stick_prefab, new Vector3(x, 1+(sz-4)*0.2f, z), Quaternion.identity);
		transform.position = new Vector3((sz + 1) / 2f, 0, (sz + 1) / 2f);
		transform.localScale = new Vector3(MAGIC*sz, 0.1f, MAGIC*sz);

		MasterServer.ipAddress = "5.175.147.48";
		MasterServer.port = 23466;
	}

	void OnGUI()
	{
		GUI.Box(new Rect(10,10,200,100),"State");
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			GUI.Label(new Rect(20, 30, 200, 20), "Status: Disconnected");
			connectionIP = GUI.TextArea(new Rect(20, 50, 120, 20), "127.0.0.1");
			if (GUI.Button(new Rect(20, 70, 120, 20), "Client Connect"))
			{
				Debug.Log(Network.Connect(connectionIP, connectionPort));
			}
			if (GUI.Button(new Rect(20, 90, 120, 20), "Initialize Server"))
			{
				Network.InitializeServer(32, connectionPort, false);
				MasterServer.RegisterHost("ttt3d", "awpfdjwopfj");
			}
		}
		else if (Network.peerType == NetworkPeerType.Connecting)
		{
			GUI.Label(new Rect(20, 30, 200, 20), "Status: Connecting...");
		}
		else if (Network.peerType == NetworkPeerType.Client)
		{
			GUI.Label(new Rect(20, 30, 200, 20), "Status: Connected to " + Network.connections[0].ipAddress);
		}
		else if (Network.peerType == NetworkPeerType.Server)
		{
			GUI.Label(new Rect(20, 30, 200, 20), "Status: Serving on " + Network.player.externalIP);
		}
		tmprect = GUI.Window(0, tmprect, wndfunc, "WINNER");
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		if (!Network.isServer)
			return;
		PlayerColour clr = (ownColour == PlayerColour.Black ? PlayerColour.White : PlayerColour.Black);
		players.Add(player, clr);
		networkView.RPC("SetMyColour", player, (int)clr);
	}

	void OnPlayerDisconnected()
	{
		Debug.Log("LOST PLAYER OMG NO");
	}

	void OnServerInitialized()
	{
		//yield Application.LoadLevel("main");
		float f = Random.value;
		players.Add(networkView.owner, (f > .5f ? PlayerColour.Black : PlayerColour.White));
		ownColour = players[networkView.owner];
		Debug.Log(f);
		Debug.Log(Time.time + " : " + GameState.players.Count);
	}

	private void wndfunc(int id)
	{
		GUI.Label(new Rect(50, 50, 200, 50), winner.ToString());
		GUI.DragWindow();
	}

	[RPC]
	void Win(int winnr) // deprecated;
	{
		winner = (PlayerColour)winnr;
		turn = PlayerColour.None;
	}

	[RPC]
	void SetMyColour(int clr)
	{
		ownColour = (PlayerColour)clr;
	}

	[RPC]
	public void ChangeState(int x, int y, int z, NetworkPlayer player)
	{
		if (!Network.isServer)
			return;
		if (x > sz || y > sz || z > sz)
			return;

		PlayerColour tmp = players[player];
		Debug.Log(tmp);
		Debug.Log(turn);

		if (tmp != turn)
			return;

		state[x - 1, y - 1, z - 1] = tmp;
		//GameObject tmp = (GameObject)Network.Instantiate(FindObjectOfType<GameState>().blackPrefab, new Vector3(x,y+4,z), Quaternion.identity, 0);
		Stick s = GameObject.Find("s_" + x + "" + z).GetComponent<Stick>();
		s.Add(tmp);
		Check();
		if (turn == PlayerColour.White)
			turn = PlayerColour.Black;
		else if (turn == PlayerColour.Black)
			turn = PlayerColour.White;
	}



	public bool Add(int x, int y, int z)
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
			return false;

		networkView.RPC("ChangeState", RPCMode.All, x, y, z, Network.player);

		return true;
	}

	internal void Check()
	{
		// vertical axes are checked in Stick.cs directly // TODO: not anymore, add them here

		for (int x = 0; x < sz; x++) // straight horizontal lookat z
			for (int y = 0; y < sz; y++)
				if (state[x, y, 0] != PlayerColour.None)
					for (int t = 1; t < sz; t++)
					{
						if (state[x, y, 0] != state[x, y, t])
							break;
						if (t == sz - 1)
							networkView.RPC("Win", RPCMode.All, (int)state[x, 0, 0]);
					}

		for (int z = 0; z < sz; z++) // straight horizontal lookat x
			for (int y = 0; y < sz; y++)
				if (state[0, y, z] != PlayerColour.None)
					for (int t = 1; t < sz; t++)
					{
						if (state[0, y, z] != state[t, y, z])
							break;
						if (t == sz - 1)
							networkView.RPC("Win", RPCMode.All, (int)state[0, 0, z]);
					}
		for (int y = 0; y < sz; y++) // main diagonals
		{
			if (state[0, y, 0] != PlayerColour.None)
				for (int t = 1; t < sz; t++)
				{
					if (state[0, y, 0] != state[t, y, t])
						break;
					if (t == sz - 1)
						networkView.RPC("Win", RPCMode.All, (int)state[0, y, 0]);
				}

			if (state[sz-1, y, 0] != PlayerColour.None)
				for (int t = 1; t < sz; t++)
				{
					if (state[sz - 1, y, 0] != state[sz - 1 - t, y, t])
						break;
					if (t == sz - 1)
						networkView.RPC("Win", RPCMode.All, (int)state[sz - 1, y, 0]);
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
						networkView.RPC("Win", RPCMode.All, (int)state[x, 0, 0]);
				}
			if (state[x, sz-1, 0] != PlayerColour.None)
				for (int t = 1; t < sz; t++)
				{
					if (state[x, sz - 1, 0] != state[x, sz - 1 - t, t])
						break;
					if (t == sz - 1)
						networkView.RPC("Win", RPCMode.All, (int)state[x, sz - 1, 0]);
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
						networkView.RPC("Win", RPCMode.All, (int)state[0, 0, z]);
				}
			if (state[0, sz-1, z] != PlayerColour.None)
				for (int t = 1; t < sz; t++)
				{
					if (state[0, sz - 1, z] != state[t, sz - 1 - t, z])
						break;
					if (t == sz - 1)
						networkView.RPC("Win", RPCMode.All, (int)state[0, sz - 1, z]);
				}
		}

	}

	internal GameObject GetPrefab(PlayerColour cc)
	{
		if (cc == PlayerColour.Black)
			return blackPrefab;
		if (cc == PlayerColour.White)
			return whitePrefab;
		return null;
	}
}

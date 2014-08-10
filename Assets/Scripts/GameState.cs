using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
	// common globals
	public static int sz = 4;
	const float MAGIC = 1.41421f; // sqrt(2)

	public enum PlayerColour
	{
		None,
		White,
		Black
	};

	// colours
	public static PlayerColour winner = PlayerColour.None;
	public static PlayerColour ownColour = PlayerColour.None;
	PlayerColour turn = PlayerColour.White;
	public PlayerColour[, ,] state = new PlayerColour[sz, sz, sz];
	public static Dictionary<NetworkPlayer,PlayerColour> players = new Dictionary<NetworkPlayer,PlayerColour>();

	public enum GState
	{
		Idle,
		Playing,
		Waiting,
		Finished
	}

	GState gState = GState.Idle;

	// prefabs
	public GameObject stick_prefab;
	public GameObject blackPrefab;
	public GameObject whitePrefab;

	void Start()
	{
		stick_prefab.transform.localScale = new Vector3(0.1f, (sz + 1) * 0.2f, 0.1f);
		state.Initialize();
		//players.Clear(); // FUCK YOU I HATE YOU YOU ARE THE REASON OF MY HEADACHE HOW DID YOU EVEN GET HERE?
		for (int x = 1; x <= sz; x++)
			for (int z = 1; z <= sz; z++)
				Instantiate(stick_prefab, new Vector3(x, 1+(sz-4)*0.2f, z), Quaternion.identity);
		transform.position = new Vector3((sz + 1) / 2f, 0, (sz + 1) / 2f);
		transform.localScale = new Vector3(MAGIC*sz, 0.1f, MAGIC*sz);

		MasterServer.ipAddress = Networking.masterIP;
		MasterServer.port = Networking.masterPort;
	}

	void OnServerInitialized()
	{
		Application.LoadLevel("main");
		float f = Random.value;
		players.Add(networkView.owner, (f > .5f ? PlayerColour.Black : PlayerColour.White));
		ownColour = players[networkView.owner];
		gState = GState.Waiting;
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		if (!Network.isServer || gState != GState.Waiting)
			return;
		PlayerColour clr = (ownColour == PlayerColour.Black ? PlayerColour.White : PlayerColour.Black);
		players.Add(player, clr);
		networkView.RPC("SetMyColour", player, (int)clr);
		gState = GState.Playing;
	}

	void OnPlayerDisconnected(NetworkPlayer player)
	{
		gState = GState.Waiting;
		players.Remove(player);
		Debug.Log("LOST PLAYER OMG NO");
	}

	void OnApplicationQuit()
	{
		MasterServer.UnregisterHost();
	}

	[RPC]
	void Win(int winnr) // deprecated;
	{
		winner = (PlayerColour)winnr;
		turn = PlayerColour.None;
		gState = GState.Finished;
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

		if (tmp != turn)
			return;

		state[x - 1, y - 1, z - 1] = tmp;
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
		for (int x = 0; x < sz; x++) // vertical
			for (int z = 0; z < sz; z++)
				if (state[x, 0, z] != PlayerColour.None)
					for (int t = 1; t < sz; t++)
					{
						if (state[x, t, z] != state[x, t, z])
							break;
						if (t == sz - 1)
							networkView.RPC("Win", RPCMode.All, (int)state[x, 0, z]);
					}

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

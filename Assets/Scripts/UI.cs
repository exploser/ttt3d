using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour {

	//GameState gs;
	//Rect tmprect;
	public Texture tx;

	void Start()
	{
		//tmprect = new Rect(Screen.width - 150, 20, 130, 90);
		//GameObject field = GameObject.Find("Field");
		//gs = field.GetComponent<GameState>();
	}

	void OnGUI()
	{
		
		GUILayout.BeginArea(new Rect(10, 10, 130, 320));
		GUILayout.BeginVertical("box");
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			GUILayout.Label("Status: Disconnected");
			Networking.connectionIP = GUILayout.TextArea(Networking.connectionIP);
			if (GUILayout.Button("Client Connect"))
			{
				Network.Connect(Networking.connectionIP, Networking.connectionPort);
			}
			if (GUILayout.Button("Initialize Server"))
			{
				Network.InitializeServer(32, Networking.connectionPort, false);
				MasterServer.RegisterHost("ttt3d", "awpfdjwopfj");
			}
		}
		else if (Network.peerType == NetworkPeerType.Connecting)
		{
			GUILayout.Label("Status: Connecting...");
		}
		else if (Network.peerType == NetworkPeerType.Client)
		{
			GUILayout.Label("Status: Connected to " + Network.connections[0].ipAddress);
			if(GUILayout.Button("Disconnect"))
				Network.Disconnect();
		}
		else if (Network.peerType == NetworkPeerType.Server)
		{
			GUILayout.Label("Status: Serving on " + Network.player.externalIP); 
			if (GUILayout.Button("Disconnect"))
				Network.Disconnect();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();

		/****************/

		GUILayout.BeginArea(new Rect(Screen.width / 2 - 100, Screen.height - 50, 200, 60));
		GUILayout.BeginHorizontal("box");
		GUILayout.Box(new GUIContent(tx), GUILayout.Height(40), GUILayout.Width(40));
		GUILayout.Label("0");
		GUILayout.EndHorizontal();
		GUILayout.EndArea();

	}

	private void wndfunc(int id)
	{
		GUI.Label(new Rect(50, 50, 200, 50), GameState.winner.ToString());
		GUI.DragWindow();
	}
}

using UnityEngine;
using System.Collections;

public class Networking : MonoBehaviour {


	public static string connectionIP = "127.0.0.1";
	public static short connectionPort = 25555;
	public static string masterIP = "5.175.147.48";
	public static short masterPort = 23466;

	public enum ServerState
	{
		Idle,
		Playing,
		Waiting,
		Finished
	}

}

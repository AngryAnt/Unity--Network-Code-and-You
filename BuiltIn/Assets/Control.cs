using UnityEngine;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;


public class Control : MonoBehaviour
{
	public enum GameState
	{
		Offline,
		PreGame,
		Game,
		PostGame
	};


	public const string kServerArgument = "-server";
	const int kPort = 424242;


	static Control instance;


	GameState state = GameState.Offline;
	IPAddress ip;
	string message = "Awake";


	static Control Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (Control)FindObjectOfType (typeof (Control));
			}

			return instance;
		}
	}


	public static GameState State
	{
		get
		{
			return Instance.state;
		}
		set
		{
			Instance.state = value;
		}
	}


	void Start ()
	{
		Application.RegisterLogCallback (OnLog);

		bool isServer = false;

		foreach (string argument in System.Environment.GetCommandLineArgs ())
		{
			if (argument == kServerArgument)
			{
				isServer = true;
				break;
			}
		}

		if (isServer)
		{
			Network.InitializeSecurity ();
			Network.InitializeServer (10, kPort, false);
		}
		else
		{
			Network.Connect (IP.ToString (), kPort);
		}
	}


	void OnServerInitialized ()
	{
		Debug.Log ("Server initialized");
		gameObject.SendMessage ("OnServerStart");
	}


	void OnConnectedToServer ()
	{
		Debug.Log ("Connected to server");
		gameObject.SendMessage ("OnClientStart");
	}


	public IPAddress IP
	{
		get
		{
			if (ip == null)
			{
				ip = (
					from entry in Dns.GetHostEntry (Dns.GetHostName ()).AddressList
						where entry.AddressFamily == AddressFamily.InterNetwork
							select entry
				).FirstOrDefault ();
			}

			return ip;
		}
	}


	void OnGUI ()
	{
		GUILayout.Label ("State: " + state);
		GUILayout.Label (message);
	}


	void OnLog (string message, string callStack, LogType type)
	{
		this.message = message;
	}
}

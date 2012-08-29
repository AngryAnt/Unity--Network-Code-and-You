using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;


public class Control : MonoBehaviour
{
	public const string kServerArgument = "-server";


	const int
		kPort = 42209,
		kHostConnectionBacklog = 10;


	static Control instance;


	string message = "Awake";
	Socket socket;
	IPAddress ip;


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


	public static Socket Socket
	{
		get
		{
			return Instance.socket;
		}
	}


	void Start ()
	{
		Application.RegisterLogCallbackThreaded (OnLog);

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
			if (Host (kPort))
			{
				gameObject.SendMessage ("OnServerStarted");
			}
		}
		else
		{
			if (Connect (IP, kPort))
			{
				gameObject.SendMessage ("OnClientStarted", socket);
			}
		}
	}


	void OnApplicationQuit ()
	{
		Disconnect ();
	}


	public bool Host (int port)
	{
		Debug.Log ("Hosting on port " + port);

		socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		try
		{
			socket.Bind (new IPEndPoint (IP, port));
			socket.Listen (kHostConnectionBacklog);
			socket.BeginAccept (new System.AsyncCallback (OnClientConnect), socket);
		}
		catch (System.Exception e)
		{
			Debug.LogError ("Exception when attempting to host (" + port + "): " + e);

			socket = null;

			return false;
		}

		return true;
	}


	public bool Connect (IPAddress ip, int port)
	{
		Debug.Log ("Connecting to " + ip + " on port " + port);

		socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		socket.Connect (new IPEndPoint (ip, port));

		if (!socket.Connected)
		{
			Debug.LogError ("Failed to connect to " + ip + " on port " + port);

			socket = null;
			return false;
		}

		return true;
	}


	void Disconnect ()
	{
		if (socket != null)
		{
			socket.BeginDisconnect (false, new System.AsyncCallback (OnEndHostComplete), socket);
		}
	}


	void OnClientConnect (System.IAsyncResult result)
	{
		Debug.Log ("Handling client connecting");

		try
		{
			gameObject.SendMessage ("OnClientConnected", socket.EndAccept (result));
		}
		catch (System.Exception e)
		{
			Debug.LogError ("Exception when accepting incoming connection: " + e);
		}

		try
		{
			socket.BeginAccept (new System.AsyncCallback (OnClientConnect), socket);
		}
		catch (System.Exception e)
		{
			Debug.LogError ("Exception when starting new accept process: " + e);
		}
	}


	void OnEndHostComplete (System.IAsyncResult result)
	{
		socket = null;
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
		GUILayout.Label ("");
		GUILayout.Label (message);
	}


	void OnLog (string message, string callStack, LogType type)
	{
		this.message = message + "\n" + this.message;
	}
}

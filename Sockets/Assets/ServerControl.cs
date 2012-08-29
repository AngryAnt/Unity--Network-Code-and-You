using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;


public class ServerControl : MonoBehaviour
{
	List<Socket> clients = new List<Socket> ();


	void OnServerStarted ()
	{
		Debug.Log ("Server started");
	}


	void OnClientConnected (Socket client)
	{
		Debug.Log ("Client connected");
		clients.Add (client);
		SocketRead.Begin (client, OnReceive, OnReceiveError);
	}


	void OnReceive (SocketRead read, byte[] data)
	{
		string message = "Client " + clients.IndexOf (read.Socket) + " says: " + Encoding.ASCII.GetString (data, 0, data.Length);
		Debug.Log (message);

		foreach (Socket client in clients)
		{
			if (client == read.Socket)
			{
				continue;
			}

			client.Send (Encoding.ASCII.GetBytes (message));
		}
	}


	void OnReceiveError (SocketRead read, System.Exception exception)
	{
		Debug.LogError ("Receive error: " + exception);
	}
}

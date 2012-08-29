using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;


public class ClientControl : MonoBehaviour
{
	const int kInputWidth = 100;


	string input = "";
	bool send = false;
	Socket socket;


	void OnClientStarted (Socket socket)
	{
		Debug.Log ("Client started");

		this.socket = socket;
		SocketRead.Begin (socket, OnReceive, OnError);
	}


	void OnReceive (SocketRead read, byte[] data)
	{
		Debug.Log (Encoding.ASCII.GetString (data, 0, data.Length));
	}


	void OnError (SocketRead read, System.Exception exception)
	{
		Debug.LogError ("Receive error: " + exception);
	}


	void OnGUI ()
	{
		if (socket == null)
		{
			return;
		}

		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
		{
			Event.current.Use ();

			send = true;
		}

		input = GUILayout.TextArea (input, GUILayout.MinWidth (kInputWidth), GUILayout.ExpandWidth (true));

		if (send && Event.current.type == EventType.Repaint)
		{
			send = false;

			input = input.Trim ();
			input = input.Substring (0, input.Length < SocketRead.kBufferSize ? input.Length : SocketRead.kBufferSize);
			socket.Send (Encoding.ASCII.GetBytes (input));
			Debug.Log ("You said: " + input);
			input = "";
		}
	}
}

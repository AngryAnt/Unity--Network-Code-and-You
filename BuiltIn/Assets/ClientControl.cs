using UnityEngine;
using System.Collections;


[RequireComponent (typeof (NetworkView))]
public class ClientControl : MonoBehaviour
{
	void OnClientStart ()
	{
		Debug.Log ("Client started");

		Control.State = Control.GameState.PreGame;

		Debug.Log ("Requesting to join the game");
		networkView.RPC ("OnClientRequestJoin", RPCMode.Server);
	}


	[RPC]
	void OnJoinAccepted ()
	{
		Debug.Log ("Server granted join access");

		Control.State = Control.GameState.Game;
	}


	[RPC]
	void OnPostGame ()
	{
		Debug.Log ("Server transitioned to post game mode");

		Control.State = Control.GameState.PostGame;
	}
}

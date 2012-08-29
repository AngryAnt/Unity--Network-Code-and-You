using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent (typeof (NetworkView))]
public class ServerControl : MonoBehaviour
{
	public Player playerPrefab;
	public Vector2 spawnArea = new Vector2 (8.0f, 8.0f);


	List<NetworkPlayer>
		joinQueue = new List<NetworkPlayer> (),
		players = new List<NetworkPlayer> ();


	IEnumerator OnServerStart ()
	{
		Debug.Log ("Server started");

		Control.State = Control.GameState.PreGame;

		do
		{
			while (Control.State == Control.GameState.PreGame)
			{
				yield return null;
			}

			if (Control.State == Control.GameState.Game)
			{
				foreach (NetworkPlayer player in joinQueue)
				{
					AcceptPlayer (player);
				}

				joinQueue.Clear ();
			}

			while (Control.State == Control.GameState.Game)
			{
				yield return null;
			}

			if (Control.State == Control.GameState.PostGame)
			{
				foreach (NetworkPlayer player in players)
				{
					networkView.RPC ("OnPostGame", player);
				}

				foreach (Player player in FindObjectsOfType (typeof (Player)))
				{
					Network.RemoveRPCs (player.networkView.viewID);
					Network.Destroy (player.networkView.viewID);
				}

				players.Clear (); // Could add tracking of these post-game players
			}

			while (Control.State == Control.GameState.PostGame)
			{
				yield return null;
			}
		}
		while (enabled);
	}


	void OnPlayerDisconnected (NetworkPlayer player)
	{
		players.Remove (player);
		Network.RemoveRPCs (player);
		Network.DestroyPlayerObjects (player);

		foreach (Player playerInstance in FindObjectsOfType (typeof (Player)))
		{
			if (playerInstance.Owner == player)
			{
				Network.RemoveRPCs (playerInstance.networkView.viewID);
				Network.Destroy (playerInstance.networkView.viewID);
			}
		}
	}


	void AcceptPlayer (NetworkPlayer player)
	{
		players.Add (player);
		networkView.RPC ("OnJoinAccepted", player);

		Vector3 position = RandomPosition;
		Player playerInstance = (Player)Network.Instantiate (playerPrefab, position, Quaternion.LookRotation (transform.position - position, Vector3.up), 0);
		playerInstance.Spawn (player);
	}


	[RPC]
	void OnClientRequestJoin (NetworkMessageInfo messageInfo)
	{
		if (Control.State == Control.GameState.Game)
		{
			Debug.Log ("Player requested to join. Accepting.");
			AcceptPlayer (messageInfo.sender);
		}
		else
		{
			Debug.Log ("Player requested to join. Adding to queue.");
			joinQueue.Add (messageInfo.sender);
		}
	}


	Vector3 RandomPosition
	{
		get
		{
			return transform.position +
				transform.right * (Random.Range (0.0f, spawnArea.x) - spawnArea.x * 0.5f) +
				transform.forward * (Random.Range (0.0f, spawnArea.y) - spawnArea.y * 0.5f);
		}
	}


	void OnDrawGizmos ()
	{
		Gizmos.color = new Color (0.2f, 0.5f, 0.5f, 0.5f);
		Gizmos.DrawCube (transform.position, new Vector3 (spawnArea.x, 0.1f, spawnArea.y));
	}
}

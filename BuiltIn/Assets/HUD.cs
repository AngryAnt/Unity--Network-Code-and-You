using UnityEngine;
using System.Collections;


public class HUD : MonoBehaviour
{
	const float kWidth = 400.0f, kHeight = 50.0f;


	void OnGUI ()
	{
		GUILayout.BeginArea (
			new Rect ((Screen.width - kWidth) * 0.5f, Screen.height - kHeight, kWidth, kHeight),
			GUI.skin.box
		);
			if (Network.isServer)
			{
				GUILayout.BeginHorizontal ();
					GUILayout.Box ("State");
					GUILayout.Toggle (Control.State == Control.GameState.Offline, "Offline");
					Control.State = GUILayout.Toggle (Control.State == Control.GameState.PreGame, "Pre-game") ? Control.GameState.PreGame : Control.State;
					Control.State = GUILayout.Toggle (Control.State == Control.GameState.Game, "Game") ? Control.GameState.Game : Control.State;
					Control.State = GUILayout.Toggle (Control.State == Control.GameState.PostGame, "Post-game") ? Control.GameState.PostGame : Control.State;
				GUILayout.EndHorizontal ();
			}
			else
			{
				switch (Control.State)
				{
					case Control.GameState.PreGame:
						GUILayout.Label ("Waiting for game to start...");
					break;
					case Control.GameState.Game:
						GUILayout.Label ("Use WASD keys to move");
					break;
					case Control.GameState.PostGame:
						GUILayout.BeginHorizontal ();
							GUILayout.Label ("Game over");
							if (GUILayout.Button ("Re-join"))
							{
								gameObject.SendMessage ("OnClientStart");
							}
						GUILayout.EndHorizontal ();
					break;
				}
			}
		GUILayout.EndArea ();
	}
}

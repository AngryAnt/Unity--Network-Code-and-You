using UnityEngine;
using System.Collections;


public class Control : MonoBehaviour
{
	public TextAsset initScript, uploadPageSource;


	string message = "Awake";
	bool callbackFrameLoaded = false;
	WWW www;
	Texture2D texture;


	void Start ()
	{
		message = "Start";
		Application.ExternalEval (initScript.text);
	}


	void ShowFrame (bool visible)
	{
		Application.ExternalEval (
			"self.parent.document.getElementById (\"BaseFrameset\").rows=\"" +
				(visible ? 50 : 0) +
				",*\";"
		);
	}


	void ShowFrame ()
	{
		ShowFrame (true);
	}


	void HideFrame ()
	{
		ShowFrame (false);
	}


	void ClearFrame ()
	{
		Application.ExternalEval ("self.parent.frames[\"CallbackFrame\"].document.location.href = 'about:blank'");
	}


	void WriteFrame (string contents)
	{
		Application.ExternalEval (
			"self.parent.frames[\"CallbackFrame\"].document.write (\"" +
				contents.Replace ("\n", "\\n").Replace ("\r", "").Replace ("\"", "\\\"") +
				"\");"
		);
	}


	void OnLoaded ()
	{
		message = "OnLoaded";
		callbackFrameLoaded = true;
	}


	void OnUploadDisplay ()
	{
		message = "OnUploadDisplay";
		ShowFrame ();
	}


	void OnSubmit ()
	{
		message = "OnSubmit";
		HideFrame ();
	}


	IEnumerator OnUploaded (string content)
	{
		message = "OnUploaded";
		www = new WWW (content.Substring (0, content.IndexOf ("<script>")).Trim ());
		yield return www;
		texture = www.texture;
	}


	void OnGUI ()
	{
		GUILayout.Label (message);

		bool enabled = GUI.enabled;
		GUI.enabled = callbackFrameLoaded && enabled;

		if (GUILayout.Button ("Upload"))
		{
			ClearFrame ();
			WriteFrame (uploadPageSource.text);
		}

		GUILayout.Label (texture);

		GUI.enabled = enabled;
	}
}

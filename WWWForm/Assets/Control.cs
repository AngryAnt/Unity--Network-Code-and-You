using UnityEngine;
using System.Collections;
using System.Reflection;


public class Control : MonoBehaviour
{
	Assembly assembly;
	string message = "Awake";


	void Awake ()
	{
		Application.RegisterLogCallback (OnLog);
	}


	IEnumerator LoadAssembly ()
	{
		Debug.Log ("Loading assembly bytes");

		WWW www = new WWW ("Assembly.dll");
		yield return www;

		Debug.Log ("Loading assembly from bytes");

		assembly = Assembly.Load (www.bytes);

		Debug.Log ("Assembly loaded");
	}


	IEnumerator UploadScreenshot ()
	{
		if (assembly == null)
		{
			Debug.LogError ("No assembly set");
			yield break;
		}

		Debug.Log ("Asking assembly to upload screenshot");

		WWW www = (WWW)assembly.GetType ("MyAssembly").GetMethod ("UploadScreenshot").Invoke (null, null);

		yield return www;

		Debug.Log ("Assembly uploaded the screenshot to " + www.text);
	}


	void OnGUI ()
	{
		GUILayout.Label (message);

		if (assembly == null)
		{
			if (GUILayout.Button ("Load"))
			{
				StartCoroutine (LoadAssembly ());
			}
		}
		else
		{
			if (GUILayout.Button ("Post screenshot"))
			{
				StartCoroutine (UploadScreenshot ());
			}
		}
	}


	void OnLog (string message, string callStack, LogType type)
	{
		this.message = message;
	}
}

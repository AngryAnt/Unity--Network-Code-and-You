using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Diagnostics;
using Debug = UnityEngine.Debug;


public class NetworkEnvironment : MonoBehaviour
{
	const string
		kServerBinaryName = "Server",
		kClientBinaryName = "Client";


	[MenuItem ("Assets/Build and run/Server %#&s")]
	static void BuildAndRunServer ()
	{
		Build (kServerBinaryName);
		Run (kServerBinaryName, Control.kServerArgument).Exited += OnServerTerminated;
	}


	[MenuItem ("Assets/Build and run/Client %#&c")]
	static void BuildAndRunClient ()
	{
		Build (kClientBinaryName);
		Run (kClientBinaryName);
	}


	static void Build (string binaryName)
	{
		BuildPipeline.BuildPlayer (EditorBuildSettings.scenes.Select (scene => scene.path).ToArray (), GetBuildPath (binaryName), Target, BuildOptions.AllowDebugging);
	}


	static Process Run (string binaryName, string arguments = "")
	{
		return Process.Start (
			new ProcessStartInfo ()
			{
				FileName = GetBinaryPath (binaryName),
				Arguments = arguments
			}
		);
	}


	static void OnServerTerminated (object sender, System.EventArgs arguments)
	{
		Debug.Log ("Server application has terminated");
	}


	static BuildTarget Target
	{
		get
		{
			switch (Application.platform)
			{
				case RuntimePlatform.OSXEditor:
					return BuildTarget.StandaloneOSXIntel;
				case RuntimePlatform.WindowsEditor:
					return BuildTarget.StandaloneWindows;
				/*case RuntimePlatform.LinuxEditor:
					return BuildTarget.StandaloneLinux;*/
			}

			throw new System.NotImplementedException ("Support for this platform has not yet been added");
		}
	}


	static string Extension
	{
		get
		{
			switch (Application.platform)
			{
				case RuntimePlatform.OSXEditor:
					return ".app";
				case RuntimePlatform.WindowsEditor:
					return ".exe";
				/*case RuntimePlatform.LinuxEditor:
					return "";*/
			}

			throw new System.NotImplementedException ("Support for this platform has not yet been added");
		}
	}


	static string GetBuildPath (string binaryName)
	{
		return Application.dataPath + "/../" + binaryName + Extension;
	}


	static string GetBinaryPath (string binaryName)
	{
		return GetBuildPath (binaryName) + ((Target == BuildTarget.StandaloneOSXIntel) ? "/Contents/MacOS/" + PlayerSettings.productName : "");
	}


	static string GetProcessName (string binaryName)
	{
		return (Target == BuildTarget.StandaloneOSXIntel) ? PlayerSettings.productName : binaryName + Extension;
	}
}

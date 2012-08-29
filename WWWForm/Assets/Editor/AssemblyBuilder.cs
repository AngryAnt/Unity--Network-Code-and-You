using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.IO;


public class AssemblyBuilder : AssetPostprocessor
{
	const string
		kSourcePath = "Assets/Assembly/",
		kSourceExtension = ".cs.txt",
		kBuildTarget = "Assets/WebPlayerTemplates/MyTemplate/Assembly.dll";


	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
	{
		List<string> changedPathes = new List<string> ();

		changedPathes.AddRange (importedAssets);
		changedPathes.AddRange (deletedAssets);
		changedPathes.AddRange (movedAssets);
		changedPathes.AddRange (movedFromPath);

		foreach (string path in changedPathes)
		{
			if (ValidSourceFile (path))
			{
				Build ();
				return;
			}
		}
	}


	[MenuItem ("Assets/Build external assembly")]
	static void Build ()
	{
		CompilerParameters compilerParameters = new CompilerParameters ();
		compilerParameters.OutputAssembly = kBuildTarget;
		compilerParameters.ReferencedAssemblies.Add (
			(
				EditorApplication.applicationContentsPath +
					"/Frameworks/Managed/UnityEngine.dll"
			).Replace ('/', Path.DirectorySeparatorChar)
		);

		List<string> source = GetSource (kSourcePath);

		CodeDomProvider codeProvider = CodeDomProvider.CreateProvider ("CSharp");
    	CompilerResults compilerResults = codeProvider.CompileAssemblyFromSource (compilerParameters, source.ToArray ());

    	if (compilerResults.Errors.Count > 0)
    	{
    		foreach (CompilerError error in compilerResults.Errors)
    		{
    			Debug.LogError (error.ToString ());
    		}
    	}

    	AssetDatabase.Refresh ();
	}


	static bool ValidSourceFile (string path)
	{
		return path.IndexOf (kSourcePath) == 0 && path.IndexOf (kSourceExtension) == path.Length - kSourceExtension.Length;
	}


	static List<string> GetSource (string path)
	{
		List<string> source = new List<string> ();

		foreach (string file in Directory.GetFiles (path))
		{
			if (!ValidSourceFile (file))
			{
				continue;
			}

			source.Add (File.ReadAllText (file));
		}

		foreach (string directory in Directory.GetDirectories (path))
		{
			source.AddRange (GetSource (directory));
		}

		return source;
	}
}

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class InstallHooks
{
	private const string packageName = "com.needle.git-hooks";

	static InstallHooks()
	{
		Install(false);
	}

	[MenuItem("Tools/Needle/Install Git Hooks 👌")]
	private static void Install()
	{
		Install(true);
	}

	private static void Install(bool log)
	{
		var hooks = GetHooks(log);
		if (hooks.Count <= 0)
		{
			if (log) Debug.Log("No hooks found to install");
			return;
		}
		var projectFolder = Application.dataPath;
		var dir = new DirectoryInfo(projectFolder);
		
		while (dir.Parent != null )
		{
			if (TryInstallHooks(log, dir, hooks))
			{
				break;
			}
			dir = dir.Parent;
		}
	}

	private static bool TryInstallHooks(bool log, DirectoryInfo dir, List<FileInfo> hooks)
	{
		if (!Directory.Exists(dir.FullName + "/.git")) return false;

		var hooksDir = dir.FullName + "/.git/hooks";
		if (log)
			Debug.Log("<b>Found git hooks directory</b>: " + hooksDir);
		
		if (!Directory.Exists(hooksDir)) Directory.CreateDirectory(hooksDir);
		foreach (var hook in hooks)
		{
			if (!hook.Exists) continue;
			var dest = new FileInfo(hooksDir + "/" + hook.Name);
			if (log || !dest.Exists || dest.Length != hook.Length)
			{
				Debug.Log("<b>Installed git hook</b>: " + hook.Name + "\n<b>From</b>: " + hook.FullName + "\n<b>To</b>: " + dest.FullName);
			}

			File.Copy(hook.FullName, dest.FullName, true);
		}

		return true;
	}

	private static List<FileInfo> GetHooks(bool log = false)
	{
		var hooksFolderPath = $"Packages/{packageName}/Hooks";
		if (log) Debug.Log("<b>Search hooks</b> in " + hooksFolderPath);
		var files = Directory.GetFiles(hooksFolderPath, "*", SearchOption.TopDirectoryOnly);
		var hooks = new List<FileInfo>();
		foreach (var file in files)
		{
			var fi = new FileInfo(file);
			if (string.IsNullOrEmpty(fi.Extension))
			{
				hooks.Add(fi);
			}
		}

		return hooks;
	}
}
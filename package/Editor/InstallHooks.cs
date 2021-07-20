using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class InstallHooks
{
    static InstallHooks()
    {
        var hooks = GetHooks();
        if (hooks.Count <= 0) return;
        var projectFolder = Application.dataPath;
        var dir = new DirectoryInfo(projectFolder);
        while (dir.Parent != null && !TryInstallHooks(dir, hooks))
        {
            dir = dir.Parent; 
        }
    }

    private static bool TryInstallHooks(DirectoryInfo dir, List<FileInfo> hooks)
    {
        if (!Directory.Exists(dir.FullName + "/.git")) return false;

        var hooksDir = dir.FullName + "/.git/hooks";
        if (!Directory.Exists(hooksDir)) Directory.CreateDirectory(hooksDir);
        Debug.Log("Found " + hooksDir);
        foreach (var hook in hooks)
        {
            if (!hook.Exists) continue;
            File.Copy(hook.FullName, hooksDir + "/" + hook.Name, true);
        }
        return true;

    }

    private static List<FileInfo> GetHooks()
    {
        var hooksFolderPath = "Packages/com.needle.hooks/Hooks";
        var files = Directory.GetFiles(hooksFolderPath, "*", SearchOption.AllDirectories);
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

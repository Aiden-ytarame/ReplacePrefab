using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace ReplacePrefab;

[BepInPlugin(Guid, Name, Version)]
[BepInProcess("Project Arrhythmia.exe")]

public class Plugin : BasePlugin
{
    static Plugin inst;

    public static ManualLogSource Logger => inst.Log;

    Harmony _harmony;
    const string Guid = "me.ytarame.ReplacePrefab";
    const string Name = "ReplacePrefab";
    const string Version = "1.0.0";


    public override void Load()
    {
        inst = this;
        _harmony = new Harmony(Guid);
        _harmony.PatchAll();

        // Plugin startup logic
        Log.LogInfo($"Plugin {Guid} is loaded!");
    }
    
}

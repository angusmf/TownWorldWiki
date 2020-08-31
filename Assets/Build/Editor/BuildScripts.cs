using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;

using System.Collections.Generic;
using System.Linq;
using System.IO;


public class BuildScripts
{

    public enum BuildMode
    {
        ci,
        menu,
    }


    public static string ClientExeName = "Client";
    public static string GameServerExeName = "GameServer";

    //public static string ClientControlsScene = "Assets/Antfarm/_Common/Scenes/" + ClientScenes.ClientControlsScene + ".unity";
    //public static string ClientLoginScene = "Assets/Antfarm/_Common/Scenes/" + ClientScenes.ClientLoginScene + ".unity";
    //public static string GameServerScene = "Assets/Antfarm/_Server/Scenes/" + GameServer.GameServerScene + ".unity";
    //public static string CarisWorld1Scene = "Assets/Antfarm/_Common/Scenes/" + ClientScenes.CarisWorldScene1 + ".unity";


    public enum Platforms
    {
        WinMono,
        WinIL2CPP,
        Linux,
        WebGL,
        Android,
        None,
    }

    public enum BuildSpecs
    {
        Client,
        GameServer,
        Client_GameServer,
        All,
        None,
    }

    static Platforms _platform = Platforms.None;
    static BuildSpecs _buildSpec = BuildSpecs.None;

    static BuildTarget _buildTarget;

    /// <summary>
    /// Build with "Development" flag, so that we can see the console if something 
    /// goes wrong
    /// </summary>
    public static BuildOptions _clientOptions;
    public static BuildOptions _serverOptions;// = BuildOptions.Development | BuildOptions.AllowDebugging;
    public static BuildTargetGroup _buildTargetGroup = BuildTargetGroup.Standalone;

    public static string PrevPath = null;

    static string _fileExt;

    static string _exeName = string.Empty;

    static string[] _args;


    #region Automated Builds


    static string GetArgumentValue(string arg)
    {
        if (!_args.Contains(arg))
            return string.Empty;

        var index = _args.ToList().FindIndex(0, a => a.Equals(arg));
        return _args[index + 1];
    }


    static void Build()
    {
        _args = System.Environment.GetCommandLineArgs();
        string path = GetArgumentValue("-buildPath");
        string buildSpecStr = GetArgumentValue("-buildSpec");
        string platformStr = GetArgumentValue("-buildPlatform");

        platformStr = platformStr.ToLower();
        switch (platformStr)
        {

            case "webgl":
                _platform = Platforms.WebGL;
                break;
            case "winmono":
                _platform = Platforms.WinMono;
                break;
            case "winil2cpp":
                _platform = Platforms.WinIL2CPP;
                break;
            case "linux":
                _platform = Platforms.Linux;
                break;
            case "android":
                _platform = Platforms.Android;
                break;
            default:
                break;
        }

        buildSpecStr = buildSpecStr.ToLower();
        buildSpecStr = buildSpecStr.Replace('_', '-');
        switch (buildSpecStr)
        {

            case "client":
                _buildSpec = BuildSpecs.Client;
                break;
            case "gameserver":
                _buildSpec = BuildSpecs.GameServer;
                break;
            case "client-gameserver":
            case "gameserver-client":
                _buildSpec = BuildSpecs.Client_GameServer;
                break;
            case "all":
                _buildSpec = BuildSpecs.All;
                break;
            default:
                break;
        }
        Build(path);

    }

    static void Build(string path, BuildOptions clientOptions = BuildOptions.None, BuildOptions serverOptions = BuildOptions.None)
    {

        _clientOptions = clientOptions;
        _serverOptions = serverOptions;

        if (!string.IsNullOrEmpty(path))
        {

            switch (_platform)
            {

                case Platforms.WebGL:
                    Settings_WebGL();
                    break;
                case Platforms.WinMono:
                    Settings_WinMono();
                    break;
                case Platforms.WinIL2CPP:
                    Settings_WinIL2CPP();
                    break;
                case Platforms.Linux:
                    Settings_Linux();
                    break;
                case Platforms.Android:
                    Settings_Android();
                    break;
                default:
                    Debug.LogError("Build failed: Platform not found!");
                    break;
            }


            switch (_buildSpec)
            {

                case BuildSpecs.Client:
                    BuildClient(path);
                    break;
                case BuildSpecs.GameServer:
                    BuildGameServer(path);
                    break;
                case BuildSpecs.All:
                    BuildClient(path);
                    BuildGameServer(path);
                    break;
                case BuildSpecs.Client_GameServer:
                    BuildClient(path);
                    BuildGameServer(path);
                    break;

                default:
                    Debug.LogError("Build(): No such build spec found in BuildSpecs!");
                    break;
            }

        }
        else Debug.LogError("Build failed: Missing buildPath,  or buildSpec or buildPlatform. Check commandline");

        CopySupportFiles(path);
    }


    #endregion


    #region Editor Menu



    [MenuItem("Tools/Build/WinMono/Game Server", false, 11)]
    public static void BuildWinMonoGameServerMenu()
    {
        _platform = Platforms.WinMono;
        _buildSpec = BuildSpecs.GameServer;
        Build(GetPath(), serverOptions: BuildOptions.EnableHeadlessMode);
    }

    [MenuItem("Tools/Build/WinMono/Client", false, 11)]
    public static void BuildWinMonoClientMenu()
    {
        _platform = Platforms.WinMono;
        _buildSpec = BuildSpecs.Client;
        Build(GetPath());
    }

    [MenuItem("Tools/Build/WinMono/All", false, 11)]
    public static void BuildWinMonoAllMenu()
    {
        _platform = Platforms.WinMono;
        _buildSpec = BuildSpecs.All;
        Build(GetPath(), serverOptions: BuildOptions.EnableHeadlessMode);
    }

    [MenuItem("Tools/Build/WinMonoDev/Game Server", false, 11)]
    public static void BuildWinMonoGameDevServerMenu()
    {
        _platform = Platforms.WinMono;
        _buildSpec = BuildSpecs.GameServer;
        Build(GetPath(), BuildOptions.Development, BuildOptions.Development);
    }

    [MenuItem("Tools/Build/WinMonoDev/Client", false, 11)]
    public static void BuildWinMonoDevClientMenu()
    {
        _platform = Platforms.WinMono;
        _buildSpec = BuildSpecs.Client;
        Build(GetPath(), BuildOptions.Development, BuildOptions.Development);
    }

    [MenuItem("Tools/Build/WinMonoDev/All", false, 11)]
    public static void BuildWinMonoDevAllMenu()
    {
        _platform = Platforms.WinMono;
        _buildSpec = BuildSpecs.All;
        Build(GetPath(), BuildOptions.Development, BuildOptions.Development);
    }

    [MenuItem("Tools/Build/WinIL2CPP/Game Server", false, 11)]
    public static void BuildWinIL2CPPGameServerMenu()
    {
        _platform = Platforms.WinIL2CPP;
        _buildSpec = BuildSpecs.GameServer;
        Build(GetPath());
    }

    [MenuItem("Tools/Build/WinIL2CPP/Client", false, 11)]
    public static void BuildWinIL2CPPClientMenu()
    {
        _platform = Platforms.WinIL2CPP;
        _buildSpec = BuildSpecs.Client;
        Build(GetPath());
    }


    [MenuItem("Tools/Build/Linux/Client", false, 11)]
    public static void BuildLinuxClientMenu()
    {
        _platform = Platforms.Linux;
        _buildSpec = BuildSpecs.Client;
        Build(GetPath());
    }


    [MenuItem("Tools/Build/Linux/Game Server", false, 11)]
    public static void BuildLinuxGameServerMenu()
    {
        _platform = Platforms.Linux;
        _buildSpec = BuildSpecs.GameServer;
        Build(GetPath());
    }


    [MenuItem("Tools/Build/Linux/All", false, 11)]
    public static void BuildLinuxAllMenu()
    {
        _platform = Platforms.Linux;
        _buildSpec = BuildSpecs.All;
        Build(GetPath());
    }

    [MenuItem("Tools/Build/WebGL Client", false, 11)]
    public static void BuildWebGLClientMenu()
    {
        _platform = Platforms.WebGL;
        _buildSpec = BuildSpecs.Client;
        Build(GetPath());
    }

    [MenuItem("Tools/Build/Android Client", false, 11)]
    public static void BuildAndroidClientMenu()
    {

        _platform = Platforms.Android;
        _buildSpec = BuildSpecs.Client;
        Build(GetPath());
    }

    #endregion


    #region Build Settings
    public static void Settings_WinMono()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
        _buildTarget = BuildTarget.StandaloneWindows64;
        _fileExt = ".exe";
    }

    public static void Settings_WinIL2CPP()
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
        _buildTarget = BuildTarget.StandaloneWindows64;
        _fileExt = ".exe";
    }

    public static void Settings_Linux()
    {
        _buildTarget = BuildTarget.StandaloneLinux64;
        _fileExt = ".x86_64";
    }

    public static void Settings_Android()
    {
        _buildTargetGroup = BuildTargetGroup.Android;
        _buildTarget = BuildTarget.Android;
        _fileExt = ".apk";
    }

    public static void Settings_WebGL()
    {
        _buildTargetGroup = BuildTargetGroup.WebGL;
        _buildTarget = BuildTarget.WebGL;
    }


    #endregion

    private static void CopySupportFiles(string destination)
    {
        Debug.Log("CopySupportfiles - " + destination);

        string source = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "build");

        //copy startscripts
        switch (_platform)
        {
            case Platforms.Android:
            case Platforms.WebGL:
                return;
            case Platforms.Linux:
                CopySupportDir(Path.Combine(Path.Combine(destination, "Linux"), "start_scripts"), Path.Combine(Path.Combine(source, "Linux"), "start_scripts"));
                break;
            case Platforms.WinMono:
            case Platforms.WinIL2CPP:
                break;
        }
    }



    private static void CopySupportDir(string destination, string sourceDir)
    {
        if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);


        foreach (string file in Directory.GetFiles(sourceDir))
        {
            string filename = Path.GetFileName(file);
            if (!File.Exists(Path.Combine(destination, filename)))
            {
                File.Copy(file, Path.Combine(destination, filename));
            }
        }
    }


    /// <summary>
    /// Creates a build for client
    /// </summary>
    /// <param name="path"></param>
    public static void BuildClient(string path)
    {

        //always add start scene first - scene id = 0
        List<string> scenes = new List<string>(new string[] {
            //ClientLoginScene,
        });

        //add Game scenes next so they are at the same scene index on client and server (doesn't seem to matter)
        scenes.AddRange(GetGameScenes());

        //add BaseMSFScene and any additional needed
        scenes.AddRange(new string[]{

            //ClientControlsScene,

        });

        PlayerSettings.SetApiCompatibilityLevel(_buildTargetGroup, ApiCompatibilityLevel.NET_Standard_2_0);

        BuildPlayer(scenes, ClientExeName, path, _clientOptions);
    }

    /// <summary>
    /// Creates a build for game server
    /// </summary>
    /// <param name="path"></param>
    public static void BuildGameServer(string path)
    {
        //always add start scene first - scene id = 0
        List<string> scenes = new List<string>(new string[] {
            //GameServerScene,
        });

        //add Game scenes next so they are at the same scene index on client and server (doesn't seem to matter)
        scenes.AddRange(GetGameScenes());

        PlayerSettings.SetApiCompatibilityLevel(_buildTargetGroup, ApiCompatibilityLevel.NET_Standard_2_0);

        BuildPlayer(scenes, GameServerExeName, path, _serverOptions);
    }

    private static List<string> GetGameScenes()
    {
        return new List<string>(new string[]{
            "Assets/Scenes/main.unity",
        })
        ;

    }

    private static void BuildPlayer(List<string> scenes, string filename, string path, BuildOptions buildOptions)
    {

        _exeName = PlayerSettings.productName + filename + _fileExt;
        BuildReport report = BuildPipeline.BuildPlayer(scenes.ToArray(), path + "/" + _platform.ToString() + "/" + filename + "/" + _exeName, _buildTarget, buildOptions);
        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError("Build failed!");
        }
        else
        {
            Debug.Log("Build succeeded!");
        }
    }



    public static string GetPath()
    {
        var prevPath = EditorPrefs.GetString("msf.buildPath", "");
        string path = EditorUtility.SaveFolderPanel("Choose Location for binaries", prevPath, "");

        if (!string.IsNullOrEmpty(path))
        {
            EditorPrefs.SetString("msf.buildPath", path);
        }
        return path;
    }


}
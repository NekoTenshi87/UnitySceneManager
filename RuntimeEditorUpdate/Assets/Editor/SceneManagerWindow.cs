using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class SceneManagerWindow : EditorWindow
{
    //private bool initScenes = true;
    //private int index = 0;
    private enum ServerType
    {
        None,
        Server,
        Client
    };

    //string[] service_names = new string[] { "Server", "Host", "Client" };

    int service = 0;

    string ip_addr = "127.0.0.1";
    string ip_port = "4444";
    string nick_name;

    EditorWindow lobby;

    //string[] scenesPath;
    //string[] scenesName;

    [MenuItem("Collaborate/Scene Manager")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SceneManagerWindow), false, "Scene Manager", true);
    }

    private void OnGUI()
    {
        /*
        if (initScenes)
        {
            FindScenePaths();
            initScenes = false;
        }*/

        if (service != (int)ServerType.Server)
        {
            if (GUILayout.Button("Start New Server"))
            {
                // TODO: Start Server

                service = (int)ServerType.Server;
            }
        }
        else
        {
            if (GUILayout.Button("Stop Server"))
            {
                // TODO: Start Server

                service = (int)ServerType.None;
            }
        }

        ShowIPAddr();

        if (service != (int)ServerType.Client)
        {
            if (GUILayout.Button("Join Server"))
            {
                // TODO: Join Host
                lobby = EditorWindow.GetWindow(typeof(SceneLinkerWindow), false, "Session Lobby", true);

                service = (int)ServerType.Client;
            }
        }
        else
        {
            if (GUILayout.Button("Leave Server"))
            {
                // TODO: Join Host
                lobby.Close();

                service = (int)ServerType.None;
            }
        }

    }

    private void Update()
    {
        //NetworkManagerEditor.
    }
    /*
    private void FindScenePaths()
    {
        string[] scenesGUIDs = AssetDatabase.FindAssets("t:Scene");
        scenesPath = new string[scenesGUIDs.Length];
        scenesName = new string[scenesGUIDs.Length];

        for (int i = 0; i < scenesGUIDs.Length; ++i)
        {
            scenesPath[i] = AssetDatabase.GUIDToAssetPath(scenesGUIDs[i]);
            scenesName[i] = Path.GetFileNameWithoutExtension(scenesPath[i]);
        }
    }

    private void ShowScenePaths()
    {
        index = EditorGUILayout.Popup(index, scenesName);
    }*/

    private void ShowIPAddr()
    {
        ip_addr = EditorGUILayout.TextField("IP Address:", ip_addr);
        ip_port = EditorGUILayout.TextField("Port:", ip_port);
        nick_name = EditorGUILayout.TextField("Nickname:", nick_name);
    }
}

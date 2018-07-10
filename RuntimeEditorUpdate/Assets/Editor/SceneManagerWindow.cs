using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class SceneManagerWindow : EditorWindow
{
    private bool initScenes = true;
    private int index = 0;
    private enum ServerType
    {
        Server,
        Host,
        Client
    };

    string[] service_names = new string[] { "Server", "Host", "Client" };

    int service = 0;

    string ip_addr = "127.0.0.1";
    string ip_port = "4444";

    string[] scenesPaths;

    [MenuItem("Scene Manager/Sessions")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SceneManagerWindow), false, "Scene Manager", true);
    }

    private void OnGUI()
    {
        if (initScenes)
        {
            FindScenePaths();
            initScenes = false;
        }

        GUILayout.Label("Service Type", EditorStyles.boldLabel);
        service = EditorGUILayout.Popup(service, service_names);

        switch(service)
        {
            case (int)ServerType.Server:
                {
                    ShowScenePaths();
                    ShowIPAddr();

                    if (GUILayout.Button("Start Server"))
                    {
                        // TODO: Start Server


                        EditorSceneManager.OpenScene(scenesPaths[index]);
                    }

                    break;
                }

            case (int)ServerType.Host:
                {
                    ShowScenePaths();
                    ShowIPAddr();

                    if (GUILayout.Button("Start Host"))
                    {
                        // TODO: Start Host


                        EditorSceneManager.OpenScene(scenesPaths[index]);
                    }

                    break;
                }

            case (int)ServerType.Client:
                {
                    ShowScenePaths();
                    ShowIPAddr();

                    if (GUILayout.Button("Join Host"))
                    {
                        // TODO: Join Host


                        EditorSceneManager.OpenScene(scenesPaths[index]);
                    }

                    break;
                }
        }

    }

    private void Update()
    {
        //NetworkManagerEditor.
    }

    private void FindScenePaths()
    {
        string[] scenesGUIDs = AssetDatabase.FindAssets("t:Scene");
        scenesPaths = new string[scenesGUIDs.Length];

        for (int i = 0; i < scenesGUIDs.Length; ++i)
        {
            scenesPaths[i] = AssetDatabase.GUIDToAssetPath(scenesGUIDs[i]);
        }
    }

    private void ShowScenePaths()
    {
        index = EditorGUILayout.Popup(index, scenesPaths);
    }

    private void ShowIPAddr()
    {
        ip_addr = EditorGUILayout.TextField("IP Address:", ip_addr);
        ip_port = EditorGUILayout.TextField("Port:", ip_port);
    }
}

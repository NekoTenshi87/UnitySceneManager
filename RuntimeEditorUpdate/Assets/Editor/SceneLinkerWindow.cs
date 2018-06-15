﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

class SceneLinkerWindow : EditorWindow
{
    string SourcePath;
    string SceneSourceFile;
    string DestinationPath;
    string SceneDestinationFile;
    string Status = "Unlinked";

    bool isLinked = false;

    GameObject[] g_gameObjects;

    [MenuItem("Scene Linker/Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SceneLinkerWindow), false, "Scene Linker", true);
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        GUILayout.Label("Link Source", EditorStyles.boldLabel);
        SourcePath = EditorGUILayout.TextField("Source Path:", SourcePath);
        SceneSourceFile = EditorGUILayout.TextField("Scene Name:", SceneSourceFile);

        GUILayout.Label("Link Destination", EditorStyles.boldLabel);
        DestinationPath = EditorGUILayout.TextField("Destination Path:", DestinationPath);
        SceneDestinationFile = EditorGUILayout.TextField("Scene Name:", SceneDestinationFile);

        if (EditorGUI.EndChangeCheck())
        {
            //UnityEditor.SceneManagement.EditorSceneManager.activeSceneChanged -= ChangedActiveScene;
            isLinked = false;
            Status = "Unlinked";
        }

        if (GUILayout.Button("Link"))
        {
            if (SourcePath != null && DestinationPath != null)
            {
                if (SceneSourceFile != null && SceneDestinationFile != null)
                {
                    //UnityEditor.SceneManagement.EditorSceneManager.activeSceneChanged += ChangedActiveScene;

                    //g_gameObjects = EditorSceneManager.GetSceneByName("Scene2").GetRootGameObjects();


                    isLinked = true;
                    Status = "Linked";
                }
            }
        }

        GUILayout.Label("Link Status: " + Status, EditorStyles.boldLabel);
    }

    private void OnInspectorUpdate()
    {
        if (isLinked)
        {
            /*
            Scene scene_copy = UnityEditor.SceneManagement.EditorSceneManager.GetSceneByName("Scene1");
            UnityEditor.SceneManagement.EditorSceneManager.UnloadScene("Scene2");
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene_copy, "Assets/Scene2.unity", true);*/

            //GameObject sc1 = EditorSceneManager.GetSceneByName("Scene2").GetRootGameObjects().g  GameObject.Find("New Sprite");
            //GameObject sc2 = g_gameObjects.
            GameObject[] sc1 = EditorSceneManager.GetSceneByName("Scene1").GetRootGameObjects();
            GameObject[] sc2 = EditorSceneManager.GetSceneByName("Scene2").GetRootGameObjects();

            sc2[1].transform.localPosition = sc1[1].transform.localPosition;


        }
    }
    /*
    private void ChangedActiveScene(Scene current, Scene next)
    {
        string currentName = current.name;

        if (currentName == null)
        {
            // Scene1 has been removed
            currentName = "Replaced";
        }

        Debug.Log("Scenes: " + currentName + ", " + next.name);
    }*/
}


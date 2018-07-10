using System.IO;
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
            isLinked = false;
            Status = "Unlinked";
        }

        if (GUILayout.Button("Link"))
        {
            if (SourcePath != null && DestinationPath != null)
            {
                if (SceneSourceFile != null && SceneDestinationFile != null)
                {



                    isLinked = true;
                    Status = "Linked";
                }
            }
        }

        GUILayout.Label("Link Status: " + Status, EditorStyles.boldLabel);
    }

    private void Update()
    {
        if (isLinked)
        {

            GameObject[] obj = Selection.gameObjects;
            int[] obj_id = Selection.instanceIDs;

            foreach (GameObject o in obj)
            {
                if (Selection.Contains(o))
                {
                    Debug.Log("ID: " + o.GetInstanceID() + " belongs to object: " + o.name);
                }
            }

            //GameObject[] sc1 = EditorSceneManager.GetSceneByName("Scene1").GetRootGameObjects();
            //GameObject[] sc2 = EditorSceneManager.GetSceneByName("Scene2").GetRootGameObjects();

            //int size = sc1.Length;
            /*
            for (int i = 0; i < size; ++i)
            {
                EditorUtility.CopySerializedIfDifferent(sc1[i], sc2[i]);

                Component[] cp1 = sc1[i].GetComponents(typeof(Component));
                Component[] cp2 = sc2[i].GetComponents(typeof(Component));

                int size2 = cp2.Length;

                for (int j = 0; j < size2; ++j)
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(cp1[j]);
                    UnityEditorInternal.ComponentUtility.PasteComponentValues(cp2[j]);
                }
            }*/
        }
    }
}


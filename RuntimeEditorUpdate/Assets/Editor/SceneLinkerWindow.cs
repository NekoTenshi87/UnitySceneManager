using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

class SceneLinkerWindow : EditorWindow
{
    string SceneSourceFile;
    string SceneDestinationFile;
    string Status = "Unlinked";

    bool sc1_exists = false;
    bool sc2_exists = false;

    Scene sc1;
    Scene sc2;

    bool isLinked = false;

    [MenuItem("Scene Manager/Linker Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SceneLinkerWindow), false, "Scene Linker", true);
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        GUILayout.Label("Link Source", EditorStyles.boldLabel);
        SceneSourceFile = EditorGUILayout.TextField("Scene Name:", SceneSourceFile);

        GUILayout.Label("Link Destination", EditorStyles.boldLabel);
        SceneDestinationFile = EditorGUILayout.TextField("Scene Name:", SceneDestinationFile);

        if (EditorGUI.EndChangeCheck())
        {
            isLinked = false;
            Status = "Unlinked";
        }

        GUILayout.Label("Link Status: " + Status, EditorStyles.boldLabel);

        if (isLinked)
        {
            GUI.enabled = false;
        }
        else
        {
            GUI.enabled = true;
        }
        bool linkButton = GUILayout.Button("Link");

        if (isLinked)
        {
            GUI.enabled = true;
        }
        else
        {
            GUI.enabled = false;
        }
        bool unlinkButton = GUILayout.Button("Unlink");

        GUI.enabled = true;

        if (linkButton && !isLinked)
        {
            sc1_exists = File.Exists("Assets/" + SceneSourceFile + ".unity");
            sc2_exists = File.Exists("Assets/" + SceneDestinationFile + ".unity");

            if (!sc1_exists)
            {
                Status = "Invalid Scource scene name!";
            }
            else if (!sc2_exists)
            {
                Status = "Invalid Destination scene name!";
            }
            else
            {
                sc1 = EditorSceneManager.GetSceneByName(SceneSourceFile);
                if (!sc1.IsValid())
                {
                    sc1 = EditorSceneManager.OpenScene("Assets/" + SceneSourceFile + ".unity", OpenSceneMode.Additive);
                }

                sc2 = EditorSceneManager.GetSceneByName(SceneDestinationFile);
                if (!sc2.IsValid())
                {
                    sc2 = EditorSceneManager.OpenScene("Assets/" + SceneDestinationFile + ".unity", OpenSceneMode.Additive);
                }

                int sceneCount = EditorSceneManager.sceneCount;

                if (sceneCount > 0)
                {
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

                    List<int> sc_index = new List<int>();

                    for (int i = 0; i < sceneCount; ++i)
                    {
                        if (EditorSceneManager.GetSceneAt(i) != sc1 &&
                            EditorSceneManager.GetSceneAt(i) != sc2)
                        {
                            sc_index.Add(i);
                        }
                    }

                    foreach (int si in sc_index)
                    {
                        EditorSceneManager.CloseScene(EditorSceneManager.GetSceneAt(si), true);
                    }
                }

                isLinked = true;
                Status = "Linked";
            }
        }

        if (unlinkButton && isLinked)
        {
            isLinked = false;
            Status = "Unlinked";
        }
    }

    private void Update()
    {
        if (isLinked)
        {
            //GameObject[] obj = Selection.gameObjects;
            //int[] obj_id = Selection.instanceIDs;

            //foreach (GameObject o in obj)
            //{
            //    if (Selection.Contains(o))
            //    {
            //        Debug.Log("ID: " + o.GetInstanceID() + " belongs to object: " + o.name);
            //    }
            //}

            GameObject[] sc1_objs = sc1.GetRootGameObjects();
            GameObject[] sc2_objs = sc2.GetRootGameObjects();

            //foreach(GameObject go1 in obj)
            //{
            //    foreach(GameObject go2 in sc2)
            //    {
            //if(go2 == go1)
            //        if(go1.Equals(go2))
            //        {
            //            Debug.Log("Same Object " + go1.name + " == " + go2.name);
            //        }
            //        else
            //        {
            //            Debug.Log("Invalid " + go1.name + " != " + go2.name);
            //        }
            //    }
            //}

            
            int size1 = sc1_objs.Length;/*
            int size2 = sc2_objs.Length;
            int size_min = Mathf.Min(sc1_objs.Length, sc2_objs.Length);
            int size_max = Mathf.Max(sc1_objs.Length, sc2_objs.Length);

            if (size1 < size2)
            {
                // 
            }
            else if (size1 > size2)
            {

            }
            else
            {

            }
            */

            for (int i = 0; i < size1; ++i)
            {
                EditorUtility.CopySerializedIfDifferent(sc1_objs[i], sc2_objs[i]);

                Component[] cp1 = sc1_objs[i].GetComponents(typeof(Component));
                Component[] cp2 = sc2_objs[i].GetComponents(typeof(Component));

                int size22 = cp2.Length;

                for (int j = 0; j < size22; ++j)
                {
                    UnityEditorInternal.ComponentUtility.CopyComponent(cp1[j]);
                    UnityEditorInternal.ComponentUtility.PasteComponentValues(cp2[j]);
                }
            }

            //int size2 = Mathf.Max(sc1_objs.Length, sc2_objs.Length);

            //for (int j = size; )
        }
    }
}


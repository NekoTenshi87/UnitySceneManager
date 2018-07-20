using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.Networking;

public class SceneLinker
{
    // Vars
    string SceneServerFile;

    string Status = "Unlinked";
    bool isLinked = false;
    bool isReset = false;

    bool sc1_exists = false;

    SceneManagerServer Server = new SceneManagerServer();
    SceneManagerClient Client = new SceneManagerClient();

    // Methods

    public void Init()
    {
        Undo.postprocessModifications += OnPostProcessModifications;
        Server.Init();
        Client.Init();

        Server.AddClient(Client);
        Client.SetServer(Server);
    }

    public void Update()
    {
        if (isLinked)
        {
            //Client.Update();
            //GameObject[] obj = Selection.gameObjects;
            //int[] obj_id = Selection.instanceIDs;

            //GameObject[] sc1_objs = sc1.GetRootGameObjects();
            //GameObject[] sc2_objs = sc2.GetRootGameObjects();

            //int size1 = sc1_objs.Length;
            //int size2 = sc2_objs.Length;

            
            /*
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
            }*/
        }


        Client.Update();
        Server.Update();

        if (isReset)
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            isReset = false;
        }
    }

    public void Close()
    {
        Undo.postprocessModifications -= OnPostProcessModifications;
        Client.Close();
        Server.Close();
    }

    public void Display()
    {
        EditorGUI.BeginChangeCheck();
        GUILayout.Label("Link Source", EditorStyles.boldLabel);
        SceneServerFile = EditorGUILayout.TextField("Scene Name:", SceneServerFile);

        if (EditorGUI.EndChangeCheck())
        {
            if (isLinked)
            {
                LeaveMessage msg = new LeaveMessage();
                ClientInfo c_info = new ClientInfo();
                c_info.client_id = Client.client_id;
                c_info.m_name = Client.client_name;
                msg.client = c_info;

                Client.SendLeaveMsg(Server, msg);
                
                isLinked = false;
                isReset = true;
            }

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
            sc1_exists = File.Exists("Assets/" + SceneServerFile + ".unity");

            if (!sc1_exists)
            {
                Status = "Invalid scene name!";
            }
            else
            {
                JoinMessage msg = new JoinMessage();
                ClientInfo c_info = new ClientInfo();
                c_info.client_id = Client.client_id;
                c_info.m_name = Client.client_name;
                msg.client = c_info;
                msg.scene_name = SceneServerFile;

                Client.SendJoinMsg(Server, msg);

                /*
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
                }*/

                isLinked = true;
                Status = "Linked";
            }
        }

        if (unlinkButton && isLinked)
        {
            LeaveMessage msg = new LeaveMessage();
            ClientInfo c_info = new ClientInfo();
            c_info.client_id = Client.client_id;
            c_info.m_name = Client.client_name;
            msg.client = c_info;

            Client.SendLeaveMsg(Server, msg);

            // EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            isLinked = false;
            isReset = true;
            Status = "Unlinked";
        }
    }

    UndoPropertyModification[] OnPostProcessModifications(UndoPropertyModification[] propertyModifications)
    {
        foreach (UndoPropertyModification mod in propertyModifications)
        {
            Debug.Log("Modified: " + mod.currentValue.propertyPath +
                      " Value: " + mod.currentValue.value +
                      " Object: " + mod.currentValue.target.name +
                      " ObjectID: " + UnityEditorInternal.InternalEditorUtility.GetGameObjectInstanceIDFromComponent(mod.currentValue.target.GetInstanceID()) +
                      " Componenet ObjectID: " + mod.currentValue.target.GetInstanceID() +
                      " Component: " + mod.currentValue.target.GetType().Name);

            if (mod.currentValue.target.GetType() == typeof(GameObject))
            {
                GameObject obj = (GameObject)mod.currentValue.target;
                
                SyncObjectMessage msg = new SyncObjectMessage();
                msg.prop = SerializeMod(mod.currentValue);
                msg.client_info = new ClientInfo();

                if (obj.scene.name == "Session")
                {
                    msg.scene_name = Client.scene_name;
                    msg.client_info.client_id = Client.client_id;
                    msg.client_info.m_name = Client.client_name;
                    msg.object_id = Client.GetServerObjID(mod.currentValue.target.GetInstanceID());

                    Client.SendSyncObjectMessage(Server, msg);
                }
                else
                {
                    msg.scene_name = obj.scene.name;
                    msg.client_info.client_id = 0;
                    msg.client_info.m_name = "Server";
                    msg.object_id = mod.currentValue.target.GetInstanceID();

                    Server.SendSyncObjectMessage(Client, msg);
                }
            }
            else// if (mod.currentValue.target.GetType() == typeof(Component))
            {
                Component comp = (Component)mod.currentValue.target;

                SyncObjectMessage msg = new SyncObjectMessage();
                msg.prop = SerializeMod(mod.currentValue);
                msg.client_info = new ClientInfo();

                if (comp.gameObject.scene.name == "Session")
                {
                    msg.scene_name = Client.scene_name;
                    msg.client_info.client_id = Client.client_id;
                    msg.client_info.m_name = Client.client_name;
                    msg.object_id = Client.GetServerObjID(mod.currentValue.target.GetInstanceID());

                    Client.SendSyncObjectMessage(Server, msg);
                }
                else
                {
                    msg.scene_name = comp.gameObject.scene.name;
                    msg.client_info.client_id = 0;
                    msg.client_info.m_name = "Server";
                    msg.object_id = mod.currentValue.target.GetInstanceID();

                    Server.SendSyncObjectMessage(Client, msg);
                }
            }
        }

        return propertyModifications;
    }

    SerializedProperty SerializeMod(PropertyModification mod)
    {
            SerializedObject ser_obj = new UnityEditor.SerializedObject(mod.target);
            return ser_obj.FindProperty(mod.propertyPath);
    }
}


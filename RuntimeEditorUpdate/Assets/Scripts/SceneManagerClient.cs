using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;



public class SceneManagerClient
{
    // Vars
    SceneManagerServer server;

    public int client_id;
    public string client_name;
    public string scene_name;
    Session session = new Session();

    Dictionary<int, int> ClientToServerID = new Dictionary<int, int>();
    Dictionary<int, int> ServerToClientID = new Dictionary<int, int>();

    Queue<JoinMessage> m_join_messages = new Queue<JoinMessage>();
    Queue<LeaveMessage> m_leave_messages = new Queue<LeaveMessage>();
    Queue<LockObjectMessage> m_lock_obj_messages = new Queue<LockObjectMessage>();
    Queue<UnlockObjectMessage> m_unlock_obj_messages = new Queue<UnlockObjectMessage>();
    Queue<SyncSceneMessage> m_sync_scene_messages = new Queue<SyncSceneMessage>();
    Queue<SyncObjectMessage> m_sync_obj_messages = new Queue<SyncObjectMessage>();

    // Methods

    public void Init()
    {
        client_id = 1;  // Set to 1 for SceneLinker example
        client_name = "Client 1";
    }

    public void SetServer(SceneManagerServer s)
    {
        server = s;
    }

    public void Update()
    {
        // Check for Join/Leave Session
        while (m_join_messages.Count > 0)
        {
            JoinMessage msg = m_join_messages.Dequeue();

            if (msg.scene_name == scene_name)
            {
                session.AddClient(msg.client);

                if (msg.client.client_id == client_id)
                {
                    SyncSceneMessage sync_msg = new SyncSceneMessage();
                    sync_msg.scene_name = scene_name;
                    sync_msg.client_id = client_id;

                    SendSyncSceneMsg(server, sync_msg);
                }
            }
        }

        while (m_leave_messages.Count > 0)
        {
            LeaveMessage msg = m_leave_messages.Dequeue();

            if (msg.scene_name == scene_name)
            {
                session.RemoveClient(msg.client);
            }
        }

        while (m_sync_scene_messages.Count > 0)
        {
            // Check if the given objects are in the client
            // Map the Client's ObjectID's to the Server's ObjectID's
            // Set Client Objects as copies of Server Objects
            // TODO: Create an object if it's not present on Client
            SyncSceneMessage msg = m_sync_scene_messages.Dequeue();

            if (msg.scene_name == scene_name)
            {
                GameObject[] objs = session.GetGameObjects();
                
                foreach(GameObject obj in objs)
                {
                    Object.DestroyImmediate(obj);
                }

                foreach(GameObject obj in msg.objects)
                {
                    GameObject g_obj = Object.Instantiate(obj);
                    ClientToServerID[g_obj.GetInstanceID()] = obj.GetInstanceID();
                    ServerToClientID[obj.GetInstanceID()] = g_obj.GetInstanceID();

                    Component[] server_comps = obj.GetComponents(typeof(Component));
                    Component[] client_comps = g_obj.GetComponents(typeof(Component));

                    for (int i = 0; i < client_comps.Length; ++i)
                    {
                        ClientToServerID[client_comps[i].GetInstanceID()] = server_comps[i].GetInstanceID();
                        ServerToClientID[server_comps[i].GetInstanceID()] = client_comps[i].GetInstanceID();
                    }
                }
            }
        }

        // Check for Locked/Unlocked Object(s)

        while (m_lock_obj_messages.Count > 0)
        {
            LockObjectMessage msg = m_lock_obj_messages.Dequeue();

            if (msg.scene_name == scene_name)
            {
                session.LockObject(msg.object_id, msg.lock_info.client_info);
            }
        }

        while (m_unlock_obj_messages.Count > 0)
        {
            UnlockObjectMessage msg = m_unlock_obj_messages.Dequeue();

            if (msg.scene_name == scene_name)
            {
                session.UnlockObject(msg.object_id, msg.lock_info.client_info);
            }
        }

        // Check for SyncObjects

        while (m_sync_obj_messages.Count > 0)
        {
            SyncObjectMessage msg = m_sync_obj_messages.Dequeue();

            if (msg.scene_name == scene_name)
            {
                msg.object_id = GetClientObjID(msg.object_id);

                if (msg.client_info.client_id != client_id)
                {
                    session.UpdateObject(msg);
                }
            }
        }

        // If any of those checked, update log



        //Object[] obj = Object.FindObjectsOfType(typeof(Component));


    }

    public void Close()
    {

    }

    public int GetServerObjID(int client_obj_id)
    {
        return ClientToServerID[client_obj_id];
    }

    public int GetClientObjID(int server_obj_id)
    {
        return ServerToClientID[server_obj_id];
    }

    public void SendJoinMsg(SceneManagerServer server, JoinMessage msg)
    {
        scene_name = msg.scene_name;
        session = new Session(EditorSceneManager.OpenScene("Assets/Session.unity"));
        server.ReceiveJoinMsg(msg);
    }

    public void ReceiveJoinMsg(JoinMessage msg)
    {
        m_join_messages.Enqueue(msg);
    }

    public void SendLeaveMsg(SceneManagerServer server, LeaveMessage msg)
    {
        scene_name = "";
        server.ReceiveLeaveMsg(msg);
    }

    public void ReceiveLeaveMsg(LeaveMessage msg)
    {
        //msg.scene_name = "Session";
        m_leave_messages.Enqueue(msg);
    }

    public void SendSyncSceneMsg(SceneManagerServer server, SyncSceneMessage msg)
    {
        server.ReceiveSyncSceneMsg(msg);
    }

    public void RecieveSyncSceneMsg(SyncSceneMessage msg)
    {
        m_sync_scene_messages.Enqueue(msg);
    }

    public void SendSyncObjectMessage(SceneManagerServer server, SyncObjectMessage msg)
    {
        msg.scene_name = scene_name;
        server.RecieveSyncObjectMessage(msg);
    }

    public void RecieveSyncObjectMessage(SyncObjectMessage msg)
    {
        m_sync_obj_messages.Enqueue(msg);
    }
}

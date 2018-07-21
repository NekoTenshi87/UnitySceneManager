using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

class SessionManager
{
    // Vars
    List<Session> m_session_list = new List<Session>();

    public void Update()
    {
        // Check for Locked/Unlocked Object(s)

        // Check for SyncObjects

        // If any of those checked, update log
    }

    public Session CreateSession(string scene_path, UnityEditor.SceneManagement.OpenSceneMode mode = OpenSceneMode.Single)
    {
        Session session = new Session(EditorSceneManager.OpenScene(scene_path, mode));

        m_session_list.Add(session);

        return session;
    }

    public void DestroySession(Session s)
    {
        //EditorSceneManager.CloseScene(s.m_scene, true);
        //EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        m_session_list.Remove(s);
    }

    public void Close()
    {
        m_session_list.Clear();
    }

    public Session GetSessionBySceneName(string name)
    {
        return m_session_list.Find(x => x.Scene.name == name);
    }

    public bool Exists(string scene_name)
    {
        return m_session_list.Exists(x => x.Scene.name == scene_name);
    }


}

public class SceneManagerServer
{
    // Vars
    List<SceneManagerClient> clients = new List<SceneManagerClient>();

    Dictionary<string, string> m_guids_to_path = new Dictionary<string, string>();
    Dictionary<string, string> m_name_to_path = new Dictionary<string, string>();
    string[] m_scenesNames;
    SessionManager m_session_mgr = new SessionManager();

    Queue<JoinMessage> m_join_messages = new Queue<JoinMessage>();
    Queue<LeaveMessage> m_leave_messages = new Queue<LeaveMessage>();
    Queue<LockObjectMessage> m_lock_obj_messages = new Queue<LockObjectMessage>();
    Queue<UnlockObjectMessage> m_unlock_obj_messages = new Queue<UnlockObjectMessage>();
    Queue<SyncSceneMessage> m_sync_scene_messages = new Queue<SyncSceneMessage>();
    Queue<SyncObjectMessage> m_sync_obj_messages = new Queue<SyncObjectMessage>();

    // Methods
    public void Init()
    {
        InitScenes();



    }

    public void AddClient(SceneManagerClient client)
    {
        clients.Add(client);
    }

    public void Update()
    {
        // Check for Join/Leave Session
        while (m_join_messages.Count > 0)
        {
            JoinMessage msg = m_join_messages.Dequeue();

            Session s = new Session();

            if (!m_session_mgr.Exists(msg.scene_name))
            {
                // CreateSession
                s = m_session_mgr.CreateSession(m_name_to_path[msg.scene_name], OpenSceneMode.Additive);
            }

            if (s)
            {
                s.AddClient(msg.client);

                foreach(SceneManagerClient smc in clients)
                {
                    SendJoinMsg(smc, msg);
                }
            }
        }

        while (m_leave_messages.Count > 0)
        {
            LeaveMessage msg = m_leave_messages.Dequeue();

            Session s = m_session_mgr.GetSessionBySceneName(msg.scene_name);

            if (s)
            {
                s.RemoveClient(msg.client);

                if (!(s.GetClientSize() > 0))
                {
                    // Save Scene
                    // EditorSceneManager.Save
                    // DestroySession
                    m_session_mgr.DestroySession(m_session_mgr.GetSessionBySceneName(msg.scene_name));
                }

                foreach (SceneManagerClient smc in clients)
                {
                    SendLeaveMsg(smc, msg);
                }
            }
        }

        while (m_sync_scene_messages.Count > 0)
        {
            // Grab all objects in the scene and send them to the client to map
            SyncSceneMessage msg = m_sync_scene_messages.Dequeue();

            Session s = m_session_mgr.GetSessionBySceneName(msg.scene_name);

            SyncSceneMessage sync_msg = new SyncSceneMessage();
            sync_msg.scene_name = s.Scene.name;
            sync_msg.client_id = 0;
            sync_msg.objects = s.Scene.GetRootGameObjects();

            SceneManagerClient client = clients.Find(x => x.client_id == msg.client_id);

            SendSyncSceneMsg(client, sync_msg);
        }

        while (m_lock_obj_messages.Count > 0)
        {
            LockObjectMessage msg = m_lock_obj_messages.Dequeue();

            Session s = m_session_mgr.GetSessionBySceneName(msg.scene_name);

            if (s)
            {
                s.LockObject(msg.object_id, msg.lock_info.client_info);

                //foreach (SceneManagerClient smc in clients)
                //{
                //    SendLockObjMsg(smc, msg);
                //}
            }
        }

        while (m_unlock_obj_messages.Count > 0)
        {
            UnlockObjectMessage msg = m_unlock_obj_messages.Dequeue();

            Session s = m_session_mgr.GetSessionBySceneName(msg.scene_name);

            if (s)
            {
                s.UnlockObject(msg.object_id, msg.lock_info.client_info);

                //foreach (SceneManagerClient smc in clients)
                //{
                //    SendUnlockObjMsg(smc, msg);
                //}
            }
        }

        while (m_sync_obj_messages.Count > 0)
        {
            SyncObjectMessage msg = m_sync_obj_messages.Dequeue();

            Session s = m_session_mgr.GetSessionBySceneName(msg.scene_name);

            if (s)
            {
                s.UpdateObject(msg);

                foreach (SceneManagerClient smc in clients)
                {
                    SendSyncObjectMessage(smc, msg);
                }
            }
        }

        m_session_mgr.Update();
    }

    public void Close()
    {

    }

    // Methods
    private void InitScenes()
    {
        string[] GUIDs = AssetDatabase.FindAssets("t:Scene");
        List<string> scene_names = new List<string>();

        for (int i = 0; i < GUIDs.Length; ++i)
        {
            string path = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
            string name = Path.GetFileNameWithoutExtension(path);
            if (name != "Session")
            {
                m_guids_to_path[GUIDs[i]] = path;
                scene_names.Add(name);
                m_name_to_path[name] = path;
            }
        }

        m_scenesNames = new string[scene_names.Count];

        for (int i = 0; i < m_scenesNames.Length; ++i)
        {
            m_scenesNames[i] = scene_names[0];
            scene_names.RemoveAt(0);
        }

    }

    public void RefreshSceneGUIDs()
    {
        string[] GUIDs = AssetDatabase.FindAssets("t:Scene");

        foreach (string s in GUIDs)
        {
            if (!m_guids_to_path.ContainsKey(s))
            {
                m_guids_to_path[s] = AssetDatabase.GUIDToAssetPath(s);
            }
        }
    }
    
    public string[] GetListOfAllSceneNames()
    {
        return m_scenesNames;
    }

    public void SendJoinMsg(SceneManagerClient client, JoinMessage msg)
    {
        client.ReceiveJoinMsg(msg);
    }

    public void ReceiveJoinMsg(JoinMessage msg)
    {
        m_join_messages.Enqueue(msg);
    }

    public void SendLeaveMsg(SceneManagerClient client, LeaveMessage msg)
    {
        client.ReceiveLeaveMsg(msg);
    }

    public void ReceiveLeaveMsg(LeaveMessage msg)
    {
        m_leave_messages.Enqueue(msg);
    }

    public void SendSyncSceneMsg(SceneManagerClient client, SyncSceneMessage msg)
    {
        client.RecieveSyncSceneMsg(msg);
    }

    public void ReceiveSyncSceneMsg(SyncSceneMessage msg)
    {
        m_sync_scene_messages.Enqueue(msg);
    }

    public void SendSyncObjectMessage(SceneManagerClient client, SyncObjectMessage msg)
    {
        client.RecieveSyncObjectMessage(msg);
    }

    public void RecieveSyncObjectMessage(SyncObjectMessage msg)
    {
        m_sync_obj_messages.Enqueue(msg);
    }

}

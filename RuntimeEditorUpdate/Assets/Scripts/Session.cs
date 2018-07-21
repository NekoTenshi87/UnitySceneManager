using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;


public class ClientInfo
{
    public int client_id = 0;
    public string m_name = "";
}

public class LockedInfo
{
    public bool is_locked = false;
    public ClientInfo client_info = new ClientInfo();
}

public class LockedList
{
    public Dictionary<int, LockedInfo> m_list = new Dictionary<int, LockedInfo>(); // <ObjectID, LockedInfo>
}

public class SyncObject
{
    public GameObject obj;
    public Component[] comps;
}

public class Session
{
    // Vars
    Scene m_scene = new Scene();
    List<ClientInfo> m_client_ids = new List<ClientInfo>();
    LockedList m_locked = new LockedList();

    public Scene Scene
    {
        get { return m_scene; }
    }

    public Session()
    {

    }

    public Session(Scene scene)
    {
        m_scene = scene;
    }

    public void AddClient(ClientInfo client)
    {
        m_client_ids.Add(client);
    }

    public void RemoveClient(ClientInfo client)
    {
        m_client_ids.Remove(client);
    }

    public int GetClientSize()
    {
        return m_client_ids.Count;
    }

    // Return True if object was locked properly
    // Return False if object is owned by someone else
    public bool LockObject(int object_id, ClientInfo client)
    {
        if (!IsObjectLocked(object_id))
        {
            LockedInfo info = m_locked.m_list[object_id];
            info.is_locked = true;
            info.client_info = client;
            m_locked.m_list[object_id] = info;

            return true;
        }

        return false;
    }

    // Return True if object was unlocked properly
    // Return False if object is owned by someone else
    public bool UnlockObject(int object_id, ClientInfo client)
    {
        if (IsObjectLocked(object_id))
        {
            if (DoesClientOwnObject(object_id, client))
            {
                m_locked.m_list[object_id].is_locked = false;

                return true;
            }
        }

        return false;
    }

    public bool IsObjectLocked(int object_id)
    {
        return m_locked.m_list[object_id].is_locked;
    }

    public bool DoesClientOwnObject(int object_id, ClientInfo client)
    {
        if (IsObjectLocked(object_id))
        {
            if (m_locked.m_list[object_id].client_info.client_id == client.client_id)
            {
                return true;
            }
        }

        return false;
    }

    public ClientInfo GetClientInfo(int object_id)
    {
        return m_locked.m_list[object_id].client_info;
    }

    public void UpdateObject(SyncObjectMessage msg)
    {
        Object obj = EditorUtility.InstanceIDToObject(msg.object_id);

        if (obj)
        {
            SerializedObject ser_obj = new UnityEditor.SerializedObject(obj);

            ser_obj.CopyFromSerializedProperty(msg.prop);
            ser_obj.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(m_scene);
        }
    }

    public bool IsValid()
    {
        return m_scene.IsValid();
    }

    public List<SyncObject> GetSyncObjects()
    {
        List<SyncObject> list = new List<SyncObject>();

        GameObject[] g_objs = m_scene.GetRootGameObjects();

        foreach (GameObject g_obj in g_objs)
        {
            SyncObject sync_obj = new SyncObject();

            sync_obj.obj = g_obj;
            sync_obj.comps = g_obj.GetComponents(typeof(Component));

            list.Add(sync_obj);
        }

        return list;
    }

    public GameObject[] GetGameObjects()
    {
        return m_scene.GetRootGameObjects();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////

    public override bool Equals(object other)
    {
        return other is Session && this == (Session)other;
    }

    public override int GetHashCode()
    {
        return m_scene.GetHashCode() ^ m_client_ids.GetHashCode() ^ m_locked.GetHashCode();
    }

    public static bool operator ==(Session lhs, Session rhs)
    {
        if (lhs.m_scene == rhs.m_scene)
        {
            return true;
        }

        return false;
    }

    public static bool operator !=(Session lhs, Session rhs)
    {
        return !(lhs == rhs);
    }

    public static implicit operator bool(Session exists)
    {
        if (exists is Session)
        {
            return exists.IsValid();
        }

        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class JoinMessage
{
    public string scene_name;
    public ClientInfo client;
}

public class LeaveMessage
{
    public string scene_name;
    public ClientInfo client;
}

public class LockObjectMessage
{
    public string scene_name;
    public int object_id;
    public LockedInfo lock_info;
}

public class UnlockObjectMessage
{
    public string scene_name;
    public int object_id;
    public LockedInfo lock_info;
}

public class SyncSceneMessage
{
    public string scene_name;
    public int client_id;
    //public List<SyncObject> sync_objects;
    public GameObject[] objects;
}

public class SyncObjectMessage
{
    public string scene_name;
    public int object_id;
    public ClientInfo client_info;
    //public Object obj;
    //public System.Type type;
    public SerializedProperty prop;
}

public class Networking
{

	// Use this for initialization
	void Init ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}

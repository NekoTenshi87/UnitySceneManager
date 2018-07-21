using UnityEditor;

// Scene 1 is Server objects
// Scene 1 Session is Client objects

// Client:

// Link to Server
// Login to Server
// Navagate Session Lobby
// Choose to create or join a session given a selected scene
// Create a new scene to be used as the Scene Session
// Copy all scene info from Server Scene on init
// While in init, map Client Objects to Server Objects
// Make sure to iterate over all objects to copy Server Objects
// During Update, receive Locked List from Server
// During Update, if selected object is on locked list, unselect
// During Update, send selected server object ID's to server to lock them
// During Update, send modified object info to server
// During Update, receive object modifications from server
// During Update, 
// Leaving Session, close Scene Session and load a blank scene

// Server:

// Init:
//   Spawn Server
//   Start Lobby
// Update:
//   Listen for Client traffic, parse queued packets
// Packet Manager:
//   Switch(packet)
//     If Client starts/joins a Session:
//       If start:
//         Open scene in multi-scene editing
//       Map Client List ID to Scene Session ID
//       Send Client the current Scene data to copy
//     If Client asks for Server Object data:
//       Check if Client is in Scene Session
//       Send Client Server Object data
//     If Client sends Selected Objects:
//       Check if Selected Objects are already in Locked List
//         If a selected object is already in Locked List:
//           Send Client the Server Objects data to reset their Client Object
//         If not already in Locked List:
//           Map Server Object ID to Client ID
//     
//

// 
// During Update, if locked list was changed, send to clients
// 
// 
// 
// 

// 1. Link Client to Server
// 2. Select a Scene, for this instance Sever is Scene 1 and Client is Scene 2
// 3. 

/*
class EditorNetworkManager
{
    // Vars


    // Methods
    public EditorNetworkManager()
    {

    }

    public void Update()
    {

    }

}

class EditorSessionManagerServer
{
    // Vars
    Dictionary<int, SessionInfo> sessions;

    // Methods
    public EditorSessionManagerServer()
    {

    }

    public SessionID StartSession(Scene s, )
    {
        return AssetDatabase.AssetPathToGUID(s.path);
    }

    public void SetSessionAsActive(int s_ID)
    {
        //session_status[s_ID] = true;
    }

    public void SetSessionAsInactive(int s_ID)
    {
        //session_status[s_ID] = false;
    }
    
    public List<int> GetActiveSessions()
    {
        List<int> active = new List<int>();

        foreach(KeyValuePair<int, bool> pair in session_status)
        {
            if (pair.Value == true)
            {
                //active.Add()
            }
        }

        return active;
    }

    public List<int> GetInactiveSessions()
    {
    
    }
    
}*/

/*
internal enum MessageType
{
    Msg_JoinSession,
    Msg_LeaveSession,
    Msg_LockObject,
    Msg_UnlockObject,
    Msg_SyncObject
};*/

/*
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
    
    public Session CreateSessionClient(string name, UnityEditor.SceneManagement.NewSceneMode mode = NewSceneMode.Single)
    {
        Session session = new Session() { m_scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, mode) };

        //EditorSceneManager.SaveScene(session.m_scene);

        //AssetDatabase.RenameAsset(session.m_scene.path, name);

        //EditorSceneManager.OpenScene(session.m_scene.path, OpenSceneMode.Single);

        m_session_list.Clear();
        m_session_list.Add(session);

        return session;
    }

    public Session CreateSession(string scene_path, UnityEditor.SceneManagement.OpenSceneMode mode = OpenSceneMode.Single)
    {
        Session session = new Session { m_scene = EditorSceneManager.OpenScene(scene_path, mode) };

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
        return m_session_list.Find(x => x.m_scene.name == name);
    }
    
    public bool StartSession(int session_id)
    {

    }

    public bool CloseSession(int session_id)
    {

    }

    public void AddClientToSession(string scene_name, ClientInfo client)
    {
        Session s = m_session_list.Find(x => x.m_scene.name == scene_name);
        s.m_client_ids.Add(client);
    }

    public void RemoveClientFromSession(string scene_name, ClientInfo client)
    {
        Session s = m_session_list.Find(x => x.m_scene.name == scene_name);
        s.m_client_ids.Remove(client);
    }

    public int GetClientSize(string scene_name)
    {
        //Session s = m_session_list.Find(x => x.m_scene.name == scene_name);

        //if ()

        return m_session_list.Find(x => x.m_scene.name == scene_name).m_client_ids.Count;
    }

    public bool SetLockedObject(int object_id, ClientInfo client)
    {
        return false;
    }

    public bool SetUnlockedObject(int object_id)
    {
        return false;
    }

    public bool IsObjectLocked(int object_id)
    {
        return false;
    }

    public bool DoesClientOwnObject(int object_id, ClientInfo client)
    {
        return false;
    }

    public bool Exists(string scene_name)
    {
        return m_session_list.Exists(x => x.m_scene.name == scene_name);
    }

}*/
/*
class EditorSceneManagerServer
{
    // Vars
    Dictionary<string, string> m_guids_to_path = new Dictionary<string, string>();
    Dictionary<string, string> m_name_to_path = new Dictionary<string, string>();
    SessionManager m_session_mgr = new SessionManager();

    Queue<JoinMessage> m_join_messages = new Queue<JoinMessage>();
    Queue<LeaveMessage> m_leave_messages = new Queue<LeaveMessage>();

    // Methods
    public void Init()
    {
        InitScenes();



    }

    public void Update()
    {
        // Check for Join/Leave Session
        while (m_join_messages.Count > 0)
        {
            JoinMessage msg = m_join_messages.Dequeue();

            if (!m_session_mgr.Exists(msg.scene_name))
            {
                // CreateSession
                m_session_mgr.CreateSession(m_name_to_path[msg.scene_name], OpenSceneMode.Additive);
            }

            m_session_mgr.AddClientToSession(msg.scene_name, msg.client);

        }

        while (m_leave_messages.Count > 0)
        {
            LeaveMessage msg = m_leave_messages.Dequeue();

            m_session_mgr.RemoveClientFromSession(msg.scene_name, msg.client);

            if (!(m_session_mgr.GetClientSize(msg.scene_name) > 0))
            {
                // Save Scene
                //EditorSceneManager.Save
                // DestroySession
                m_session_mgr.DestroySession(m_session_mgr.GetSessionBySceneName(msg.scene_name));
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

        foreach (string s in GUIDs)
        {
            m_guids_to_path[s] = AssetDatabase.GUIDToAssetPath(s);
            m_name_to_path[Path.GetFileNameWithoutExtension(m_guids_to_path[s])] = m_guids_to_path[s];
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
        string[] scene_names = new string[sessions.Count];

        int i = 0;

        foreach (KeyValuePair<string, SessionInfo> pair in sessions)
        {
            scene_names[i] = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(pair.Key));
        }

        return scene_names;
    }

    public void SendJoinMsg(EditorSceneManagerClient client, JoinMessage msg)
    {
        client.ReceiveJoinMsg(msg);
    }

    public void ReceiveJoinMsg(JoinMessage msg)
    {
        m_join_messages.Enqueue(msg);
    }

    public void SendLeaveMsg(EditorSceneManagerClient client, LeaveMessage msg)
    {
        client.ReceiveLeaveMsg(msg);
    }

    public void ReceiveLeaveMsg(LeaveMessage msg)
    {
        m_leave_messages.Enqueue(msg);
    }

}

class EditorSceneManagerClient
{
    // Vars
    public int client_id;
    public string client_name;
    SessionManager m_session_mgr = new SessionManager();

    Queue<JoinMessage> m_join_messages = new Queue<JoinMessage>();
    Queue<LeaveMessage> m_leave_messages = new Queue<LeaveMessage>();

    // Methods

    public void Init()
    {
        client_id = 1;  // Set to 1 for SceneLinker example
        client_name = "Client 1";
    }

    public void Update()
    {
        // Check for Join/Leave Session
        while (m_join_messages.Count > 0)
        {
            JoinMessage msg = m_join_messages.Dequeue();

            m_session_mgr.AddClientToSession(msg.scene_name, msg.client);

        }

        while (m_leave_messages.Count > 0)
        {
            LeaveMessage msg = m_leave_messages.Dequeue();

            m_session_mgr.RemoveClientFromSession(msg.scene_name, msg.client);
        }

        // Check for Locked/Unlocked Object(s)

        // Check for SyncObjects

        // If any of those checked, update log

        m_session_mgr.Update();
    }

    public void Close()
    {

    }

    public void SendJoinMsg(EditorSceneManagerServer server, JoinMessage msg)
    {
        m_session_mgr.CreateSession("Assets/Session.unity");
        server.ReceiveJoinMsg(msg);
    }

    public void ReceiveJoinMsg(JoinMessage msg)
    {
        msg.scene_name = "Session";
        m_join_messages.Enqueue(msg);
    }

    public void SendLeaveMsg(EditorSceneManagerServer server, LeaveMessage msg)
    {
        m_session_mgr.DestroySession(m_session_mgr.GetSessionBySceneName(msg.scene_name));
        server.ReceiveLeaveMsg(msg);
    }

    public void ReceiveLeaveMsg(LeaveMessage msg)
    {
        msg.scene_name = "Session";
        m_leave_messages.Enqueue(msg);
    }

}*/
/*
class EditorSceneLinker
{
    // Vars
    string SceneServerFile;
    //string SceneDestinationFile;

    string Status = "Unlinked";
    bool isLinked = false;

    bool sc1_exists = false;
    //bool sc2_exists = false;

    //Scene sc1;
    //Scene sc2;

    SceneManagerServer Server = new SceneManagerServer();
    SceneManagerClient Client = new SceneManagerClient();

    // Methods

    public void Init()
    {
        Undo.postprocessModifications += OnPostProcessModifications;
        Server.Init();
        Client.Init();
    }

    public void Update()
    {
        if (isLinked)
        {
            Client.Update();
            Server.Update();

            //GameObject[] obj = Selection.gameObjects;
            //int[] obj_id = Selection.instanceIDs;

            //GameObject[] sc1_objs = sc1.GetRootGameObjects();
            //GameObject[] sc2_objs = sc2.GetRootGameObjects();

            //int size1 = sc1_objs.Length;
            //int size2 = sc2_objs.Length;

            

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

        //GUILayout.Label("Link Destination", EditorStyles.boldLabel);
        //SceneDestinationFile = EditorGUILayout.TextField("Scene Name:", SceneDestinationFile);

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
            sc1_exists = File.Exists("Assets/" + SceneServerFile + ".unity");
            //sc2_exists = File.Exists("Assets/" + SceneDestinationFile + ".unity");

            if (!sc1_exists)
            {
                Status = "Invalid scene name!";
            }
            //else if (!sc2_exists)
            //{
            //    Status = "Invalid Destination scene name!";
            //}
            else
            {
                JoinMessage msg = new JoinMessage();
                ClientInfo c_info = new ClientInfo();
                c_info.client_id = Client.client_id;
                c_info.m_name = Client.client_name;
                msg.client = c_info;
                msg.scene_name = SceneServerFile;

                Client.SendJoinMsg(Server, msg);




                
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
            LeaveMessage msg = new LeaveMessage();
            ClientInfo c_info = new ClientInfo();
            c_info.client_id = Client.client_id;
            c_info.m_name = Client.client_name;
            msg.client = c_info;
            msg.scene_name = SceneServerFile;

            Client.SendLeaveMsg(Server, msg);

            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            isLinked = false;
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
                      " ObjectID: " + mod.currentValue.target.GetInstanceID() +
                      " Component: " + EditorUtility.InstanceIDToObject(mod.currentValue.target.GetInstanceID()).GetType().Name);
        }
        return propertyModifications;
    }
}*/


class SceneLinkerWindow : EditorWindow
{
    public SceneLinker Scene_Linker = new SceneLinker();
    /*
    [MenuItem("Scene Manager/Linker Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SceneLinkerWindow), false, "Scene Linker", true);
    }*/

    public void OnEnable()
    {
        Scene_Linker.Init();
    }

    public void OnDisable()
    {
        Scene_Linker.Close();
    }

    private void OnGUI()
    {
        Scene_Linker.Display();
    }

    private void Update()
    {
        Scene_Linker.Update();
    }
}
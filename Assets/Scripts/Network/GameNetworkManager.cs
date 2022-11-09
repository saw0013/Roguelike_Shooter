using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

public class GameNetworkManager : NetworkManager
{
    // Overrides the base singleton so we don't
    // have to cast to this type everywhere.
    public static new GameNetworkManager singleton { get; private set; }

    #region Variables

    [SerializeField, Scene, Tooltip("Сцена, на которую вы хотите перейти при присоединении к серверу. " +
        "Эта сцена будет загружена (additively) дополнительно, если она еще не загружена.")]
    protected string moveToSceneOnJoin;

    [SerializeField, Tooltip("Имя объекта также должно быть помечено \"NetworkSpawnPoint\"," +
        " к которому вы хотите перейти после присоединения к серверу и загрузки сцен. " +
        "Этот конкретный прыжок игнорирует настройки команды. Так что эта точка должна быть уникальной и частью одной из загруженных сцен.")]
    protected string jumpToPointName = "";

    protected Dictionary<NetworkConnection, GameObject> _playerPrefabList = new Dictionary<NetworkConnection, GameObject>();
    protected bool _spawnedPlayerPrefab = false;

    [SerializeField, Tooltip("Если вы хотите, чтобы ваш персонаж автоматически спавнился при подключении к серверу")]
    protected bool autoSpawnCharacter = false;

    [Tooltip("Объект, который будет появляться, когда ваш собственный клиент вызывает вашего персонажа.")]
    public GameObject characterToSpawn = null;

    #endregion

    #region Unity Callbacks

    public override void OnValidate()
    {
        base.OnValidate();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// Networking is NOT initialized when this fires
    /// </summary>
    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// Networking is NOT initialized when this fires
    /// </summary>
    public override void Start()
    {
        singleton = this;
        base.Start();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// </summary>
    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    /// <summary>
    /// Runs on both Server and Client
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion

    #region Start & Stop

    /// <summary>
    /// Set the frame rate for a headless server.
    /// <para>Override if you wish to disable the behavior or set your own tick rate.</para>
    /// </summary>
    public override void ConfigureHeadlessFrameRate()
    {
        base.ConfigureHeadlessFrameRate();
    }

    /// <summary>
    /// called when quitting the application by closing the window / pressing stop in the editor
    /// </summary>
    public override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
    }

    #endregion

    #region Scene Management

    /// <summary>
    /// This causes the server to switch scenes and sets the networkSceneName.
    /// <para>Clients that connect to this server will automatically switch to this scene. This is called automatically if onlineScene or offlineScene are set, but it can be called from user code to switch scenes again while the game is in progress. This automatically sets clients to be not-ready. The clients must call NetworkClient.Ready() again to participate in the new scene.</para>
    /// </summary>
    /// <param name="newSceneName"></param>
    public override void ServerChangeScene(string newSceneName)
    {
        base.ServerChangeScene(newSceneName);
    }

    /// <summary>
    /// Called from ServerChangeScene immediately before SceneManager.LoadSceneAsync is executed
    /// <para>This allows server to do work / cleanup / prep before the scene changes.</para>
    /// </summary>
    /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
    public override void OnServerChangeScene(string newSceneName) { }

    /// <summary>
    /// Called on the server when a scene is completed loaded, when the scene load was initiated by the server with ServerChangeScene().
    /// </summary>
    /// <param name="sceneName">The name of the new scene.</param>
    public override void OnServerSceneChanged(string sceneName) { }

    /// <summary>
    /// Called from ClientChangeScene immediately before SceneManager.LoadSceneAsync is executed
    /// <para>This allows client to do work / cleanup / prep before the scene changes.</para>
    /// </summary>
    /// <param name="newSceneName">Name of the scene that's about to be loaded</param>
    /// <param name="sceneOperation">Scene operation that's about to happen</param>
    /// <param name="customHandling">true to indicate that scene loading will be handled through overrides</param>
    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling) { }

    /// <summary>
    /// Called on clients when a scene has completed loaded, when the scene load was initiated by the server.
    /// <para>Scene changes can cause player objects to be destroyed. The default implementation of OnClientSceneChanged in the NetworkManager is to add a player object for the connection if no player object exists.</para>
    /// </summary>
    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();
    }

    #endregion

    #region Server System Callbacks

    /// <summary>
    /// Called on the server when a new client connects.
    /// <para>Unity calls this on the Server when a Client connects to the Server. Use an override to tell the NetworkManager what to do when a client connects to the server.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {

    }

    /// <summary>
    /// Called on the server when a client is ready.
    /// <para>The default implementation of this function calls NetworkServer.SetClientReady() to continue the network setup process.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
    }

    /// <summary>
    /// Called on the server when a client adds a new player with ClientScene.AddPlayer.
    /// <para>The default implementation for this function creates a new player object from the playerPrefab.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        if (numPlayers > 1)
        {

        }
    }

    /// <summary>
    /// Called on the server when a client disconnects.
    /// <para>This is called on the Server when a Client disconnects from the Server. Use an override to decide what should happen when a disconnection is detected.</para>
    /// </summary>
    /// <param name="conn">Connection from client.</param>
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
    }

    /// <summary>
    /// Called on server when transport raises an exception.
    /// <para>NetworkConnection may be null.</para>
    /// </summary>
    /// <param name="conn">Connection of the client...may be null</param>
    /// <param name="exception">Exception thrown from the Transport.</param>
    public override void OnServerError(NetworkConnectionToClient conn, TransportError transportError, string message) { }

    #endregion

    #region Client System Callbacks

    /// <summary>
    /// Called on the client when connected to a server.
    /// <para>The default implementation of this function sets the client as ready and adds a player. Override the function to dictate what happens when the client connects.</para>
    /// </summary>
    public override void OnClientConnect()
    {
        base.OnClientConnect();
    }

    /// <summary>
    /// Called on clients when disconnected from a server.
    /// <para>This is called on the client when it disconnects from the server. Override this function to decide what happens when the client disconnects.</para>
    /// </summary>
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
    }

    /// <summary>
    /// Called on clients when a servers tells the client it is no longer ready.
    /// <para>This is commonly used when switching scenes.</para>
    /// </summary>
    public override void OnClientNotReady() { }

    /// <summary>
    /// Called on client when transport raises an exception.</summary>
    /// </summary>
    /// <param name="exception">Exception thrown from the Transport.</param>
    public override void OnClientError(TransportError transportError, string message) { }

    #endregion

    #region Start & Stop Callbacks

    // Since there are multiple versions of StartServer, StartClient and StartHost, to reliably customize
    // their functionality, users would need override all the versions. Instead these callbacks are invoked
    // from all versions, so users only need to implement this one case.

    /// <summary>
    /// This is invoked when a host is started.
    /// <para>StartHost has multiple signatures, but they all cause this hook to be called.</para>
    /// </summary>
    public override void OnStartHost() { }

    /// <summary>
    /// This is invoked when a server is started - including when a host is started.
    /// <para>StartServer has multiple signatures, but they all cause this hook to be called.</para>
    /// </summary>
    public override void OnStartServer()
    {
        //Регистрируем события
        NetworkServer.RegisterHandler<JumpToScene>(ClientRequestedJumpToScene);
        NetworkServer.RegisterHandler<SpawnPlayer>(SpawnPlayer);
        NetworkServer.RegisterHandler<FinishedMoving>(PlayerFinishedSceneMove);
        base.OnStartServer();
    }

    /// <summary>
    /// This is invoked when the client is started.
    /// </summary>
    public override void OnStartClient() 
    {
        NetworkClient.RegisterHandler<ClientJumpToScene>(JumpToScene);
        base.OnStartClient();
    }

    /// <summary>
    /// This is called when a host is stopped.
    /// </summary>
    public override void OnStopHost() { }

    /// <summary>
    /// This is called when a server is stopped - including when a host is stopped.
    /// </summary>
    public override void OnStopServer()
    {
        NetworkServer.UnregisterHandler<JumpToScene>();
        NetworkServer.UnregisterHandler<SpawnPlayer>();
        NetworkServer.UnregisterHandler<FinishedMoving>();
        base.OnStopServer();
    }

    /// <summary>
    /// This is called when a client is stopped.
    /// </summary>
    public override void OnStopClient() 
    {
        NetworkClient.UnregisterHandler<ClientJumpToScene>();
        base.OnStopClient();
    }

    #endregion

    #region Messages



    #endregion

    #region Player Spawning

    /// <summary>
    /// Это будет вызываться созданным вами персонажем PlayerMovement, которым вы владеете.
    /// Это позволяет вам узнать, нужно ли вам запрашивать повторный вызов или нет. Это только для
    /// первое подключение клиентов. Каждое появление после этого не имеет значения.
    /// </summary>
    [Client]
    public virtual void MarkInitialSpawnCompleted()
    {
        _spawnedPlayerPrefab = true;
    }

    /// <summary>
    /// Этот клиент попросит сервер создать для него нового персонажа.
    /// </summary>
    /// <param name="prefabName">Имя создаваемого префаба</param>
    /// <param name="pointType">Тип точки создания префаба</param>
    /// <param name="sceneName">Имя сцены, в которую нужно переместить этот префаб</param>
    /// <param name="pointName">Имя точки в группе pointType, к которой вы хотите переместить этот префаб.</param>
    /// <param name="unloadScene">Имя сцены для выгрузки</param>
    [Client]
    public virtual void RequestSpawnCharacter(string prefabName, SpawnUtils.PointType pointType, string sceneName = "", string pointName = "", string unloadScene = "")
    {
        Debug.Log($"<color=green>Client</color> sending <color=purple>SpawnPlayer</color> request to server with data: <color=magenta>[PrefabName: {prefabName}, PointType: {pointType}, SceneName: {sceneName}, PointName: {pointName},]</color>");
        NetworkClient.Send(new SpawnPlayer
        {
            prefabName = prefabName,
            pointType = pointType,
            sceneName = sceneName,
            pointName = pointName,
            unloadScene = unloadScene
        });
    }

    /// <summary>
    /// Это вызывается, когда клиент хочет создать игрока
    /// </summary>
    /// <param name="conn">Networkconnection клиента, отправляющего запрос</param>
    /// <param name="data">Данные для порождения префаба</param>
    [Server]
    protected virtual void SpawnPlayer(NetworkConnection conn, SpawnPlayer data)
    {
        StartCoroutine(E_SpawnPlayer(conn, data));
    }

    protected virtual IEnumerator E_SpawnPlayer(NetworkConnection conn, SpawnPlayer data)
    {
        //#if UNITY_SERVER || UNITY_EDITOR
        Debug.Log($"<color=blue>Server</color> received <color=purple>SpawnPlayer</color> request from <color=green>[connectionId={conn.connectionId}]</color> with data: <color=magenta>[PrefabName: {data.prefabName}, PointType: {data.pointType}, SceneName: {data.sceneName}, PointName: {data.pointName},]</color>");

        GameObject prefab = spawnPrefabs.Find(x => x.name == data.prefabName);
        if (prefab)
        {
            Debug.Log($"<color=blue>Server</color> found target spawnable prefab in list: <color=magenta>{prefab}</color>");

            GameObject point = SpawnUtils.GetPoint(data.pointType, data.pointName, data.sceneName);
            GameObject character = null;
            if (point)
            {
                Debug.Log($"<color=blue>Server</color> locally spawning: <color=magenta>{data.prefabName}</color> in scene <color=magenta>{SceneManager.GetActiveScene().name}</color> at found point.");
                character = Instantiate(prefab, point.transform.position, point.transform.rotation);
            }
            else
            {
                Debug.Log($"<color=blue>Server</color> locally spawning: <color=magenta>{data.prefabName}</color> at Vector3.zero in scene <color=magenta>{SceneManager.GetActiveScene().name}</color> because no point was found.");
                character = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            }

            if (_playerPrefabList.ContainsKey(conn))
            {
                Debug.Log($"<color=blue>Server</color> destroying: <color=magenta>{_playerPrefabList[conn].name}</color> from the network.");
                Destroy(_playerPrefabList[conn]);
                yield return new WaitForSeconds(0.001f); // wait for destroy to propigate across the network
            }

            Debug.Log($"<color=blue>Server</color> spawning: <color=magenta>{character.name}</color> across the network.");
            NetworkServer.Spawn(character, conn);    // instantiates this character across the network and makes it owned by the conn
            yield return new WaitForSeconds(0.001f); // wait for the spawn request to settle across the network

            ConnectionPlayer clientConn = ClientUtils.GetClientConnection(conn.connectionId);
            if (clientConn)
            {
                clientConn.playerCharacter = character;
            }

            if (_playerPrefabList.ContainsKey(conn))
            {
                _playerPrefabList[conn] = character;
            }
            else
            {
                // первый раз, когда игрок был создан для этого соединения.
                _playerPrefabList.Add(conn, character);

                Debug.Log($"<color=blue>Server</color> triggering <color=purple>JumpToScene</color> like it was called from <color=green>[connectionId={conn.connectionId}]</color> with data: <color=magenta>[PointType: {data.pointType}, PointName: {data.pointName}, SceneName: {data.sceneName}]</color>");

                ClientRequestedJumpToScene(conn, new JumpToScene
                {
                    pointType = data.pointType,
                    sceneName = data.sceneName,
                    pointName = data.pointName,
                    unloadScene = data.unloadScene
                });
            }
        }
        else
        {
            Debug.LogError($"<color=blue>Server</color> failed to spawn: <color=magenta>{data.prefabName}</color> because this name was not found in the \"Registered Spawnable Prefabs\"");
        }

        yield return null;
    }

    #endregion

    #region Scene Managment

    public void RequestJumpToScene(SpawnUtils.PointType pointType)
    {
        RequestJumpToScene(pointType, moveToSceneOnJoin, jumpToPointName, onlineScene = "Lobby");
    }

    /// <summary>
    /// Клиент отправляет серверу запрос на переход к определенной сцене с запрошенными данными
    /// </summary>
    /// <param name="sceneName"></param>
    [Client]
    public virtual void RequestJumpToScene(SpawnUtils.PointType pointType, string sceneName, string pointName, string unloadScene = "")
    {
        Debug.Log($"<color=green>Client</color> sending <color=purple>JumpToScene</color> request with: <color=magenta>[PointType: {pointType},  PointName: {pointName}]</color>");
        NetworkClient.Send(new JumpToScene
        {
            pointType = pointType,
            sceneName = sceneName.GetCleanSceneName(), //Сцена для прыжка
            pointName = pointName, //Имя точки в новой сцене для перехода
            unloadScene = unloadScene
        });
    }

    /// <summary>
    /// Сервер получит это, когда клиент запрашивает переход к сцене.
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="data"></param>
    [Server]
    public virtual void ClientRequestedJumpToScene(NetworkConnection conn, JumpToScene data)
    {
        Debug.Log($"<color=blue>Server</color> received <color=purple>JumpToScene</color> request from client <color=green>" +
            $"[connectionId={conn.connectionId}]</color> with data: <color=magenta>" +
            $"[PointType: {data.pointType}, " +
            $"PointName: {data.pointName}, " +
            $"SceneName: {data.sceneName}, " +
            $"UnloadScene: {data.unloadScene}]</color>");
        StartCoroutine(ServerLoadScene(data.pointType, data.sceneName, data.pointName, conn, data.unloadScene));
    }

    /// <summary>
    /// Метод только для сервера — будет дополнительно загружать сцену и сообщать NetworkConn, чтобы она также загружалась..
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="pointName"></param>
    /// <param name="teamName"></param>
    /// <param name="conn"></param>
    /// <returns></returns>
    [Server]
    protected virtual IEnumerator ServerLoadScene(SpawnUtils.PointType pointType, string sceneName, string pointName, NetworkConnection conn = null, string unloadScene = "")
    {
        if (SceneManager.GetSceneByName(sceneName).name.GetCleanSceneName() != sceneName && !SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.Log($"<color=blue>Server</color> loading scene: <color=magenta>{sceneName}</color>");
            AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!loadScene.isDone)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.001f);

            Debug.Log($"<color=blue>Server</color> finished loading scene: <color=magenta>{sceneName}</color>");
        }
        if (conn != null)
        {
            yield return new WaitForSeconds(0.001f); // Wait for the scene to finialize before telling people to move to it.

            //var cc = playerPrefab.GetComponent<ConnectionPlayer>();
            ConnectionPlayer cp = ClientUtils.GetClientConnection(conn.connectionId);

            if (cp.playerCharacter != null)
            {
                GameObject foundPoint = SpawnUtils.GetPoint(pointType, pointName, sceneName.GetCleanSceneName());

                Debug.Log($"<color=blue>Server</color> jumping target player <color=magenta>{cp.playerCharacter}</color> to scene: <color=magenta>{sceneName.GetCleanSceneName()}</color>");


                if (foundPoint == null)
                {
                    Debug.Log($"<color=blue>Server</color> no target point found, jumping character to <color=magenta>Vector3.zero</color>");
                    cp.playerCharacter.transform.position = Vector3.zero;
                }
                else
                {
                    Debug.Log($"<color=blue>Server</color> jumping character to point: <color=magenta>{foundPoint}</color>");
                    cp.playerCharacter.transform.position = foundPoint.transform.position;
                    cp.playerCharacter.transform.rotation = foundPoint.transform.rotation;
                }
            }

            Debug.Log($"<color=blue>Server</color> not target player found for connectionID: <color=magenta>{conn.connectionId}</color>, " +
                $"skipping character scene moving.");
            Debug.Log($"<color=blue>Server</color> sending <color=purple>ClientJumpToScene</color> to client <color=green>" +
                $"[connectionId={conn.connectionId}]</color> with data: <color=magenta>" +
                $"[PointType: {pointType}, " +
                $"PointName: {pointName}, " +
                $"SceneName: {sceneName}, " +
                $"ConnectionId: {conn.connectionId}, " +
                $"UnloadScene: {unloadScene}]</color>");

            conn.Send(new ClientJumpToScene
            {
                pointType = pointType,
                sceneName = sceneName,
                pointName = pointName,
                connectionId = conn.connectionId,
                unloadScene = unloadScene
            });
        }
    }

    /// <summary>
    /// Это вызывается на сервере от клиента, когда он закончил перемещать своего персонажа в новую сцену.
    /// </summary>
    [Server]
    public virtual void PlayerFinishedSceneMove(NetworkConnection conn, FinishedMoving data)
    {
        ConnectionPlayer targetConn = ClientUtils.GetClientConnection(conn.connectionId);
        if (targetConn.playerCharacter != null && targetConn.playerCharacter.scene.name.GetCleanSceneName() != data.sceneName.GetCleanSceneName())
        {
            Debug.Log("<color=blue>Server</color> received client finished moving and the client object on the server isn't on the right scene, moving them to the correct scene.");
            SceneManager.MoveGameObjectToScene(targetConn.playerCharacter, SceneManager.GetSceneByName(data.sceneName.GetCleanSceneName()));
        }

        Debug.Log("<color=blue>Server</color> received client finished moving but no playerCharacter was assigned to this client connection. Not doing anything.");

    }

    [Server]
    public virtual void UnloadIfEmpty(string sceneName)
    {
        Debug.Log($"Check if scene: {sceneName.GetCleanSceneName()} can be unloaded.");
        foreach (ConnectionPlayer conn in FindObjectsOfType<ConnectionPlayer>().ToList())
        {
            if (conn.inScene.GetCleanSceneName() == sceneName.GetCleanSceneName()) return;
            
        }
        Debug.Log($"<color=blue>Server</color> Unloading scene: <color=magenta>{sceneName.GetCleanSceneName()}</color>");
        SceneManager.UnloadSceneAsync(sceneName.GetCleanSceneName());
    }

    /// <summary>
    /// Это ответ сервера на запрос клиента о переходе к сцене.
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="data"></param>
    [Client]
    protected virtual void JumpToScene(ClientJumpToScene data)
    {
        Debug.Log($"<color=green>Client</color> received <color=purple>ClientJumpToScene</color> request from the server with: <color=magenta>[PointType: {data.pointType}, SceneName: {data.sceneName.GetCleanSceneName()}, PointName: {data.pointName}, ConnectionId: {data.connectionId}, UnloadScene: {data.unloadScene}]</color>");
        StartCoroutine(JumpToScene(data.pointType, data.sceneName.GetCleanSceneName(), data.pointName, data.connectionId, data.unloadScene));
    }

    [Client]
    protected virtual IEnumerator JumpToScene(SpawnUtils.PointType pointType, string sceneName, string pointName, int connectionId, string unloadScene)
    {
        if (SceneManager.GetSceneByName(sceneName).name.GetCleanSceneName() != sceneName.GetCleanSceneName() && !SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.Log($"<color=green>Client</color> loading scene: <color=magenta>{sceneName}</color>");
            
            AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!loadScene.isDone)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.001f);
            
            Debug.Log($"<color=green>Client</color> finished loading scene: <color=magenta>{sceneName}</color>");
        }


        yield return new WaitForSeconds(0.001f); // Подождите, пока сцена завершит переключение.
        GameObject targetPlayer = GameObject.FindGameObjectsWithTag("Player").ToList().Find(x =>
            x.GetComponent<ConnectionPlayer>().connId == connectionId
        );
        if (targetPlayer)
        {
            Debug.Log($"<color=green>Client</color> moving target player: <color=magenta>{targetPlayer}</color> to scene: <color=magenta>{sceneName.GetCleanSceneName()}</color>");
            
            SceneManager.MoveGameObjectToScene(targetPlayer, SceneManager.GetSceneByName(sceneName.GetCleanSceneName()));
           
            Debug.Log($"<color=green>Client</color> looking for target point - TYPE: <color=magenta>{pointType}</color>, NAME: <color=magenta>{pointName}</color>, INSCENE: <color=magenta>{sceneName}</color>");
           
            GameObject targetPoint = SpawnUtils.GetPoint(pointType, pointName, sceneName);
            if (targetPoint != null)
            {
                Debug.Log($"<color=green>Client</color> jumping target player: <color=magenta>{targetPlayer}</color> to target point: <color=magenta>{targetPoint}</color>");
                // точка найдена прыгаем к ней игроком
                targetPlayer.transform.position = targetPoint.transform.position;
                targetPlayer.transform.rotation = targetPoint.transform.rotation;
            }
            else
            {
                Debug.Log($"<color=green>Client</color> jumping target player: <color=magenta>{targetPlayer}</color> to Vector3.zero since no target point was found.");
                // Точка не найдена, просто сбросьте игрока
                targetPlayer.transform.position = Vector3.zero;
                targetPlayer.transform.rotation = Quaternion.identity;
            }
        }
       
            Debug.Log($"<color=green>Client</color> failed to find any target player tagged with \"Player\" with [connectionId={connectionId}], skipping move targetPlayer method.");
        

        if (!_spawnedPlayerPrefab && autoSpawnCharacter)
        {
            // Если ваш персонаж еще не был создан, создайте его в новой сцене.
            RequestSpawnCharacter(
                prefabName: characterToSpawn.name,
                pointType: SpawnUtils.PointType.NetworkSpawnPoint,
                sceneName: sceneName,
                pointName: pointName,
                unloadScene: unloadScene
            );
        }
        else
        {
            // Персонаж закончил появляться, теперь убедитесь, что он находится в нужной сцене.
            NetworkClient.Send(new FinishedMoving
            {
                sceneName = sceneName
            });
            if (!string.IsNullOrEmpty(unloadScene))
            {
                try
                {
                    if (!NetworkServer.active)
                    {
                        Debug.Log($"<color=green>Client</color> unload scene: <color=magenta>{unloadScene}</color>");
                        SceneManager.UnloadSceneAsync(unloadScene);
                    }
                    
                    else
                    {
                        UnloadIfEmpty(unloadScene);
                    }
                    
                }
                catch { }
            }
        }

        yield return null;
    }


    #endregion
}

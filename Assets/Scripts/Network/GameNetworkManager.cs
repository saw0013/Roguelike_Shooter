using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror.Examples.MultipleAdditiveScenes;
using UnityEngine.ProBuilder.Shapes;
using Random = System.Random;

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

    [Header("Scenes Count")]
    public int instances = 3;

    [SerializeField, Scene, Tooltip("Сцена которая будет спавниться только на сервере. Клиент будет подсоединяться к свободной.")]
    private string gameScene;

    // Это устанавливается после того, как сервер загружает все экземпляры подсцены.
    bool subscenesLoaded;

    // подсцены добавляются в этот список по мере их загрузки
    public readonly List<Scene> subScenes = new List<Scene>();

    // Последовательный индекс, используемый при круговом распределении игроков по экземплярам и позиционировании очков
    int clientIndex;

    [Header("Setup Spawn"), Tooltip("")]
    public GameObject player;

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
        //if (newSceneName == subScenes[1].name)
        //{
        //    
        //}

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
        if (numPlayers > 1)
        {
        }

        StartCoroutine(OnServerAddPlayerDelayed(conn));
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
        StartCoroutine(ServerLoadSubScenes());
        base.OnStartServer();
    }

    /// <summary>
    /// This is invoked when the client is started.
    /// </summary>
    public override void OnStartClient() 
    {
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
        NetworkServer.SendToAll(new SceneMessage { sceneName = gameScene, sceneOperation = SceneOperation.UnloadAdditive });
        StartCoroutine(ServerUnloadSubScenes());
        clientIndex = 0;
        base.OnStopServer();
    }

    /// <summary>
    /// This is called when a client is stopped.
    /// </summary>
    public override void OnStopClient() 
    {
        // Убидимся, что мы не в режиме хоста
        if (mode == NetworkManagerMode.ClientOnly)
            StartCoroutine(ClientUnloadSubScenes());

        base.OnStopClient();
    }

    #endregion

    #region Messages



    #endregion

    #region Клиентские методы

    #region OnServerAddPlayerDelayed

    // Эта задержка в основном связана с хост-плеером, который загружается слишком быстро для
    // сервер для асинхронной загрузки подсцен с OnStartServer перед ним.
    IEnumerator OnServerAddPlayerDelayed(NetworkConnectionToClient conn)
    {
        // ждем, пока сервер асинхронно загрузит все подсцены для экземпляров игры
        while (!subscenesLoaded)
            yield return null;

        // Отправляем клиенту сообщение о сцене для дополнительной загрузки игровой сцены
        conn.Send(new SceneMessage { sceneName = gameScene, sceneOperation = SceneOperation.LoadAdditive });

        // Дождаться конца кадра перед добавлением игрока, чтобы убедиться, что сообщение сцены идет первым
        yield return new WaitForEndOfFrame();

        base.OnServerAddPlayer(conn);

        //PlayerScore playerScore = conn.identity.GetComponent<PlayerScore>();
        //playerScore.playerNumber = clientIndex;
        //playerScore.scoreIndex = clientIndex / subScenes.Count;
        //playerScore.matchIndex = clientIndex % subScenes.Count;

        // Делаем это только на сервере, а не на клиентах
        // Это то, что позволяет выполнить проверку сетевой сцены на объектах игрока и сцены
        // чтобы изолировать совпадения для каждого экземпляра сцены на сервере.
        
        if (subScenes.Count > 0)
        {
            SceneManager.MoveGameObjectToScene(player, subScenes[1]);
        }

        //Каждого игрока будем спавнить в новой сцене
        clientIndex++;
    }

    #endregion

    #region ServerLoadSubScenes

    // Мы загружаем сцены аддитивно, поэтому GetSceneAt(0) вернет основную сцену-"контейнер",
    // поэтому мы начинаем индекс с еденицы и перебираем значения экземпляров включительно.
    // Если instances равно нулю, цикл полностью пропускается.
    IEnumerator ServerLoadSubScenes()
    {
        for (int index = 0; index <= instances; index++)
        {
            yield return SceneManager.LoadSceneAsync(gameScene, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics3D });

            Scene newScene = SceneManager.GetSceneAt(index);
            subScenes.Add(newScene);
            //Spawner.InitialSpawn(newScene);
            Debug.Log($"Индекс {index} | instances {instances}");
        }

        subscenesLoaded = true;
    }

    #endregion

    #region ServerUnloadSubScenes

    // Выгрузить подсцены и неиспользуемые ресурсы и очистим список подсцен.
    IEnumerator ServerUnloadSubScenes()
    {
        for (int index = 0; index < subScenes.Count; index++)
            yield return SceneManager.UnloadSceneAsync(subScenes[index]);

        subScenes.Clear();
        subscenesLoaded = false;

        yield return Resources.UnloadUnusedAssets();
    }

    #endregion

    #region ClientUnloadSubScenes

    // Выгружаем все, кроме активной сцены, которая является "контейнерной" сценой
    IEnumerator ClientUnloadSubScenes()
    {
        for (int index = 0; index < SceneManager.sceneCount; index++)
        {
            if (SceneManager.GetSceneAt(index) != SceneManager.GetActiveScene())
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(index));
        }
    }

    #endregion

    #endregion
}

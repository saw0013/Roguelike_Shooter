using Cinemachine;
using Mirror;
using MirrorBasics;
using Mono.CSharp;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Random = System.Random;


public class PlayerMovementAndLookNetwork : NetworkBehaviour
{
    #region Variables
    [Header("Panels")]
    [SerializeField] private GameObject _panelEscape;
    [SerializeField] private GameObject _panelSetting;
    [SerializeField] private GameObject _panelExit;
    [SerializeField] private GameObject _panelMain;
    [SerializeField] private GameObject _panelInfoItem;
    [SerializeField] private GameObject[] _panelsCanvas;

    //[Header("Camera")]
    //public Camera mainCamera;
    internal Camera mainCamera;


    [Header("Movement")]
    public Rigidbody playerRigidbody;
    private Vector3 inputDirection;
    private Vector3 movement;

    //Rotation

    private Plane playerMovementPlane;

    private RaycastHit floorRaycastHit;

    private Vector3 playerToMouse;

    [SerializeField] private PlayerData playerData;


    [Header("Animation")]
    public Animator playerAnimator;


    [Header("Audio VFX")]
    [SerializeField] private AudioSource _runPlayer;

    internal CinemachineVirtualCamera vCamera;

    [Header("Tool")]
    [SerializeField] private GameObject _healthBarRpcLookAt;

    #endregion

    #region Network Variables
    public static PlayerMovementAndLookNetwork localPlayer;

    [Space(20)]
    [SyncVar] public string matchID;
    [SyncVar] public int playerIndex;

    [SyncVar] public string UserName;

    [SyncVar] public int connId;

    [SyncVar] public string inScene = "";

    internal NetworkMatch networkMatch;

    public string MymatchID;

    [SyncVar] public Match currentMatch;

    [SerializeField] GameObject playerLobbyUI;

    Guid netIDGuid;

    #endregion

    #region Network singleton

    #region OVERRIDES BASE

    public override void OnStartServer()
    {
        netIDGuid = netId.ToString().ToGuid();
        networkMatch.matchId = netIDGuid;
    }

    public override void OnStartClient()
    {
        Debug.Log($"Client connected");
        if (isLocalPlayer)
        {
            localPlayer = this;
        }
        else
        {
            Debug.Log($"Spawning other player UI Prefab");
            playerLobbyUI = UILobby.instance.SpawnPlayerUIPrefab(this);
        }

        if (hasAuthority)
        {
            UserName = !string.IsNullOrWhiteSpace(PlayerPrefs.GetString("PlayerName")) ?
                PlayerPrefs.GetString("PlayerName") : $"Player{UnityEngine.Random.Range(1, 999999)}";
            GetComponent<PlayerData>().CmdShowName(UserName);
        }
    }

    public override void OnStopClient()
    {
        Debug.Log($"Client Stopped");
        ClientDisconnect();
    }

    public override void OnStopServer()
    {
        Debug.Log($"Client Stopped on Server");
        ServerDisconnect();
    }

    #endregion

    #region HOST MATCH

    public void HostGame(bool publicMatch)
    {
        string matchID = MatchMaker.GetRandomMatchID();
        CmdHostGame(matchID, publicMatch);
    }

    [Command]
    void CmdHostGame(string _matchID, bool publicMatch)
    {
        matchID = _matchID;
        if (MatchMaker.instance.HostGame(_matchID, this, publicMatch, out playerIndex))
        {
            Debug.Log($"<color=green>Game hosted successfully</color>");
            networkMatch.matchId = _matchID.ToGuid();
            TargetHostGame(true, _matchID, playerIndex);
        }
        else
        {
            Debug.Log($"<color=red>Game hosted failed</color>");
            TargetHostGame(false, _matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetHostGame(bool success, string _matchID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log($"MatchID: {matchID} == {_matchID}");
        UILobby.instance.HostSuccess(success, _matchID);
    }

    #endregion

    #region  JOIN MATCH

    public void JoinGame(string _inputID)
    {
        CmdJoinGame(_inputID);
    }

    [Command]
    void CmdJoinGame(string _matchID)
    {
        matchID = _matchID;
        if (MatchMaker.instance.JoinGame(_matchID, this, out playerIndex))
        {
            Debug.Log($"<color=green>Game Joined successfully</color>");
            networkMatch.matchId = _matchID.ToGuid();
            TargetJoinGame(true, _matchID, playerIndex);

            //Host
            if (isServer && playerLobbyUI != null)
            {
                playerLobbyUI.SetActive(true);
            }
        }
        else
        {
            Debug.Log($"<color=red>Game Joined failed</color>");
            TargetJoinGame(false, _matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetJoinGame(bool success, string _matchID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log($"MatchID: {matchID} == {_matchID}");
        UILobby.instance.JoinSuccess(success, _matchID);
    }

    #endregion

    #region DISCONNECT

    public void DisconnectGame()
    {
        CmdDisconnectGame();
    }

    [Command]
    void CmdDisconnectGame()
    {
        ServerDisconnect();
    }

    void ServerDisconnect()
    {
        MatchMaker.instance.PlayerDisconnected(this, matchID);
        RpcDisconnectGame();
        networkMatch.matchId = netIDGuid;
    }

    [ClientRpc]
    void RpcDisconnectGame()
    {
        ClientDisconnect();
    }

    void ClientDisconnect()
    {
        if (playerLobbyUI != null)
        {
            if (!isServer)
            {
                Destroy(playerLobbyUI);
            }
            else
            {
                playerLobbyUI.SetActive(false);
            }
        }
    }

    #endregion

    #region SEARCH MATCH

    public void SearchGame()
    {
        CmdSearchGame();
    }

    [Command]
    void CmdSearchGame()
    {
        if (MatchMaker.instance.SearchGame(this, out playerIndex, out matchID))
        {
            Debug.Log($"<color=green>Game Found Successfully</color>");
            networkMatch.matchId = matchID.ToGuid();
            TargetSearchGame(true, matchID, playerIndex);

            //Host
            if (isServer && playerLobbyUI != null)
            {
                playerLobbyUI.SetActive(true);
            }
        }
        else
        {
            Debug.Log($"<color=red>Game Search Failed</color>");
            TargetSearchGame(false, matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetSearchGame(bool success, string _matchID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log($"MatchID: {matchID} == {_matchID} | {success}");
        UILobby.instance.SearchGameSuccess(success, _matchID);
    }

    #endregion

    #region  MATCH PLAYERS

    [Server]
    public void PlayerCountUpdated(int playerCount)
    {
        TargetPlayerCountUpdated(playerCount);
    }

    /// <summary>
    /// Сколько игроков подключено к комнате
    /// </summary>
    /// <param name="playerCount"></param>
    [TargetRpc]
    void TargetPlayerCountUpdated(int playerCount)
    {
        if (playerCount > 0)
        {
            UILobby.instance.SetStartButtonActive(true);
        }
        else
        {
            UILobby.instance.SetStartButtonActive(false);
        }
    }

    #endregion

    #region  BEGIN MATCH

    public void BeginGame()
    {
        CmdBeginGame();
    }

    [Command]
    void CmdBeginGame()
    {
        MatchMaker.instance.BeginGame(matchID);
        Debug.Log($"<color=red>Game Beginning</color>");
        MymatchID = networkMatch.matchId.ToString(); //TODO : Удалить из переменных

        //Найдём наш триггер спавн
        GameObject TriggerSpawnMob = Instantiate((ShooterNetworkManager.singleton).spawnPrefabs
            .FirstOrDefault(x => x.name == "TriggerSpawnMob"));

        GameObject DefaultItemHP = Instantiate((ShooterNetworkManager.singleton).spawnPrefabs
    .FirstOrDefault(x => x.name == "DefaultItemHP"));

        GameObject DefaultItemDamage = Instantiate((ShooterNetworkManager.singleton).spawnPrefabs
.FirstOrDefault(x => x.name == "DefaultItemDamage"));

        GameObject DefaultItemMove = Instantiate((ShooterNetworkManager.singleton).spawnPrefabs
.FirstOrDefault(x => x.name == "DefaultItemMove"));

        GameObject DefaultItemAmmo = Instantiate((ShooterNetworkManager.singleton).spawnPrefabs
.FirstOrDefault(x => x.name == "DefaultItemAmmo"));

        GameObject DefaultItemGuard = Instantiate((ShooterNetworkManager.singleton).spawnPrefabs
.FirstOrDefault(x => x.name == "DefaultItemGuard"));

        //Укажем ему наш ID match
        TriggerSpawnMob.GetComponent<NetworkMatch>().matchId = networkMatch.matchId;
        DefaultItemHP.GetComponent<NetworkMatch>().matchId = networkMatch.matchId;
        DefaultItemDamage.GetComponent<NetworkMatch>().matchId = networkMatch.matchId;
        DefaultItemMove.GetComponent<NetworkMatch>().matchId = networkMatch.matchId;
        DefaultItemAmmo.GetComponent<NetworkMatch>().matchId = networkMatch.matchId;
        DefaultItemGuard.GetComponent<NetworkMatch>().matchId = networkMatch.matchId;

        NetworkServer.Spawn(TriggerSpawnMob);
        NetworkServer.Spawn(DefaultItemDamage);
        NetworkServer.Spawn(DefaultItemMove);
        NetworkServer.Spawn(DefaultItemHP);
        NetworkServer.Spawn(DefaultItemAmmo);
        NetworkServer.Spawn(DefaultItemGuard);
    }

    public void StartGame()
    { //Server
        TargetBeginGame();
    }

    [TargetRpc]
    void TargetBeginGame()
    {
        Debug.Log($"MatchID: {matchID} | Beginning");
        //Additively load game scene
        SceneManager.LoadScene(2, LoadSceneMode.Additive);

        //TODO : Будущее обновление. Если сервер будет загружать сцены
        //var sceneGame = SceneManager.GetSceneAt(1);
        //SceneManager.MoveGameObjectToScene(connectionToClient.identity.gameObject, sceneGame);

        UILobby.instance.gameObject.SetActive(false);
        Debug.Log($"Мой индекс " + playerIndex);
        Debug.Log($"Состояние сервер " + NetworkServer.active);
        GetComponent<PlayerData>().InputActive = true;
    }

    #endregion

    #endregion

    #region Server \ Client callback
    //Что делаем когда подключились к серверу.
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        //Спавним виртаульную камеру на сцену локально
        var vCam = Instantiate(Resources.LoadAsync("Prefabs/PlayerCommon/VirtualFollowCamera").asset as GameObject);
        //NetworkServer.Spawn(vCam);

        mainCamera = vCam.GetComponentInChildren<Camera>();

        vCamera = vCam.GetComponent<CinemachineVirtualCamera>();
        vCamera.Follow = transform;

        //TODO : Включить слушатель только на том клиенте на котором играем
        //if (isLocalPlayer) mainCamera.GetComponent<AudioListener>().enabled = true;
    }

    #endregion

    #region Awake, Start, Update, FixedUpdate

    void Awake()
    {
        playerMovementPlane = new Plane(transform.up, transform.position + transform.up);
        networkMatch = GetComponent<NetworkMatch>();
    }

    private void Start()
    {
        foreach (var pan in _panelsCanvas)
            if (isLocalPlayer)
                pan.SetActive(true);
    }



    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && _panelInfoItem.active != true)
        {
            if (playerData.InputActive)
            {
                StartMenu();
                EscapeMenu(true, false);
            }
            else EscapeMenu(false, true);
        }

        if (isLocalPlayer & Input.GetKeyDown(KeyCode.F1) && _panelEscape.active != true)
        {
            if (playerData.InputActive) InfoItemMenu(true, false);
            else InfoItemMenu(false, true);
        }
    }

    private void FixedUpdate()
    {
        if (hasAuthority)
        {
            //Arrow Key Input
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (playerData.InputActive) inputDirection = new Vector3(h, 0, v);
            else inputDirection = new Vector3(0, 0, 0);

            //Camera Direction
            var cameraForward = mainCamera.transform.forward;
            var cameraRight = mainCamera.transform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            //Try not to use var for roadshows or learning code
            Vector3 desiredDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;

            //Why not just pass the vector instead of breaking it up only to remake it on the other side?

            if (inputDirection.z > 0 || inputDirection.z < 0 || inputDirection.x > 0 || inputDirection.x < 0)
            {
                if (!_runPlayer.isPlaying)
                    _runPlayer.Play();
            }

            MoveThePlayer(desiredDirection);

            if (playerData.InputActive) TurnThePlayer();

            AnimateThePlayer(desiredDirection);
        }
    }

    #endregion

    #region Игровое меню
    public void EscapeMenu(bool active, bool input)
    {
        _panelEscape.SetActive(active);
        playerData.InputActive = input;
    }

    public void InfoItemMenu(bool active, bool input)
    {
        _panelInfoItem.SetActive(active);
        playerData.InputActive = input;
    }

    public void StartMenu()
    {
        _panelMain.SetActive(true);
        _panelExit.SetActive(false);
        _panelSetting.SetActive(false);

        _panelExit.GetComponent<RectTransform>().localPosition = new Vector3(0, 150.6f, 0);
        _panelSetting.GetComponent<RectTransform>().localPosition = new Vector3(0, 150.6f, 0);
    }

    #endregion

    #region Передвижение и вращение персонажа

    void MoveThePlayer(Vector3 desiredDirection)
    {
        movement.Set(desiredDirection.x, 0f, desiredDirection.z);

        movement = movement.normalized * playerData.SpeedPlayer * Time.deltaTime;

        playerRigidbody.MovePosition(transform.position + movement);
    }

    void TurnThePlayer()
    {
        Vector3 cursorScreenPosition = Input.mousePosition;

        Vector3 cursorWorldPosition = ScreenPointToWorldPointOnPlane(cursorScreenPosition, playerMovementPlane, mainCamera);

        playerToMouse = cursorWorldPosition - transform.position;

        playerToMouse.y = 0f;

        playerToMouse.Normalize();

        Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

        playerRigidbody.MoveRotation(Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * 10));
    }

    Vector3 PlaneRayIntersection(Plane plane, Ray ray)
    {
        float dist = 0.0f;
        plane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }

    Vector3 ScreenPointToWorldPointOnPlane(Vector3 screenPoint, Plane plane, Camera camera)
    {
        Ray ray = camera.ScreenPointToRay(screenPoint);
        return PlaneRayIntersection(plane, ray);
    }

    #endregion

    #region Анимация персонажа
    void AnimateThePlayer(Vector3 desiredDirection)
    {
        if (!playerAnimator)
            return;

        Vector3 movement = new Vector3(desiredDirection.x, 0f, desiredDirection.z);
        float forw = Vector3.Dot(movement, transform.forward);
        float stra = Vector3.Dot(movement, transform.right);

        playerAnimator.SetFloat("Forward", forw);
        playerAnimator.SetFloat("Strafe", stra);

        /*
		bool walking = h != 0f || v != 0f;

		if(walking)
		{
			Vector3 movement = new Vector3(h, 0f, v);
			float forw = Vector3.Dot(movement, transform.forward);
			float stra = Vector3.Dot(movement, transform.right);

			playerAnimator.SetFloat("Forward", forw);
			playerAnimator.SetFloat("Strafe", stra);
		}
		*/

    }

    public void DeadPlayer() => playerAnimator.SetBool("dead", true);
    #endregion
}

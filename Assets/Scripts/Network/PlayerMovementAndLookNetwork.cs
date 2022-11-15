using Cinemachine;
using Mirror;
using MirrorBasics;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerMovementAndLookNetwork : NetworkBehaviour
{
    #region Variables
    [Header("Panels")]
    [SerializeField] private GameObject _panelEscape;
    [SerializeField] private GameObject _panelSetting;
    [SerializeField] private GameObject _panelExit;
    [SerializeField] private GameObject _panelMain;

    //[Header("Camera")]
    //public Camera mainCamera;
    private Camera mainCamera;


    [Header("Movement")]
    public Rigidbody playerRigidbody;
    public float speed = 4.5f;
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

    private CinemachineVirtualCamera vCamera;

    #endregion

    #region Network Variables
    public static PlayerMovementAndLookNetwork localPlayer;
    [SyncVar] public string matchID;
    [SyncVar] public int playerIndex;

    [SyncVar] public int connId;

    [SyncVar] public string inScene = "";

    Mirror.NetworkMatch networkMatch;

    [SyncVar] public Match currentMatch;

    [SerializeField] GameObject playerLobbyUI;

    Guid netIDGuid;

    #endregion

    #region Network singleton

    public override void OnStartServer()
    {
        netIDGuid = netId.ToString().ToGuid();
        networkMatch.matchId = netIDGuid;
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            localPlayer = this;
        }
        else
        {
            Debug.Log($"Spawning other player UI Prefab");
            playerLobbyUI = UILobby.instance.SpawnPlayerUIPrefab(this);
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
        UILobby.instance.gameObject.SetActive(false);
        //GameObject.FindGameObjectWithTag("VirtualFollowCamera").GetComponent<CinemachineVirtualCamera>().Follow = transform;

        GetComponent<PlayerData>().InputActive = true;

        
    }


    #endregion

    #endregion

    //Что делаем когда подключились к серверу.
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        //Спавним виртаульную камеру на сцену локально
        var vCam = Instantiate(Resources.LoadAsync("Prefabs/VirtualFollowCamera").asset as GameObject);

        mainCamera = vCam.GetComponentInChildren<Camera>();

        vCamera = vCam.GetComponent<CinemachineVirtualCamera>();
        vCamera.Follow = transform;

        //TODO : Включить слушатель только на том клиенте на котором играем
        //if (isLocalPlayer) mainCamera.GetComponent<AudioListener>().enabled = true;
    }



    #region Awake, Start, Update, FixedUpdate

    void Awake()
    {
        playerMovementPlane = new Plane(transform.up, transform.position + transform.up);
        networkMatch = GetComponent<NetworkMatch>();
    }

    private void Start()
    {
        
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ShooterNetworkManager.singleton.PlayNewScene();
        }
        if (Input.GetButtonDown("Cancel"))
        {
            if (playerData.InputActive)
            {
                StartMenu();
                EscapeMenu(true, false);
            }
            else EscapeMenu(false, true);
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

        movement = movement.normalized * speed * Time.deltaTime;

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
    #endregion
}

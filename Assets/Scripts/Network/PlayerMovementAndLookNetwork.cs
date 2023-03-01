using Cinemachine;
using Mirror;
using MirrorBasics;
using Mono.CSharp;
using System;
using System.Linq;
using System.Threading.Tasks;
using Cosmoground;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Utils;
using Event = UnityEngine.Event;
using Random = System.Random;
using UnityEngine.Events;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

[Serializable]
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
    [SerializeField] private GameObject _gameObjectChatUI;

    private MainMenuManager _mainMenuManager;

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

    [SyncVar(hook = nameof(OnChangeMaterial))]
    private int materialChange;

    [SyncVar(hook = nameof(OnChangeMaterialSkin))]
    private int skinMaterialChange;

    [SerializeField] private PlayerData playerData;

    [SerializeField] private Renderer gunRenderer;

    [SerializeField] private SkinnedMeshRenderer skinRenderer;

    [SerializeField] private Material[] _gunMaterials;

    [SerializeField] private Material[] _skinMaterials;


    [Header("Animation")]
    public Animator playerAnimator;


    [Header("Audio VFX")]
    //[SerializeField] private AudioSource _runPlayer;

    [SerializeField] private StudioEventEmitter playerEventEmitter;

    internal CinemachineVirtualCamera vCamera;
    private float vCamAngele = 0;

    [Header("Tool")]
    [SerializeField] private GameObject _healthBarRpcLookAt;

    [SerializeField] private Texture2D _GunTexture;

    [SerializeField] private Camera LobbyCamera;

    [SerializeField] private UnityEvent OnStartGame;


    #region Delegate event
    //public delegate void ScorePlayerChanged(int score);
    //public event ScorePlayerChanged OnScorePlayerChanger;

    //public void ScoreCHANGED(int score)
    //{
    //TMPro_EventManager.TEXT_CHANGED_EVENT.Add(/*value*/);
    //}
    #endregion

    #endregion

    #region Network Variables
    public static PlayerMovementAndLookNetwork localPlayer;

    [Space(20)]
    [SyncVar] public string matchID;
    [SyncVar] public int playerIndex;

    [SyncVar] public string UserName;

    [SyncVar] public int connId;

    [SyncVar] public string inScene = "";

    [SyncVar] public string StatusGame = "";

    internal NetworkMatch networkMatch;

    public string MymatchID;

    [SyncVar] public Match currentMatch;

    [SerializeField] GameObject playerLobbyUI;

    Guid netIDGuid;

    [SyncVar] public int ZoneModule_ = 0;

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

        if (isLocalPlayer)
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


        ((ShooterNetworkManager)NetworkManager.singleton).spawnPrefabs.ForEach(x =>
        {
            if (x.tag == "Door")
            {
                var obj = Instantiate(x);
                obj.GetComponent<NetworkMatch>().matchId = networkMatch.matchId;
                NetworkServer.Spawn(obj);

                MatchMaker.ManagerLogic(matchID.ToGuid()).AddDoorInGameManager(obj.GetComponent<EventTrigger>());
            }
        });

        ((ShooterNetworkManager)NetworkManager.singleton).spawnPrefabs.ForEach(x =>
        {
            if (x.tag == "Props")
            {
                var obj = Instantiate(x);
                obj.GetComponent<NetworkMatch>().matchId = networkMatch.matchId;
                NetworkServer.Spawn(obj);
            }
        });

        ((ShooterNetworkManager)NetworkManager.singleton).spawnPrefabs.ForEach(x =>
        {
            if (x.tag == "BeginEnemy")
            {
                var obj = Instantiate(x);
                obj.GetComponent<NetworkMatch>().matchId = networkMatch.matchId;
                NetworkServer.Spawn(obj); //ОБРАТИ ВНИМАНИЕ СЮДА

                MatchMaker.ManagerLogic(matchID.ToGuid()).AddWaveInGameManager(obj.GetComponent<ManagerWave>());
            }
        });


        GameObject Level = Instantiate((ShooterNetworkManager.singleton).spawnPrefabs
 .FirstOrDefault(x => x.name == "Level"));

        Level.GetComponent<NetworkMatch>().matchId = matchID.ToGuid();
        
        NetworkServer.Spawn(Level);

        #region TEST BUFF
        GameObject TestItemBuffS = Instantiate((ShooterNetworkManager.singleton).spawnPrefabs
.FirstOrDefault(x => x.name == "RareItemRed"));

        TestItemBuffS.GetComponent<NetworkMatch>().matchId = matchID.ToGuid();

        NetworkServer.Spawn(TestItemBuffS);

        GameObject TestItemBuffD = Instantiate((ShooterNetworkManager.singleton).spawnPrefabs
    .FirstOrDefault(x => x.name == "DefaultItemDamage"));

        TestItemBuffD.GetComponent<NetworkMatch>().matchId = matchID.ToGuid();

        NetworkServer.Spawn(TestItemBuffD);

        GameObject TestItemBuffA = Instantiate((ShooterNetworkManager.singleton).spawnPrefabs
    .FirstOrDefault(x => x.name == "DefaultItemAmmo"));

        TestItemBuffA.GetComponent<NetworkMatch>().matchId = matchID.ToGuid();

        NetworkServer.Spawn(TestItemBuffA);

        GameObject TestItemBuffH = Instantiate((ShooterNetworkManager.singleton).spawnPrefabs
    .FirstOrDefault(x => x.name == "DefaultItemHP"));

        TestItemBuffH.GetComponent<NetworkMatch>().matchId = matchID.ToGuid();

        NetworkServer.Spawn(TestItemBuffH);

        GameObject TestItemBuffG = Instantiate((ShooterNetworkManager.singleton).spawnPrefabs
            .FirstOrDefault(x => x.name == "DefaultItemGuard"));

        TestItemBuffG.GetComponent<NetworkMatch>().matchId = matchID.ToGuid();

        NetworkServer.Spawn(TestItemBuffG);

        #endregion


        #region Пример добавления в менеджер объектов

        //GameObject TriggerSpawnMob = Instantiate((ShooterNetworkManager.singleton).spawnPrefabs
        //    .FirstOrDefault(x => x.name == "TriggerSpawnMob"));
        //TriggerSpawnMob.GetComponent<NetworkMatch>().matchId = matchID.ToGuid();
        //NetworkServer.Spawn(TriggerSpawnMob);
        //MatchMaker.ResycleObjects(matchID.ToGuid()).AddObjectWithMatch(TriggerSpawnMob);

        #endregion

    }

    public void StartGame()
    { //Server
        TargetBeginGame();
    }


    [Command]
    void CmdUpdateStat(string[] arr)
    {
        //Debug.LogWarning($"MaxCartridges {connectionToClient} == {int.Parse(arr[2])}") ;
        playerData.DamagePlayer = int.Parse(arr[1]);
        playerData.MaxCartridges = int.Parse(arr[2]);
        playerData.SpeedPlayer = int.Parse(arr[3]);
        playerData.AmmoReload = int.Parse(arr[4]); //TODO : тут нужно наверно использовать не AmmoReload потому что после бафа будет startAmmoReload
    }

    [TargetRpc]
    void TargetBeginGame()
    {
        var arr = PlayerPrefs.GetString("TypePilotStat").Split(new[] { "," }, StringSplitOptions.None);
        //Debug.LogWarning($"Урон == {int.Parse(arr[1])}");
        CmdUpdateStat(arr);

        var musicManager = GameObject.Find("MusicManager").GetComponent<ChangeTheme>();

        musicManager.ChangeMusic("Ambience");

       
        //TODO : Будущее обновление. Если сервер будет загружать сцены
        //var sceneGame = SceneManager.GetSceneAt(1);
        //SceneManager.MoveGameObjectToScene(connectionToClient.identity.gameObject, sceneGame);

        UILobby.instance.gameObject.SetActive(false);
        GetComponent<PlayerData>().InputIsActive(true);
        playerData.MenuInputIsActive(true);
        OnStartGame.Invoke();

        LobbyCamera.gameObject.SetActive(false);//Выключаем камеру лобби. 

        //Additively load game scene
        SceneManager.LoadScene(2, LoadSceneMode.Additive);

        Debug.Log("PLAYER GUID" + matchID.ToGuid());
    }

    #endregion

    #endregion

    #region Server \ Client callback
    //Что делаем когда подключились к серверу.
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        //Спавним виртаульную камеру на сцену локально
        var vCam = Instantiate(Resources.LoadAsync("Prefabs/VirtualFollowCamera").asset as GameObject);
        //NetworkServer.Spawn(vCam);

        mainCamera = vCam.GetComponentInChildren<Camera>();

        vCamera = vCam.GetComponent<CinemachineVirtualCamera>();
        vCamera.Follow = transform;

        //TODO : Включить слушатель только на том клиенте на котором играем
        //if (isLocalPlayer) mainCamera.GetComponent<AudioListener>().enabled = true;

    }

    /// <summary>
    /// Будем следить где игрок
    /// </summary>
    /// <param name="zone"></param>
    public void ZoneModuleChange(int zone) => ZoneModule_ = zone;

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

        _mainMenuManager = FindObjectOfType<MainMenuManager>();
        _mainMenuManager.playerData = playerData;
        _mainMenuManager.UploadStatPlayer();

    }

    private void OnChangeMaterial(int _Old, int _New)
    {
        gunRenderer.material = _gunMaterials[_New - 1];
    }

    private void OnChangeMaterialSkin(int _Old, int _New)
    {
        skinRenderer.material = _skinMaterials[_New - 1];
    }

    private int indexTextures = 1;
    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && _panelInfoItem.activeSelf != true && playerData.GetMenuInputActive())
        {
            if (/*playerData.GetInputActive()*/ _panelEscape.activeSelf != true)
            {
                StartMenu();
                EscapeMenu(true, false);
            }
            else EscapeMenu(false, true);
        }

        if (isLocalPlayer & Input.GetKeyDown(KeyCode.F1) && _panelEscape.activeSelf != true && playerData.GetMenuInputActive())
        {
            if (/*playerData.GetInputActive()*/ _panelInfoItem.activeSelf != true) InfoItemMenu(true, false);
            else InfoItemMenu(false, true);
        }

        if (isLocalPlayer & Input.GetKeyDown(KeyCode.I))
        {
            CmdSetupPlayer();
            if (indexTextures <= _skinMaterials.Length)
            {
                Debug.LogWarning(indexTextures);
                CmdSetupSkinPlayer(indexTextures);
                indexTextures++;
            }
            else
            {
                Debug.LogWarning(indexTextures);
                indexTextures = 1;
                CmdSetupSkinPlayer(indexTextures);
            }
        }

        if (isLocalPlayer & Input.GetKeyDown(KeyCode.E))
            _changeCameraAngle(-90);

        if (isLocalPlayer & Input.GetKeyDown(KeyCode.Q))
            _changeCameraAngle(90);

        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (isClient) Debug.LogWarning("Мы клиент");
            if (isOwned) Debug.LogWarning("Мы isOwned");

            if (!_gameObjectChatUI.activeSelf) _gameObjectChatUI.SetActive(true);
            else _gameObjectChatUI.SetActive(false);

        }

        if (isLocalPlayer & Input.GetKeyDown(KeyCode.F10))
        {
            CmdFind();
        }

    }

    [Command]
    void CmdFind()
    {
        var mat = MatchMaker.instance.matches.FirstOrDefault(p => p.players.Any(pl => pl.connectionToClient == connectionToClient));
        mat.players.ForEach(p => Debug.LogWarning($"{mat.matchID} Игрок матча {p.connectionToClient}"));
    }

    [Command]
    public void CmdSetupPlayer()
    {
        materialChange = 1;
    }

    [Command]
    public void CmdSetupSkinPlayer(int index)
    {
        skinMaterialChange = index;
    }

    private void FixedUpdate()
    {
        if (isOwned)
        {
            //Arrow Key Input
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (playerData.GetInputActive()) inputDirection = new Vector3(h, 0, v);
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
                if (!playerEventEmitter.IsPlaying())
                    playerEventEmitter.Play();
            }

            MoveThePlayer(desiredDirection);

            if (playerData.GetInputActive()) TurnThePlayer();

            AnimateThePlayer(desiredDirection);
        }
    }

    #endregion

    #region Игровое меню
    public void EscapeMenu(bool active, bool input)
    {
        _panelEscape.SetActive(active);
        if (playerData.isDead) input = false;
        else playerData.InputIsActive(input);
    }

    public void InfoItemMenu(bool active, bool input)
    {
        _panelInfoItem.SetActive(active);
        if (playerData.isDead) input = false;
        else playerData.InputIsActive(input);
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

    private void _changeCameraAngle(float _angle)
    {
        //Если угол превышает больше или меньше 180 градусов, мы меняем на положительное или отрицательное число чтобы не уйти за 180 градусов
        if (vCamAngele >= 180 & _angle == 90 || vCamAngele <= -180 & _angle == -90)
            _angle = _angle * -1;

        vCamAngele += _angle;

        var angles = vCamera.transform.rotation.eulerAngles;

        angles.y = vCamAngele;

        vCamera.transform.DORotate(angles, 3);
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

using UnityEngine;
using Mirror;
using MirrorBasics;
using System;
using UnityEngine.SceneManagement;
using Utils;
using Unity.Scenes;
using UnityEngine.ProBuilder.Shapes;
using Unity.Entities.UniversalDelegates;

[RequireComponent(typeof(NetworkMatch))]
public class ConnectionPlayer : NetworkBehaviour
{
    //#region Network Variables
    //public static ConnectionPlayer localPlayer;
    //[SyncVar] public string matchID;
    //[SyncVar] public int playerIndex;

    //[SerializeField] protected GameObject _playerCharacter = null;

    //[SyncVar] public int connId;

    //[SyncVar] public string inScene = "";

    //NetworkMatch networkMatch;

    //[SerializeField] GameNetworkManager networkManager;

    //[SyncVar] public Match currentMatch;

    //[SerializeField] GameObject playerLobbyUI;

    //Guid netIDGuid;

    //#endregion

    //#region property

    //public GameObject playerCharacter
    //{
    //    get
    //    {
    //        return _playerCharacter;
    //    }
    //    set
    //    {
    //        if (!GameObject.Equals(_playerCharacter, value))
    //        {
    //            _playerCharacter = value;
    //            if (NetworkServer.active)
    //                NetworkServer.ReplacePlayerForConnection(GetComponent<NetworkIdentity>().connectionToClient, _playerCharacter, true);
    //        }
    //    }
    //}

    //#endregion

    //#region Network singleton

    //public override void OnStartServer()
    //{
    //    netIDGuid = netId.ToString().ToGuid();
    //    networkMatch.matchId = netIDGuid;
    //}

    //public override void OnStartClient()
    //{
    //    if (isLocalPlayer)
    //    {
    //        localPlayer = this;
    //    }
    //    else
    //    {
    //        Debug.Log($"Spawning other player UI Prefab");
    //        playerLobbyUI = UILobby.instance.SpawnPlayerUIPrefab(this);
    //    }
    //}

    //public override void OnStopClient()
    //{
    //    Debug.Log($"Client Stopped");
    //    ClientDisconnect();
    //}

    //public override void OnStopServer()
    //{
    //    Debug.Log($"Client Stopped on Server");
    //    ServerDisconnect();
    //}

    //#region HOST MATCH

    //public void HostGame(bool publicMatch)
    //{
    //    string matchID = MatchMaker.GetRandomMatchID();
    //    CmdHostGame(matchID, publicMatch);
    //}

    //[Command]
    //void CmdHostGame(string _matchID, bool publicMatch)
    //{
    //    matchID = _matchID;
    //    if (MatchMaker.instance.HostGame(_matchID, this, publicMatch, out playerIndex))
    //    {
    //        Debug.Log($"<color=green>Game hosted successfully</color>");
    //        networkMatch.matchId = _matchID.ToGuid();
    //        TargetHostGame(true, _matchID, playerIndex);
    //    }
    //    else
    //    {
    //        Debug.Log($"<color=red>Game hosted failed</color>");
    //        TargetHostGame(false, _matchID, playerIndex);
    //    }
    //}

    //[TargetRpc]
    //void TargetHostGame(bool success, string _matchID, int _playerIndex)
    //{
    //    playerIndex = _playerIndex;
    //    matchID = _matchID;
    //    Debug.Log($"MatchID: {matchID} == {_matchID}");
    //    UILobby.instance.HostSuccess(success, _matchID);
    //}

    //#endregion

    //#region  JOIN MATCH

    //public void JoinGame(string _inputID)
    //{
    //    CmdJoinGame(_inputID);
    //}

    //[Command]
    //void CmdJoinGame(string _matchID)
    //{
    //    matchID = _matchID;
    //    if (MatchMaker.instance.JoinGame(_matchID, this, out playerIndex))
    //    {
    //        Debug.Log($"<color=green>Game Joined successfully</color>");
    //        networkMatch.matchId = _matchID.ToGuid();
    //        TargetJoinGame(true, _matchID, playerIndex);

    //        //Host
    //        if (isServer && playerLobbyUI != null)
    //        {
    //            playerLobbyUI.SetActive(true);
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log($"<color=red>Game Joined failed</color>");
    //        TargetJoinGame(false, _matchID, playerIndex);
    //    }
    //}

    //[TargetRpc]
    //void TargetJoinGame(bool success, string _matchID, int _playerIndex)
    //{
    //    playerIndex = _playerIndex;
    //    matchID = _matchID;
    //    Debug.Log($"MatchID: {matchID} == {_matchID}");
    //    UILobby.instance.JoinSuccess(success, _matchID);
    //}

    //#endregion

    //#region DISCONNECT

    //public void DisconnectGame()
    //{
    //    CmdDisconnectGame();
    //}

    //[Command]
    //void CmdDisconnectGame()
    //{
    //    ServerDisconnect();
    //}

    //void ServerDisconnect()
    //{
    //    MatchMaker.instance.PlayerDisconnected(this, matchID);
    //    RpcDisconnectGame();
    //    networkMatch.matchId = netIDGuid;
    //}

    //[ClientRpc]
    //void RpcDisconnectGame()
    //{
    //    ClientDisconnect();
    //}

    //void ClientDisconnect()
    //{
    //    if (playerLobbyUI != null)
    //    {
    //        if (!isServer)
    //        {
    //            Destroy(playerLobbyUI);
    //        }
    //        else
    //        {
    //            playerLobbyUI.SetActive(false);
    //        }
    //    }
    //}

    //#endregion

    //#region SEARCH MATCH

    //public void SearchGame()
    //{
    //    CmdSearchGame();
    //}

    //[Command]
    //void CmdSearchGame()
    //{
    //    if (MatchMaker.instance.SearchGame(this, out playerIndex, out matchID))
    //    {
    //        Debug.Log($"<color=green>Game Found Successfully</color>");
    //        networkMatch.matchId = matchID.ToGuid();
    //        TargetSearchGame(true, matchID, playerIndex);

    //        //Host
    //        if (isServer && playerLobbyUI != null)
    //        {
    //            playerLobbyUI.SetActive(true);
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log($"<color=red>Game Search Failed</color>");
    //        TargetSearchGame(false, matchID, playerIndex);
    //    }
    //}

    //[TargetRpc]
    //void TargetSearchGame(bool success, string _matchID, int _playerIndex)
    //{
    //    playerIndex = _playerIndex;
    //    matchID = _matchID;
    //    Debug.Log($"MatchID: {matchID} == {_matchID} | {success}");
    //    UILobby.instance.SearchGameSuccess(success, _matchID);
    //}

    //#endregion

    //#region  MATCH PLAYERS

    //[Server]
    //public void PlayerCountUpdated(int playerCount)
    //{
    //    TargetPlayerCountUpdated(playerCount);
    //}

    ///// <summary>
    ///// ������� ������� ���������� � �������
    ///// </summary>
    ///// <param name="playerCount"></param>
    //[TargetRpc]
    //void TargetPlayerCountUpdated(int playerCount)
    //{
    //    if (playerCount > 0)
    //    {
    //        UILobby.instance.SetStartButtonActive(true);
    //    }
    //    else
    //    {
    //        UILobby.instance.SetStartButtonActive(false);
    //    }
    //}

    //#endregion

    //#region  BEGIN MATCH

    //public void BeginGame()
    //{
    //    CmdBeginGame();
    //}

    //[Command]
    //void CmdBeginGame()
    //{
    //    MatchMaker.instance.BeginGame(matchID);
    //    Debug.Log($"<color=red>Game Beginning</color>");
    //}

    //public void StartGame()
    //{ //Server
    //    TargetBeginGame();
    //}

    //[TargetRpc]
    //void TargetBeginGame()
    //{
    //    Debug.Log($"MatchID: {matchID} | Beginning");
    //    //Additively load game scene
    //    SceneManager.LoadScene(2, LoadSceneMode.Additive);
    //}


    //#endregion

    //#endregion

    //private void Awake()
    //{
    //    networkMatch = GetComponent<NetworkMatch>();
    //}

}

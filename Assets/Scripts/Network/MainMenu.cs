using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.InputSystem.HID;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

[Serializable]
public class Match : NetworkBehaviour
{
    public string ID;
    public readonly List<GameObject> Players = new List<GameObject>();

    public Match(string iD, GameObject players)
    {
        ID = iD;
        Players.Add(players);
    }
}
public class MainMenu : NetworkBehaviour
{
    public static MainMenu instance;
    public readonly SyncList<Match> Matches = new SyncList<Match>();
    public readonly SyncList<string> MatchesID = new SyncList<string>();
    public TMP_InputField JoinInput;
    public Button HostButton;
    public Button JoinButton;
    public Canvas LobbyCanvas;

    public Transform UIPlayerParent;
    public GameObject UIPlayerPrefab;
    public TextMeshProUGUI IDText;
    public Button BeginGameButton;
    public GameObject TurnManager;
    public bool inGame;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (!inGame)
        {
            PlayerMovementAndLookNetwork[] players = FindObjectsOfType<PlayerMovementAndLookNetwork>();

            for (int i = 0; i < players.Length; i++)
                players[i].gameObject.transform.localScale = Vector3.zero;
        }
    }

    #region Host Manager
    public void Host()
    {
        JoinInput.interactable = false;
        HostButton.interactable = false;
        JoinButton.interactable = false;

        PlayerMovementAndLookNetwork.localPlayer.HostGame();
    }

    public void HostSuccess(bool success, string matchID)
    {
        if (success)
        {
            LobbyCanvas.enabled = true;
            SpawnPlayerUIPrefab(PlayerMovementAndLookNetwork.localPlayer);
            IDText.text = matchID;
            BeginGameButton.interactable = true;
        }

        else
        {
            JoinInput.interactable = true;
            HostButton.interactable = true;
            JoinButton.interactable = true;
        }
    }
    #endregion

    #region Join manager
    public void Join()
    {
        JoinInput.interactable = false;
        HostButton.interactable = false;
        JoinButton.interactable = false;

        PlayerMovementAndLookNetwork.localPlayer.JoinGame(JoinInput.text.ToUpper());
    }

    public void JoinSuccess(bool success, string matchID)
    {
        if (success)
        {
            LobbyCanvas.enabled = true;
            SpawnPlayerUIPrefab(PlayerMovementAndLookNetwork.localPlayer);
            IDText.text = matchID;
            BeginGameButton.interactable = false;
        }

        else
        {
            JoinInput.interactable = true;
            HostButton.interactable = true;
            JoinButton.interactable = true;
        }
    }
    #endregion

    public bool HostGame(string matchID, GameObject player)
    {
        if (!MatchesID.Contains(matchID))
        {
            Matches.Add(new Match(matchID, player));
            return true;
        }

        else return false;
    }

    public bool JoinGame(string matchID, GameObject player)
    {
        //Если найдём матчи получим их
        if (MatchesID.Contains(matchID))
        {
            for (int i = 0; i < Matches.Count; i++)
            {
                //Если совпадают ID засунем в матч игрока и остановим цикл
                if (Matches[i].ID == matchID)
                {
                    Matches[i].Players.Add(player);
                    break;
                }
            }

            return true;
        }

        else return false;
    }

    public static string GetRandomID()
    {
        string ID = string.Empty;

        for (int i = 0; i < 5; i++)
        {
            int rnd = UnityEngine.Random.Range(0, 36);

            if (rnd < 26) ID += (char)(rnd + 65);
            else ID += (rnd - 26).ToString();
        }

        return ID;
    }

    public void SpawnPlayerUIPrefab(PlayerMovementAndLookNetwork player)
    {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
        newUIPlayer.GetComponent<PlayerUI>().SetPlayer(player);
    }

    public void StartGame() => PlayerMovementAndLookNetwork.localPlayer.BeginGame();

    public void BeginGame(string matchID)
    {
        GameObject newTurnManager = Instantiate(TurnManager);
        NetworkServer.Spawn(newTurnManager);
        newTurnManager.GetComponent<NetworkMatch>().matchId = matchID.ToGuid();
        TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();

        for (int i = 0; i < Matches.Count; i++)
        {
            if (Matches[i].ID == matchID)
            {
                foreach (var player in Matches[i].Players)
                {
                    PlayerMovementAndLookNetwork player1 = player.GetComponent<PlayerMovementAndLookNetwork>();
                    turnManager.AddPlayers(player1);
                    player1.StartGame();
                }
                break;
            }
        }
    }

}

public static class MatchExtension
{
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hasBytes = provider.ComputeHash(inputBytes);

        return new Guid(hasBytes);
    }
}

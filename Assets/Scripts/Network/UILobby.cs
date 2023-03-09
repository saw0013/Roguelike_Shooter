using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorBasics {

    public class UILobby : MonoBehaviour {

        public static UILobby instance;

        [Header ("Host Join")]
        [SerializeField] TMP_InputField joinMatchInput;
        [SerializeField] List<Selectable> lobbySelectables = new List<Selectable> ();
        [SerializeField] Canvas lobbyCanvas;
        [SerializeField] Canvas searchCanvas;
        bool searching = false;

        [Header ("Lobby")]
        [SerializeField] Transform UIPlayerParent;
        [SerializeField] GameObject UIPlayerPrefab;
        [SerializeField] Text matchIDText;
        [SerializeField] GameObject beginGameButton;
        public TMP_Text TextStatusGame; //Статус комнаты

        [SerializeField] private TMP_Text LogLobbyConnection;

        GameObject localPlayerLobbyUI;

        private BroadcastService bs = null;


        void Start () {
            instance = this;

            bs = new BroadcastService();
            bs.Initialize();
            bs.OnMessageRecived += Bs_OnMessageRecived;
            bs.OnMessageGroupRecived += Bs_OnMessageGroupRecived;
        }

        public void SetStartButtonActive (bool active) {
            beginGameButton.SetActive (active);
        }

        public void HostPublic () {
            lobbySelectables.ForEach (x => x.interactable = false);
            TextStatusGame.text = "Вы публичный хост";
            PlayerMovementAndLookNetwork.localPlayer.HostGame (true);

        }

        public void HostPrivate () {
            lobbySelectables.ForEach (x => x.interactable = false);
            TextStatusGame.text = "Вы приватный хост";
            PlayerMovementAndLookNetwork.localPlayer.HostGame (false);
        }

        public void HostSuccess (bool success, string matchID) {
            if (success) {
                lobbyCanvas.enabled = true;

                if (localPlayerLobbyUI != null) Destroy (localPlayerLobbyUI);
                localPlayerLobbyUI = SpawnPlayerUIPrefab (PlayerMovementAndLookNetwork.localPlayer);
                matchIDText.text = matchID;

                TextStatusGame.text = "Хост подтверждён";
                InitializeChatRoom(matchID, PlayerMovementAndLookNetwork.localPlayer.UserName);
            } else {
                lobbySelectables.ForEach (x => x.interactable = true);
            }
        }

        public void Join () {
            lobbySelectables.ForEach (x => x.interactable = false);

            PlayerMovementAndLookNetwork.localPlayer.JoinGame (joinMatchInput.text.ToUpper ());
        }

        public void JoinSuccess (bool success, string matchID) {
            if (success) {
                lobbyCanvas.enabled = true;

                if (localPlayerLobbyUI != null) Destroy (localPlayerLobbyUI);
                localPlayerLobbyUI = SpawnPlayerUIPrefab (PlayerMovementAndLookNetwork.localPlayer);
                matchIDText.text = matchID;
                TextStatusGame.text = "Ожидайте лидера лобби...";

                JoinChatRoom(matchID, PlayerMovementAndLookNetwork.localPlayer.UserName, "Подключился к группе!");

            } else {
                lobbySelectables.ForEach (x => x.interactable = true);
            }
        }

        public void DisconnectGame () {
            if (localPlayerLobbyUI != null) Destroy (localPlayerLobbyUI);
            PlayerMovementAndLookNetwork.localPlayer.DisconnectGame ();

            lobbyCanvas.enabled = false;
            lobbySelectables.ForEach (x => x.interactable = true);
        }

        public GameObject SpawnPlayerUIPrefab (PlayerMovementAndLookNetwork player) {
            player.UserName = player.GetComponent<PlayerData>()._nameDisplay;
            GameObject newUIPlayer = Instantiate (UIPlayerPrefab, UIPlayerParent);
            newUIPlayer.GetComponent<UIPlayer> ().SetPlayerName(player.UserName);
            newUIPlayer.transform.SetSiblingIndex (player.playerIndex - 1);

            return newUIPlayer;
        }

        public void BeginGame () {
            PlayerMovementAndLookNetwork.localPlayer.BeginGame ();
            GetComponent<MainMenuManager>().Fade(); //Вызывается только у локального игрока лидера группы
        }

        public void SearchGame () {
            StartCoroutine (Searching ());
        }

        public void CancelSearchGame () {
            searching = false;
        }

        public void SearchGameSuccess (bool success, string matchID) {
            if (success) {
                searchCanvas.enabled = false;
                searching = false;
                JoinSuccess (success, matchID);
            }
        }

        IEnumerator Searching () {
            searchCanvas.enabled = true;
            searching = true;

            float searchInterval = 1;
            float currentTime = 1;

            while (searching) {
                if (currentTime > 0) {
                    currentTime -= Time.deltaTime;
                } else {
                    currentTime = searchInterval;
                    PlayerMovementAndLookNetwork.localPlayer.SearchGame ();
                }
                yield return null;
            }
            searchCanvas.enabled = false;
        }


        private async void InitializeChatRoom(string roomId, string username)
        {
            //bs.SendTest("AABB1", "anomal3"); //Помечен устаревшим
            await bs.InitializeGroupAsync(roomId, username);
        }

     

        private async void JoinChatRoom(string roomId, string username, string message)
        {
            await bs.SendMessageGroupAsync(roomId, username, message);
        }

        private void Bs_OnMessageRecived(string obj)
        {
            LogLobbyConnection.text += obj + "\r\n";
            Debug.LogWarning("<color=Blue>SignalR</color>: Вызван асинхронный Action в потоке " + System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        private void Bs_OnMessageGroupRecived(string arg1, string arg2)
        {
            LogLobbyConnection.text += arg1 + ": " + arg2  + "\r\n";
        }
    }
}
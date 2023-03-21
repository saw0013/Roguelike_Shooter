using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Cosmoground
{
    public class GameChatUISignalR : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] Text chatHistory;
        [SerializeField] Scrollbar scrollbar;
        [SerializeField] InputField chatMessage;
        [SerializeField] Button sendButton;

        private BroadcastService bs = new BroadcastService();



        // This is only set on client to the name of the local player
        public static string localPlayerName;


        private void Start()
        {
            chatHistory.text = "";
        }

        /// <summary>
        /// Инициируем чат только при старте матча
        /// </summary>
        public async void BeginGame()
        {
            bs.Initialize();
            bs.OnMessageGroupRecived += Bs_OnMessageGroupRecived; ;
            localPlayerName = GetComponent<PlayerData>()._nameDisplay;
            await bs.InitializeMatchAsync(GetComponent<PlayerMovementAndLookNetwork>().matchID);
        }

        private void Bs_OnMessageGroupRecived(string arg1, string arg2)
        {
            string prettyMessage = arg1 == localPlayerName ?
                $"<color=\"red\">{arg1}:</color> {arg2.Trim()}" :
                $"<color=\"blue\">{arg1}:</color> {arg2.Trim()}";

            StartCoroutine(AppendAndScroll(prettyMessage));
        }

        IEnumerator AppendAndScroll(string message)
        {
            chatHistory.text += message + "\n";

            // it takes 2 frames for the UI to update ?!?!
            yield return null;
            yield return null;

            // slam the scrollbar down
            scrollbar.value = 0;
        }

        public void ToggleButton(string input)
        {
            sendButton.interactable = !string.IsNullOrWhiteSpace(input);
        }

        // Called by UI element MessageField.OnEndEdit
        public void OnEndEdit(string input)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) ||
                Input.GetButtonDown("Submit"))
                SendMessage();

        }

        // Called by OnEndEdit above and UI element SendButton.OnClick
        public void SendMessage()
        {
            //Debug.LogWarning($"Cxbnft");
            if (!string.IsNullOrWhiteSpace(chatMessage.text))
            {
                SendMsg(chatMessage.text.Trim());
                chatMessage.text = string.Empty;
                chatMessage.ActivateInputField();
            }
        }


        async void SendMsg(string message)
        {
            await bs.SendMessageGroupAsync(GetComponent<PlayerMovementAndLookNetwork>().matchID,
                localPlayerName, message);
        }


    }
}

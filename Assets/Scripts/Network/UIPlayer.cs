using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorBasics {

    public class UIPlayer : NetworkBehaviour {

        [SerializeField] Text text;

        [SyncVar]
        public string UserName;
        PlayerMovementAndLookNetwork player;

        [Obsolete("Использовать SetPlayerName")]
        public void SetPlayer (PlayerMovementAndLookNetwork player) {
            this.player = player;
            UserName = player.GetComponent<PlayerData>()._nameDisplay;
            text.text = /*player.UserName*/UserName + " " + player.playerIndex.ToString();
            Debug.LogWarning(player.GetComponent<PlayerData>()._nameDisplay + " _в UIPLAYER");
        }

        //TODO : Изменить имя
        public void SetPlayerName(string name)
        {
            text.text = name;
            Debug.LogWarning("ВАШ ИМЯ " + name);
        }
    }
}
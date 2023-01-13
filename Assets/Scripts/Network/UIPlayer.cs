using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorBasics {

    public class UIPlayer : NetworkBehaviour {

        [SerializeField] Text text;

       
        PlayerMovementAndLookNetwork player;

        [Obsolete("Использовать SetPlayerName")]
        public void SetPlayer (PlayerMovementAndLookNetwork player) {
            this.player = player;
           
            text.text = player.UserName + " " + player.playerIndex.ToString();
            Debug.LogWarning(player.GetComponent<PlayerData>()._nameDisplay + " _в UIPLAYER");
        }

        //FIX : Изменить имя
        public void SetPlayerName(string name)
        {
            text.text = name;
        }
    }
}
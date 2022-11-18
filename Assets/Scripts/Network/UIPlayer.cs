using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorBasics {

    public class UIPlayer : MonoBehaviour {

        [SerializeField] Text text;
        PlayerMovementAndLookNetwork player;

        public void SetPlayer (PlayerMovementAndLookNetwork player) {
            this.player = player;
            text.text = player.UserName + " " + player.playerIndex.ToString();
        }

        public void SetPlayerName(string name) => text.text = name;
    }
}
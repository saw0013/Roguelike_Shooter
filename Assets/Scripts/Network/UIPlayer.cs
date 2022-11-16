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
            text.text = PlayerPrefs.GetString("PlayerName") + " " + player.playerIndex.ToString ();
        }

        public void SetPlayer2(PlayerMovementAndLookNetwork player, string name)
        {
            this.player = player;
            if(name == PlayerPrefs.GetString("PlayerName"))
            {
                text.text = name + " " + player.playerIndex.ToString();
            }
            else
            {
                text.text = PlayerPrefs.GetString("PlayerName") + " " + player.playerIndex.ToString();
            }
        }
    }
}
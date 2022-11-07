using System.Collections;
using System.Collections.Generic;
using Mirror.Examples.Basic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI PlayerText;
    private PlayerMovementAndLookNetwork Player;

    public void SetPlayer(PlayerMovementAndLookNetwork player)
    {
        this.Player = player;
        PlayerText.text = "»Ãﬂ";
    }
}

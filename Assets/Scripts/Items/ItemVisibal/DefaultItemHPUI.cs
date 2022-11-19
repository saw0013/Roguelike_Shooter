using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultItemHPUI : MonoBehaviour
{
    private PlayerData owner;

    public void RegisterOwner(PlayerData ownerItem) => owner = ownerItem;
}
